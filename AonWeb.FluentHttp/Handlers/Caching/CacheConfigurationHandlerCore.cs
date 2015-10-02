using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    // this class is inspired by the excellent work in CacheCow Caching libraries - https://github.com/aliostad/CacheCow
    // for whatever reason, I couldn't get the inmemory cache in the HttpClientHandler / HttpCacheConfigurationHandler 
    // to play nice with call / client builders, and the cache kept disappearing
    // additionally I need an implementation that allows for deserialized object level caching in addition to http response caching
    // so I implemented a cachehandler that plugs in to the TypedBuilder higher up,
    // but the base logic for cache validation / invalidation was based off CacheCow
    public abstract class CacheConfigurationHandlerCore
    {
        protected CacheConfigurationHandlerCore(ICacheSettings settings)
        {
            Settings = settings;
        }

        protected ICacheSettings Settings { get; set; }

        #region IHandler<HandlerType> Implementation

        public bool Enabled
        {
            get { return Settings.Enabled; }
            protected set { Settings.Enabled = value;}
        } 

        public HandlerPriority GetPriority(HandlerType type)
        {
            if (type == HandlerType.Sending)
                return HandlerPriority.First;

            if (type == HandlerType.Sent)
                return HandlerPriority.Last;

            return HandlerPriority.Default;
        }

        #endregion

        protected ICacheContext CreateCacheContext(IHandlerContext handlerContext)
        {
            return new CacheContext(Settings, handlerContext);
        }

        protected async Task TryGetFromCache(IHandlerContextWithResult handlerContext)
        {
            var context = CreateCacheContext(handlerContext);

            if (!context.CacheValidator(context))
            {
                await ExpireResult(context);
                return ;
            }

            context.Result = await Cache.CurrentCacheStore.GetCachedResult(context);

            if (context.Result.Found)
            {
                context.ResultInspector?.Invoke(context.Result);
                context.ValidationResult = context.ResponseValidator(context, context.Result.ResponseInfo);
            }

            // TODO: Determine the need for conditional put - if so, that logic would go here


            if (context.ValidationResult == ResponseValidationResult.Stale && !context.AllowStaleResultValidator(context, context.Result.ResponseInfo))
                context.ValidationResult = ResponseValidationResult.MustRevalidate;

            if (context.ValidationResult == ResponseValidationResult.MustRevalidate && context.Request != null)
            {
                if (context.Result.ResponseInfo.ETag != null)
                    context.Request.Headers.Add("If-None-Match", context.Result.ResponseInfo.ETag.ToString());
                else if (context.Result.ResponseInfo.LastModified != null)
                    context.Request.Headers.Add("If-Modified-Since", context.Result.ResponseInfo.LastModified.Value.ToString("r"));

                //hang on to this we are going to need it in a sec. We could try to get it from cache store, but it may have expired by then
                context.Items["CacheHandlerCachedItem"] = context.Result;

                return;
            }

            if (!context.Result.Found)
            {
                var missedResult = await context.Handler.OnMiss(context);

                if (missedResult.IsDirty)
                {
                    context.Result = new CacheResult(missedResult.Value, null);
                    context.ValidationResult = ResponseValidationResult.OK;
                }
            }

            if (context.Result.Found)
            {
                var hitResult = await context.Handler.OnHit(context, context.Result.Result);

                if (!hitResult.IsDirty || !(bool)hitResult.Value)
                    handlerContext.Result = context.Result.Result;
            }
        }

        protected async Task TryGetRevalidatedResult(IHandlerContextWithResult handlerContext, HttpRequestMessage request, HttpResponseMessage response)
        {
            var context = CreateCacheContext(handlerContext);

            if (!context.RevalidateValidator(context, context.Result.ResponseInfo))
                return;

            context.Result = (CacheResult)context.Items["CacheHandlerCachedItem"];

            if (context.Result.Found && context.Result.Result != null)
            {
                context.Result.UpdateResponseInfo(request, response, context);

                context.ResultInspector?.Invoke(context.Result);

                ObjectHelpers.Dispose(response);
            }
            else
            {
                context.Result = CacheResult.Empty;
            }

            if (!context.Result.Found)
            {
                var missedResult = await context.Handler.OnMiss(context);

                if (missedResult.IsDirty)
                {
                    context.Result = new CacheResult(missedResult.Value, null);
                    context.ValidationResult = ResponseValidationResult.OK;
                }
            }

            if (context.Result.Found)
            {
                var hitResult = await context.Handler.OnHit(context, context.Result.Result);

                if (hitResult.IsDirty && !(bool)hitResult.Value)
                    handlerContext.Result = context.Result.Result;
            }
        }

        protected async Task TryCacheResult(IHandlerContext handlerContext, object result, HttpRequestMessage request, HttpResponseMessage response)
        {
            var context = CreateCacheContext(handlerContext);

            context.Result = new CacheResult(result, request, response, context);

            if (context.CacheValidator(context))
            {
                context.ValidationResult = context.ResponseValidator(context, context.Result.ResponseInfo);

                if (context.ValidationResult == ResponseValidationResult.OK
                    || context.ValidationResult == ResponseValidationResult.MustRevalidate)
                {
                    await Cache.CurrentCacheStore.AddOrUpdate(context);

                    Cache.CurrentVaryByStore.AddOrUpdate(context.Uri, response.Headers.Vary);
                }

                await context.Handler.OnStore(context, context.Result.Result);
            }
            else
            {
                await ExpireResult(context);
            }
        }

        protected Task ExpireResult(IHandlerContext handlerContext)
        {
            var context = CreateCacheContext(handlerContext);

            return ExpireResult(context);
        }

        private static async Task ExpireResult(ICacheContext context)
        {
            if ((context.Items["CacheHandler_ItemExpired"] as bool?).GetValueOrDefault())
                return;

            var expiringResult = await context.Handler.OnExpiring(context);

            var uris = Cache.CurrentCacheStore.TryRemove(context, expiringResult.Value as IEnumerable<Uri>).ToList();

            await context.Handler.OnExpired(context, uris);

            context.Items["CacheHandler_ItemExpired"] = true;
        }
    }
}