using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class HttpCacheConfigurationHandler : CacheConfigurationHandlerCore, IHttpHandler, IAdvancedCacheConfigurable<HttpCacheConfigurationHandler>
    {
        public HttpCacheConfigurationHandler(ICacheSettings settings, ICacheProvider cacheProvider, IEnumerable<IHttpCacheHandler> handlers)
            : base(settings, cacheProvider)
        {
            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    Settings.HandlerRegister.WithHandler(handler);
                }
            }
        }

        #region IFluentConfigurable implementation

        public HttpCacheConfigurationHandler WithConfiguration(Action<ICacheSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        void IConfigurable<ICacheSettings>.WithConfiguration(Action<ICacheSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        #endregion

        #region IHttpHandler Implementation

        async Task IHttpHandler.OnSending(HttpSendingContext context)
        {
            Settings.ResultInspector = cacheEntry =>
            {
                ((HttpResponseMessage)cacheEntry.Value).RequestMessage = context.Request;
            };

            await TryGetFromCache(context);
        }

        async Task IHttpHandler.OnSent(HttpSentContext context)
        {

            Settings.ResultInspector = cacheEntry => ((HttpResponseMessage)cacheEntry.Value).RequestMessage = context.Result.RequestMessage;

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
