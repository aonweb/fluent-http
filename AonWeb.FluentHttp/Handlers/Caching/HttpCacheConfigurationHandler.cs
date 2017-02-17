using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class HttpCacheConfigurationHandler : CacheConfigurationHandlerCore, IHttpHandler, IAdvancedCacheConfigurable<HttpCacheConfigurationHandler>
    {
        public HttpCacheConfigurationHandler(ICacheSettings settings, ICacheManager cacheManager, IEnumerable<IHttpCacheHandler> handlers)
            : base(settings, cacheManager)
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

            var response = context.Result;

            await TryGetRevalidatedResult(context, context.Request, context.Result);

            // if we just retrieved a response from cache
            // dispose of the revalidation result
            if (context.Result != response)
                ObjectHelpers.Dispose(response);

            await TryCacheResult(context, context.Result, context.Request, context.Result);
        }

        Task IHttpHandler.OnException(HttpExceptionContext context)
        {
            return ExpireResult(context, RequestValidationResult.ErrorOrException);
        }

        #endregion
    }
}
