using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class TypedCacheConfigurationHandler : CacheConfigurationHandlerCore, ITypedHandler
    {
        public TypedCacheConfigurationHandler()
            : this(new CacheSettings()) { }

        protected TypedCacheConfigurationHandler(ICacheSettings settings)
            : base(settings)
        {
            var handlers = Defaults.Caching.TypedHandlerFactory?.Invoke();

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    WithHandler(handler);
                }
            }
        }

        #region Configuration Methods

        public TypedCacheConfigurationHandler WithCaching(bool enabled = true)
        {
            Enabled = enabled;

            return this;
        }

        public TypedCacheConfigurationHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public TypedCacheConfigurationHandler WithDependentUri(Uri uri)
        {
            if (uri == null) 
                return this;

            uri = uri.NormalizeUri();

            if (uri != null && !Settings.DependentUris.Contains(uri))
                Settings.DependentUris.Add(uri);

            return this;
        }

        public TypedCacheConfigurationHandler WithCacheDuration(TimeSpan? duration)
        {
            Settings.CacheDuration = duration;

            return this;
        }

        public TypedCacheConfigurationHandler WithHandler(ITypedCacheHandler handler)
        {
            Settings.Handler.WithHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler WithHandlerConfiguration<THandler>(Action<THandler> configure)
            where THandler : class, ITypedCacheHandler
        {
            Settings.Handler.WithHandlerConfiguration(configure);

            return this;
        }

        public TypedCacheConfigurationHandler WithOptionalHandlerConfiguration<THandler>(Action<THandler> configure)
            where THandler : class, ITypedCacheHandler
        {
            Settings.Handler.WithHandlerConfiguration(configure, false);

            return this;
        }

        #endregion

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

        #region Events

        #region Hit

        public TypedCacheConfigurationHandler OnHit<TResult>(Action<CacheHitContext<TResult>> handler)
        {
            Settings.Handler.WithHitHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnHit<TResult>(HandlerPriority priority, Action<CacheHitContext<TResult>> handler)
        {
            Settings.Handler.WithHitHandler(priority, handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnHitAsync<TResult>(Func<CacheHitContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncHitHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnHitAsync<TResult>(HandlerPriority priority, Func<CacheHitContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncHitHandler(priority, handler);

            return this;
        }

        #endregion

        #region Miss

        public TypedCacheConfigurationHandler OnMiss<TResult>(Action<CacheMissContext<TResult>> handler)
        {
            Settings.Handler.WithMissHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnMiss<TResult>(HandlerPriority priority, Action<CacheMissContext<TResult>> handler)
        {
            Settings.Handler.WithMissHandler(priority, handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnMissAsync<TResult>(Func<CacheMissContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncMissHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnMissAsync<TResult>(HandlerPriority priority, Func<CacheMissContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncMissHandler(priority, handler);

            return this;
        }

        #endregion

        #region Store

        public TypedCacheConfigurationHandler OnStore<TResult>(Action<CacheStoreContext<TResult>> handler)
        {
            Settings.Handler.WithStoreHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnStore<TResult>(HandlerPriority priority, Action<CacheStoreContext<TResult>> handler)
        {
            Settings.Handler.WithStoreHandler(priority, handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnStoreAsync<TResult>(Func<CacheStoreContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncStoreHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnStoreAsync<TResult>(HandlerPriority priority, Func<CacheStoreContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncStoreHandler(priority, handler);

            return this;
        }

        #endregion

        #region Expiring

        public TypedCacheConfigurationHandler OnExpiring(Action<CacheExpiringContext> handler)
        {
            Settings.Handler.WithExpiringHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnExpiring(HandlerPriority priority, Action<CacheExpiringContext> handler)
        {
            Settings.Handler.WithExpiringHandler(priority, handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnExpiringAsync(Func<CacheExpiringContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiringHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnExpiringAsync(HandlerPriority priority, Func<CacheExpiringContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiringHandler(priority, handler);

            return this;
        }

        #endregion

        #region Expired

        public TypedCacheConfigurationHandler OnExpired(Action<CacheExpiredContext> handler)
        {
            Settings.Handler.WithExpiredHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnExpired(HandlerPriority priority, Action<CacheExpiredContext> handler)
        {
            Settings.Handler.WithExpiredHandler(priority, handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnExpiredAsync(Func<CacheExpiredContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiredHandler(handler);

            return this;
        }

        public TypedCacheConfigurationHandler OnExpiredAsync(HandlerPriority priority, Func<CacheExpiredContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiredHandler(priority, handler);

            return this;
        }

        #endregion

        #endregion
    }
}