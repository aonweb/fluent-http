using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class TypedCacheConfigurationHandler : CacheConfigurationHandlerCore, ITypedHandler, IAdvancedCacheConfigurable<TypedCacheConfigurationHandler>
    {
        public TypedCacheConfigurationHandler(ICacheSettings settings, ICacheProvider cacheProvider, IEnumerable<ITypedCacheHandler> handlers)
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

        #region ITypedHandler implementation

        Task ITypedHandler.OnSending(TypedSendingContext context)
        {

            return TryGetFromCache(context);
        }

        Task ITypedHandler.OnSent(TypedSentContext context)
        {
            return TryGetRevalidatedResult(context, context.Request, context.Response);
        }

        Task ITypedHandler.OnResult(TypedResultContext context)
        {
            return TryCacheResult(context, context.Result, context.Request, context.Response);
        }

        Task ITypedHandler.OnError(TypedErrorContext context)
        {
            return ExpireResult(context);
        }

        Task ITypedHandler.OnException(TypedExceptionContext context)
        {
            return ExpireResult(context);
        }

        #endregion

        #region IFluentConfigurable implementation

        public TypedCacheConfigurationHandler WithConfiguration(Action<ICacheSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        void IConfigurable<ICacheSettings>.WithConfiguration(Action<ICacheSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        #endregion
       
    }
}