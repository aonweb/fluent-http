using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{

    //TODO: create inverted indexes for allow global cache expiry by some index
    // this class is inspired by the excellent work in CacheCow Caching libraries - https://github.com/aliostad/CacheCow
    // for whatever reason, I couldn't get the inmemory cache in the HttpClientHandler / CachingHandler 
    // to play nice with call / client builders, and the cache kept disappearing
    // additionally I need an implementation that allows for deserialized object level caching in addition to http response caching
    // so I implemented a cachehandler that plugs in to the HttpCallBuilder higher up,
    // but the base logic for cache validation / invalidation was based off CacheCow
    public abstract class CacheHandlerBase<TResult>
    {
        protected CacheHandlerBase(CacheSettings<TResult> settings)
        {
            Settings = settings;
        }

        protected CacheSettings<TResult> Settings { get; set; }

        protected async Task<CacheResult<TResult>> TryGetFromCache(IHttpCallContext callContext, HttpRequestMessage request)
        {
            if (BypassCache(request))
                return CacheResult<TResult>.Empty;

            var cacheContext = CreateCacheContext(request);

            cacheContext.Result = await Settings.CacheStore.GetCachedResult(cacheContext);

            if (cacheContext.Result.Found)
            {
                if (Settings.ResultInspector != null)
                    Settings.ResultInspector(cacheContext.Result);

                cacheContext.ValidationResult = ValidateResult(cacheContext.Result.ResponseInfo);
            }

            // TODO: Determine the need for conditional put - if so, that logic would go here

            if (cacheContext.ValidationResult == ResponseValidationResult.OK)
            {
                return cacheContext.Result;
            }

            if (cacheContext.ValidationResult == ResponseValidationResult.Stale)
            {
                if (AllowStaleResult(request, cacheContext.Result.ResponseInfo))
                    return cacheContext.Result;		
                
                cacheContext.ValidationResult = ResponseValidationResult.MustRevalidate;
            }

            if (cacheContext.ValidationResult == ResponseValidationResult.MustRevalidate)
            {
                if (cacheContext.Result.ResponseInfo.ETag != null)
                    request.Headers.Add("If-None-Match", cacheContext.Result.ResponseInfo.ETag.ToString());
                else if (cacheContext.Result.ResponseInfo.LastModified != null)
                    request.Headers.Add("If-Modified-Since", cacheContext.Result.ResponseInfo.LastModified.Value.ToString("r"));

                //hang on to this we are going to need it in a sec. We could try to get it from cache store, but it may have expired by then
                callContext.Items["CacheHandlerCachedItem"] = cacheContext.Result;
            }

            return CacheResult<TResult>.Empty;
        }

        protected CacheResult<TResult> TryGetRevalidatedResult(IHttpCallContext callContext, HttpResponseMessage response)
        {
            if (BypassRevalidate(response))
                return CacheResult<TResult>.Empty;

            var context = CreateCacheContext(response.RequestMessage);

            context.Result = callContext.Items["CacheHandlerCachedItem"] as CacheResult<TResult>;

            if (context.Result != null)
            {
                context.Result.UpdateResponseInfo(response, Settings);

                if (Settings.ResultInspector != null)
                    Settings.ResultInspector(context.Result);
            }
            else
            {
                context.Result = CacheResult<TResult>.Empty;
            }

            Helper.DisposeResponse(response);
            
            return context.Result;
        }

        protected async Task TryCacheResult(HttpResponseMessage response, TResult result)
        {
            var cacheContext = CreateCacheContext(response.RequestMessage);

            cacheContext.Result = new CacheResult<TResult>(result, response, Settings);

            if (!BypassCache(response.RequestMessage))
            {
                cacheContext.ValidationResult = ValidateResult(cacheContext.Result.ResponseInfo);

                if (cacheContext.ValidationResult == ResponseValidationResult.OK
                    || cacheContext.ValidationResult == ResponseValidationResult.MustRevalidate)
                {
                    await Settings.CacheStore.AddOrUpdate(cacheContext);
                    Settings.VaryByStore.AddOrUpdate(cacheContext.Uri, response.Headers.Vary);
                    return;
                }
            }

            Settings.CacheStore.TryRemove(cacheContext);
        }

        private bool AllowStaleResult(HttpRequestMessage request, ResponseInfo responseInfo)
        {
            //This is almost verbatim from the CacheCow CachingHandler's  IsFreshOrStaleAcceptable 

            if (responseInfo == null)
                throw new ArgumentNullException("responseInfo");

            if (request == null)
                throw new ArgumentNullException("request");

            if (responseInfo.HasContent)
                return false;

            var responseDate = responseInfo.LastModified ?? responseInfo.Date;

            if (responseInfo.HasExpiration)
                if(responseInfo.Expiration < DateTimeOffset.UtcNow)
                    return false;

            var staleness = DateTimeOffset.UtcNow - responseDate;

            if (request.Headers.CacheControl == null)
                return staleness < TimeSpan.Zero;

            if (request.Headers.CacheControl.MinFresh.HasValue)
                return -staleness > request.Headers.CacheControl.MinFresh.Value; // staleness is negative if still fresh

            if (request.Headers.CacheControl.MaxStale) // stale acceptable
                return true;

            if (request.Headers.CacheControl.MaxStaleLimit.HasValue)
                return staleness < request.Headers.CacheControl.MaxStaleLimit.Value;

            if (request.Headers.CacheControl.MaxAge.HasValue)
                return responseDate.Add(request.Headers.CacheControl.MaxAge.Value) > DateTimeOffset.Now;

            return false;
        }

        private ResponseValidationResult ValidateResult(ResponseInfo responseInfo)
        {
            //This is almost verbatim from the CacheCow CachingHandler's ResponseValidator func

            // 13.4
            //Unless specifically constrained by a cache-control (section 14.9) directive, a caching system MAY always store 
            // a successful response (see section 13.8) as a cache entry, MAY return it without validation if it 
            // is fresh, and MAY return it after successful validation. If there is neither a cache validator nor an 
            // explicit expiration time associated with a response, we do not expect it to be cached, but certain caches MAY violate this expectation 
            // (for example, when little or no network connectivity is available).

            // 14.9.1
            // If the no-cache directive does not specify a field-name, then a cache MUST NOT use the response to satisfy a subsequent request without 
            // successful revalidation with the origin server. This allows an origin server to prevent caching 
            // even by caches that have been configured to return stale responses to client requests.
            //If the no-cache directive does specify one or more field-names, then a cache MAY use the response 
            // to satisfy a subsequent request, subject to any other restrictions on caching. However, the specified 
            // field-name(s) MUST NOT be sent in the response to a subsequent request without successful revalidation 
            // with the origin server. This allows an origin server to prevent the re-use of certain header fields in a result, while still allowing caching of the rest of the response.
            if (!Settings.CacheableStatusCodes.Contains(responseInfo.StatusCode))
                return ResponseValidationResult.NotCacheable;

            if (responseInfo.NoStore)
                return ResponseValidationResult.NotCacheable;

            if (!responseInfo.HasContent)
                return ResponseValidationResult.NotCacheable;

            if (!responseInfo.HasExpiration)
                return ResponseValidationResult.NotCacheable;

            if (responseInfo.NoCache)
                return ResponseValidationResult.MustRevalidate;

            if (responseInfo.Expiration < DateTimeOffset.UtcNow)
                return responseInfo.ShouldRevalidate ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

            return ResponseValidationResult.OK;
        }

        private bool BypassCache(HttpRequestMessage request)
        {
            if (!Settings.Enabled)
                return true;

            if (!Settings.CacheableMethods.Contains(request.Method))
                return true;

            // client can tell CachingHandler not to do caching for a particular request
            // rather than expiring here and facing a thundering herd, let a success repopulate
            if (request.Headers.CacheControl != null)
            {
                if (request.Headers.CacheControl.NoStore)
                    return true;
            }

            return false;
        }

        private bool BypassRevalidate(HttpResponseMessage response)
        {
            if (!Settings.Enabled)
                return true;

            if (!Settings.CacheableMethods.Contains(response.RequestMessage.Method))
                return true;

            return response.StatusCode != HttpStatusCode.NotModified;
        }

        private CacheContext<TResult> CreateCacheContext(HttpRequestMessage request)
        {
            return new CacheContext<TResult>(Settings, request.RequestUri, request.Headers);
        }
    }
}