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
            _cacheProvider = cacheProvider;

            Settings = settings;
        }

        public ICacheSettings Settings { get; private set; }

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
            var context = GetContext(handlerContext);

            if (!context.RequestValidator(context))
            {
                await ExpireResult(context);
                return ;
            }

            var result = await _cacheProvider.Get(context);
            var validationResult = ResponseValidationResult.NotExist;

            if (!result.IsEmpty)
            {
                context.ResultInspector?.Invoke(result);
                validationResult = context.ResponseValidator(context, result.Metadata);
            }

            // TODO: Determine the need for conditional put - if so, that logic would go here
            if (validationResult == ResponseValidationResult.Stale && !context.AllowStaleResultValidator(context, result.Metadata))
                validationResult = ResponseValidationResult.MustRevalidate;

            if (validationResult == ResponseValidationResult.MustRevalidate && context.Request != null)
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
            var context = GetContext(handlerContext);

            var cacheEntry = new CacheEntry(result, request, response, context);

            if (context.RequestValidator(context))
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
                await ExpireResult(context);
            }
        }

        protected Task ExpireResult(IHandlerContext handlerContext)
        {
            var context = GetContext(handlerContext);

            return ExpireResult(context);
        }

        private async Task ExpireResult(ICacheContext context)
        {
            if ((context.Items["CacheHandler_ItemExpired"] as bool?).GetValueOrDefault())
                return;

            var expiringResult = await context.HandlerRegister.OnExpiring(context);

            var uris = _cacheProvider.Remove(context, expiringResult.Value as IEnumerable<Uri>).ToList();

            await context.HandlerRegister.OnExpired(context, uris);

            context.Items["CacheHandler_ItemExpired"] = true;
        }

        public void WithSettings(ICacheSettings settings)
        {
            Settings = settings;
        }
    }
}