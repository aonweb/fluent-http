using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    // this class is inspired by the excellent work in CacheCow Caching libraries - https://github.com/aliostad/CacheCow
    // for whatever reason, I couldn't get the inmemory cache in the HttpClientHandler / HttpCallCacheHandler 
    // to play nice with call / client builders, and the cache kept disappearing
    // additionally I need an implementation that allows for deserialized object level caching in addition to http response caching
    // so I implemented a cachehandler that plugs in to the TypedHttpCallBuilder higher up,
    // but the base logic for cache validation / invalidation was based off CacheCow
    public abstract class CacheHandlerBase
    {
        protected CacheHandlerBase()
            : this(new CacheSettings()) { }

        protected CacheHandlerBase(CacheSettings settings)
        {
            Settings = settings;
        }

        protected CacheSettings Settings { get; set; }

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

        protected CacheContext CreateCacheContext(IHttpCallHandlerContext callContext)
        {
            return new CacheContext(Settings, callContext);
        }

        protected async Task TryGetFromCache<TResult>(IHttpCallHandlerContextWithResult<TResult> callContext)
        {
            var context = CreateCacheContext(callContext);

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


            if (context.ValidationResult == ResponseValidationResult.Stale && !context.AllowStaleResultValidator(context))
                context.ValidationResult = ResponseValidationResult.MustRevalidate;

            if (context.ValidationResult == ResponseValidationResult.MustRevalidate && context.Request != null)
            {
                if (context.CacheResult.ResponseInfo.ETag != null)
                    context.Request.Headers.Add("If-None-Match", context.CacheResult.ResponseInfo.ETag.ToString());
                else if (context.CacheResult.ResponseInfo.LastModified != null)
                    context.Request.Headers.Add("If-Modified-Since", context.CacheResult.ResponseInfo.LastModified.Value.ToString("r"));

                //hang on to this we are going to need it in a sec. We could try to get it from cache store, but it may have expired by then
                context.Items["CacheHandlerCachedItem"] = context.CacheResult;
            }
            else
            {
                if (!context.ResultFound)
                {
                    var missedResult = await context.Handler.OnMiss(context);

                    if (missedResult.Modified)
                    {
                        context.CacheResult = new CacheResult(missedResult.Value, null);
                        context.ValidationResult = ResponseValidationResult.OK;
                    }
                }

                if (context.ResultFound)
                {
                    var hitResult = await context.Handler.OnHit(context, context.Result);

                    if (!hitResult.Modified || !(bool)hitResult.Value)
                        callContext.Result = (TResult)context.Result;
                } 
            } 
        }

        protected async Task TryGetRevalidatedResult<TResult>(IHttpCallHandlerContextWithResult<TResult> callContext, HttpResponseMessage response)
        {
            var context = CreateCacheContext(callContext);

            if (!context.RevalidateValidator(context))
                return;

            context.CacheResult = context.Items["CacheHandlerCachedItem"] as CacheResult;

            if (context.CacheResult != null && context.ResultFound)
            {
                context.CacheResult.UpdateResponseInfo(response, context);

                if (context.ResultInspector != null)
                    context.ResultInspector(context.CacheResult);

                Helper.DisposeResponse(response);
            }
            else
            {
                context.CacheResult = CacheResult.Empty;
            }

            if (!context.ResultFound)
            {
                var missedResult = await context.Handler.OnMiss(context);

                if (missedResult.Modified)
                {
                    context.CacheResult = new CacheResult(missedResult.Value, null);
                    context.ValidationResult = ResponseValidationResult.OK;
                }
            }

            if (context.ResultFound)
            {
                var hitResult = await context.Handler.OnHit(context, context.Result);

                if (hitResult.Modified && !(bool)hitResult.Value)
                    callContext.Result = (TResult)context.Result;
            }
        }

        protected async Task TryCacheResult(IHttpCallHandlerContext callContext, object result, HttpResponseMessage response)
        {
            var context = CreateCacheContext(callContext);

            context.CacheResult = new CacheResult(result, response, context);

            if (context.CacheValidator(context))
            {
                context.ValidationResult = context.ResponseValidator(context);

                if (context.ValidationResult == ResponseValidationResult.OK
                    || context.ValidationResult == ResponseValidationResult.MustRevalidate)
                {
                    await Settings.CacheStore.AddOrUpdate(context);

                    context.VaryByStore.AddOrUpdate(context.Uri, response.Headers.Vary);
                }

                await context.Handler.OnStore(context, context.Result);
            }
            else
            {
                await ExpireResult(context);
            }
        }

        protected Task ExpireResult(IHttpCallHandlerContext callContext)
        {
            var context = CreateCacheContext(callContext);

            return ExpireResult(context);
        }

        private static async Task ExpireResult(CacheContext context)
        {
            if ((context.Items["CacheHandler_ItemExpired"] as bool?).GetValueOrDefault())
                return;

            var expiringResult = await context.Handler.OnExpiring(context);

            var keys = context.CacheStore.TryRemove(context, expiringResult.Value as IEnumerable<Uri>).ToList();

            await context.Handler.OnExpired(context, keys);

            context.Items["CacheHandler_ItemExpired"] = true;
        }
    }
}