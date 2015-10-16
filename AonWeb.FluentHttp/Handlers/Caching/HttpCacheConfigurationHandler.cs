using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class HttpCacheConfigurationHandler : CacheConfigurationHandlerCore, IHttpHandler
    {
        public HttpCacheConfigurationHandler(ICacheSettings settings)
            : base(settings)
        {
            var handlers = Defaults.Current.GetCachingDefaults().HttpHandlers.GetHandlers(settings);

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    WithHandler(handler);
                }
            }
        }

        #region Configuration Methods

        public HttpCacheConfigurationHandler WithCaching(bool enabled = true)
        {
            Enabled = enabled;

            return this;
        }

        public HttpCacheConfigurationHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public HttpCacheConfigurationHandler WithDependentUri(Uri uri)
        {
            if (uri == null)
                return this;

            uri = uri.NormalizeUri();

            if (uri != null && !Settings.DependentUris.Contains(uri))
                Settings.DependentUris.Add(uri);

            return this;
        }

        public HttpCacheConfigurationHandler WithCacheDuration(TimeSpan? duration)
        {
            Settings.CacheDuration = duration;

            return this;
        }

        public HttpCacheConfigurationHandler WithHandler(IHttpCacheHandler handler)
        {
            Settings.Handler.WithHandler(handler);

            return this;
        }

        public HttpCacheConfigurationHandler WithHandlerConfiguration<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCacheHandler
        {
            Settings.Handler.WithHandlerConfiguration(configure);

            return this;
        }

        public HttpCacheConfigurationHandler WithOptionalHandlerConfiguration<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCacheHandler
        {
            Settings.Handler.WithHandlerConfiguration(configure, false);

            return this;
        }

        #endregion

        #region IHttpHandler Implementation

        async Task IHttpHandler.OnSending(HttpSendingContext context)
        {
            Settings.ResultInspector = cacheResult =>
            {
                ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Request;
            };

            await TryGetFromCache(context);
        }

        async Task IHttpHandler.OnSent(HttpSentContext context)
        {

            Settings.ResultInspector = cacheResult => ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Result.RequestMessage;

            await TryGetRevalidatedResult(context, context.Request, context.Result);

            await TryCacheResult(context, context.Result, context.Request, context.Result);
        }

        Task IHttpHandler.OnException(HttpExceptionContext context)
        {
            return ExpireResult(context);
        }

        #endregion
    }
}
