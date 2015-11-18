using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Settings;

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
        private readonly ICacheProvider _cacheProvider;

        protected CacheConfigurationHandlerCore(ICacheSettings settings, ICacheProvider cacheProvider)
        {
            Enabled = true;

            _cacheProvider = cacheProvider;

            Settings = settings;
        }

        public ICacheSettings Settings { get; }

        #region IHandler<HandlerType> Implementation

        public bool Enabled { get; set; }

        public HandlerPriority GetPriority(HandlerType type)
        {
            if (type == HandlerType.Sending)
                return HandlerPriority.First;

            if (type == HandlerType.Sent)
                return HandlerPriority.Last;

            return HandlerPriority.Default;
        }

        #endregion

        protected ICacheContext GetContext(IHandlerContext handlerContext)
        {
            var context = handlerContext.Items["CacheContext"] as ICacheContext;

            if (context == null)
            {
                context = new CacheContext(Settings, handlerContext);
                handlerContext.Items["CacheContext"] = context;
            }

            return context;
        }

        protected async Task TryGetFromCache(IHandlerContextWithResult handlerContext)
        {
            if (!Enabled)
                return;

            var context = GetContext(handlerContext);

            var requestValidation = context.RequestValidator(context);

            if (requestValidation != RequestValidationResult.OK)
            {
                await ExpireResult(context, requestValidation);
                return;
            }

            var result = await _cacheProvider.Get(context);
            var responseValidation = ResponseValidationResult.NotExist;

            if (!result.IsEmpty)
            {
                context.ResultInspector?.Invoke(result);
                responseValidation = context.ResponseValidator(context, result.Metadata);
            }

            // TODO: Determine the need for conditional put - if so, that logic would go here
            if (responseValidation == ResponseValidationResult.Stale && !context.AllowStaleResultValidator(context, result.Metadata))
                responseValidation = ResponseValidationResult.MustRevalidate;

            if (responseValidation == ResponseValidationResult.MustRevalidate && context.Request != null)
            {
                if (result.Metadata.ETag != null)
                    context.Request.Headers.Add("If-None-Match", result.Metadata.ETag);
                else if (result.Metadata.LastModified != null)
                    context.Request.Headers.Add("If-Modified-Since", result.Metadata.LastModified.Value.ToString("r"));

                //hang on to this we are going to need it in a sec. We could try to get it from cache store, but it may have expired by then
                context.Items["CacheHandlerCachedItem"] = result;

                return;
            }

            if (result.IsEmpty)
            {
                var missedResult = await context.HandlerRegister.OnMiss(context);

                if (missedResult.IsDirty)
                {
                    result = new CacheEntry(missedResult.Value, null);
                }
            }

            if (!result.IsEmpty)
            {
                var hitResult = await context.HandlerRegister.OnHit(context, result.Value);

                if (!hitResult.IsDirty || !(bool)hitResult.Value)
                    handlerContext.Result = result.Value;
            }

            context.Items["CacheHandlerCachedItem"] = result;
        }

        protected async Task TryGetRevalidatedResult(IHandlerContextWithResult handlerContext, HttpRequestMessage request, HttpResponseMessage response)
        {
            if (!Enabled)
                return;

            var context = GetContext(handlerContext);

            var result = (CacheEntry)context.Items["CacheHandlerCachedItem"];

            if (result == null || !context.RevalidateValidator(context, result.Metadata))
                return;

            if (!result.IsEmpty && result.Value != null)
            {
                result.UpdateResponseInfo(request, response, context);

                context.ResultInspector?.Invoke(result);

                ObjectHelpers.Dispose(response);
            }
            else
            {
                result = CacheEntry.Empty;
            }

            if (result.IsEmpty)
            {
                var missedResult = await context.HandlerRegister.OnMiss(context);

                if (missedResult.IsDirty)
                {
                    result = new CacheEntry(missedResult.Value, null);
                }
            }

            if (!result.IsEmpty)
            {
                var hitResult = await context.HandlerRegister.OnHit(context, result.Value);

                if (hitResult.IsDirty && !(bool)hitResult.Value)
                    handlerContext.Result = result.Value;
            }

            context.Items["CacheHandlerCachedItem"] = result;
        }

        protected async Task TryCacheResult(IHandlerContext handlerContext, object result, HttpRequestMessage request, HttpResponseMessage response)
        {
            if (!Enabled)
                return;

            var context = GetContext(handlerContext);

            var cacheEntry = new CacheEntry(result, request, response, context);

            var requestValidation = context.RequestValidator(context);

            if (requestValidation == RequestValidationResult.OK)
            {
                var validationResult = context.ResponseValidator(context, cacheEntry.Metadata);

                if (validationResult == ResponseValidationResult.OK
                    || validationResult == ResponseValidationResult.MustRevalidate)
                {
                    await _cacheProvider.Put(context, cacheEntry);
                }

                await context.HandlerRegister.OnStore(context, cacheEntry.Value);
            }
            else
            {
                await ExpireResult(context, requestValidation);
            }
        }

        protected Task ExpireResult(IHandlerContext handlerContext, RequestValidationResult reason)
        {
            var context = GetContext(handlerContext);

            return ExpireResult(context, reason);
        }

        private async Task ExpireResult(ICacheContext context, RequestValidationResult reason)
        {
            if (!Enabled)
                return;

            if ((context.Items["CacheHandler_ItemExpired"] as bool?).GetValueOrDefault())
                return;

            var expiringResult = await context.HandlerRegister.OnExpiring(context, reason);

            var uris = _cacheProvider.Remove(context, expiringResult.Value as IEnumerable<Uri>).ToList();

            await context.HandlerRegister.OnExpired(context, reason, uris);

            context.Items["CacheHandler_ItemExpired"] = true;
        }
    }
}