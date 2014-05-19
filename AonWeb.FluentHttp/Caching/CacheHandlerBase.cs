using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    // this class is inspired by the excellent work in CacheCow Caching libraries - https://github.com/aliostad/CacheCow
    // for whatever reason, I couldn't get the inmemory cache in the HttpClientHandler / CachingHandler 
    // to play nice with call / client builders, and the cache kept disappearing
    // additionally I need an implementation that allows for deserialized object level caching in addition to http response caching
    // so I implemented a cachehandler that plugs in to the HttpCallBuilder higher up,
    // but the base logic for cache validation / invalidation was based off CacheCow
    public abstract class CacheHandlerBase<TResult>
    {
        protected CacheHandlerBase()
        {
            Settings = new CacheSettings<TResult>();
        }

        protected CacheHandlerBase(CacheSettings<TResult> settings)
        {
            Settings = settings;
        }

        protected CacheSettings<TResult> Settings { get; set; }

        #region HttpCallHandler Implementation

        public bool Enabled
        {
            get
            {
                return Settings.Enabled;
            }
            set
            {
                Settings.Enabled = value;
            }
        }

        public HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            if (type == HttpCallHandlerType.Sending)
                return HttpCallHandlerPriority.First;

            if (type == HttpCallHandlerType.Sent)
                return HttpCallHandlerPriority.Last;

            return HttpCallHandlerPriority.Default;
        }

        #endregion

        protected CacheContext<TResult> CreateCacheContext(IHttpCallContext callContext, HttpRequestMessage request)
        {
            return new CacheContext<TResult>(Settings, callContext, request);
        }

        protected CacheContext<TResult> CreateCacheContext(IHttpCallContext callContext, HttpResponseMessage response)
        {
            return CreateCacheContext(callContext, response.RequestMessage);
        }

        protected async Task TryGetFromCache(CacheContext<TResult> context)
        {
            if (!context.CacheValidator(context))
                return;

            context.CacheResult = await Settings.CacheStore.GetCachedResult(context);

            if (context.CacheResult.Found)
            {
                if (context.ResultInspector != null)
                    context.ResultInspector(context.CacheResult);

                context.ValidationResult = context.ResponseValidator(context);
            }

            // TODO: Determine the need for conditional put - if so, that logic would go here

            if (context.ValidationResult == ResponseValidationResult.OK)
                return;
            if (context.ValidationResult == ResponseValidationResult.Stale)
            {
                if (context.AllowStaleResultValidator(context))
                    return;

                context.ValidationResult = ResponseValidationResult.MustRevalidate;
            }

            if (context.ValidationResult == ResponseValidationResult.MustRevalidate)
            {
                if (context.CacheResult.ResponseInfo.ETag != null)
                    context.Request.Headers.Add("If-None-Match", context.CacheResult.ResponseInfo.ETag.ToString());
                else if (context.CacheResult.ResponseInfo.LastModified != null)
                    context.Request.Headers.Add("If-Modified-Since", context.CacheResult.ResponseInfo.LastModified.Value.ToString("r"));

                //hang on to this we are going to need it in a sec. We could try to get it from cache store, but it may have expired by then
                context.Items["CacheHandlerCachedItem"] = context.CacheResult;
            }
        }

        protected void TryGetRevalidatedResult(CacheContext<TResult> context, HttpResponseMessage response)
        {
            if (!context.RevalidateValidator(context))
                return;

            context.CacheResult = context.Items["CacheHandlerCachedItem"] as CacheResult<TResult>;

            if (context.CacheResult != null && context.ResultFound)
            {
                context.CacheResult.UpdateResponseInfo(response, context);

                if (context.ResultInspector != null)
                    context.ResultInspector(context.CacheResult);

                Helper.DisposeResponse(response);
            }
            else
            {
                context.CacheResult = CacheResult<TResult>.Empty;
            }
        }

        protected async Task TryCacheResult(CacheContext<TResult> context, TResult result, HttpResponseMessage response)
        {
            context.CacheResult = new CacheResult<TResult>(result, response, context);

            if (context.CacheValidator(context))
            {
                context.ValidationResult = context.ResponseValidator(context);

                if (context.ValidationResult == ResponseValidationResult.OK
                    || context.ValidationResult == ResponseValidationResult.MustRevalidate)
                {
                    await Settings.CacheStore.AddOrUpdate(context);

                    context.VaryByStore.AddOrUpdate(context.Uri, response.Headers.Vary);
                }
            }
            else
            {
                context.CacheStore.TryRemove(context);
            }
        }
    }
}