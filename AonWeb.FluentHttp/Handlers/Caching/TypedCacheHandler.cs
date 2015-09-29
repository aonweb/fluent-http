using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class TypedCacheHandler : CacheHandlerCore, IBoxedHandler
    {
        public TypedCacheHandler() { }

        protected TypedCacheHandler(CacheSettings settings)
            : base(settings) { }

        #region Configuration Methods

        public TypedCacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public TypedCacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public TypedCacheHandler WithDependentUri(Uri uri)
        {
            if (uri == null) 
                return this;

            uri = uri.NormalizeUri();

            if (uri != null && !Settings.DependentUris.Contains(uri))
                Settings.DependentUris.Add(uri);

            return this;
        }

        #endregion

        #region IHttpCallHandler Implementation

        public Task OnSending(TypedSendingContext<object, object> context)
        {
            return TryGetFromCache(context);
        }

        public Task OnSent(TypedSentContext<object> context)
        {
            return TryGetRevalidatedResult(context, context.Response);
        }

        public Task OnResult(TypedResultContext<object> context)
        {
            return TryCacheResult(context, context.Result, context.Response);
        }

        public Task OnError(TypedErrorContext<object> context)
        {
            return ExpireResult(context);
        }

        public Task OnException(TypedExceptionContext context)
        {
            return ExpireResult(context);
        }

        #endregion

        #region Handlers and Events

        Task ITypedHandler.OnSending<TResult, TContent>(TypedSendingContext<TResult, TContent> context)
        {
            var boxedContext = context as TypedSendingContext<object, object> ?? new TypedSendingContext<object, object>(context, context.Request, context.Content, context.HasContent);

            return OnSending(boxedContext);
        }

        Task ITypedHandler.OnSent<TResult>(TypedSentContext<TResult> context)
        {
            var boxedContext = context as TypedSentContext<object> ?? new TypedSentContext<object>(context, context.Request, context.Response);

            return OnSent(boxedContext);
        }

        Task ITypedHandler.OnResult<TResult>(TypedResultContext<TResult> context)
        {
            var boxedContext = context as TypedResultContext<object> ?? new TypedResultContext<object>(context, context.Request, context.Response, context.Result);

            return OnResult(boxedContext);
        }

        Task ITypedHandler.OnError<TError>(TypedErrorContext<TError> context)
        {
            var boxedContext = context as TypedErrorContext<object> ?? new TypedErrorContext<object>(context, context.Request, context.Response, context.Error);

            return OnError(boxedContext);
        }

        public TypedCacheHandler WithHandler<TResult>(ICacheHandler handler)
        {
            Settings.Handler.WithHandler<TResult>(handler);

            return this;
        }

        public TypedCacheHandler WithHandlerConfiguration<THandler>(Action<THandler> configure)
            where THandler : class, ICacheHandler
        {
            Settings.Handler.WithHandlerConfiguration(configure);

            return this;
        }

        public TypedCacheHandler WithOptionalHandlerConfiguration<THandler>(Action<THandler> configure)
            where THandler : class, ICacheHandler
        {
            Settings.Handler.WithHandlerConfiguration(configure, false);

            return this;
        }

        #region Hit

        public TypedCacheHandler OnLookup<TResult>(Action<CacheLookupContext<TResult>> handler)
        {
            Settings.Handler.WithLookupHandler(handler);

            return this;
        }

        public TypedCacheHandler OnLookup<TResult>(HandlerPriority priority, Action<CacheLookupContext<TResult>> handler)
        {
            Settings.Handler.WithLookupHandler(priority, handler);

            return this;
        }

        public TypedCacheHandler OnLookupAsync<TResult>(Func<CacheLookupContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncLookupHandler(handler);

            return this;
        }

        public TypedCacheHandler OnLookupAsync<TResult>(HandlerPriority priority, Func<CacheLookupContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncLookupHandler(priority, handler);

            return this;
        }

        #endregion

        #region Hit

        public TypedCacheHandler OnHit<TResult>(Action<CacheHitContext<TResult>> handler)
        {
            Settings.Handler.WithHitHandler(handler);

            return this;
        }

        public TypedCacheHandler OnHit<TResult>(HandlerPriority priority, Action<CacheHitContext<TResult>> handler)
        {
            Settings.Handler.WithHitHandler(priority, handler);

            return this;
        }

        public TypedCacheHandler OnHitAsync<TResult>(Func<CacheHitContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncHitHandler(handler);

            return this;
        }

        public TypedCacheHandler OnHitAsync<TResult>(HandlerPriority priority, Func<CacheHitContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncHitHandler(priority, handler);

            return this;
        }

        #endregion

        #region Miss

        public TypedCacheHandler OnMiss<TResult>(Action<CacheMissContext<TResult>> handler)
        {
            Settings.Handler.WithMissHandler(handler);

            return this;
        }

        public TypedCacheHandler OnMiss<TResult>(HandlerPriority priority, Action<CacheMissContext<TResult>> handler)
        {
            Settings.Handler.WithMissHandler(priority, handler);

            return this;
        }

        public TypedCacheHandler OnMissAsync<TResult>(Func<CacheMissContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncMissHandler(handler);

            return this;
        }

        public TypedCacheHandler OnMissAsync<TResult>(HandlerPriority priority, Func<CacheMissContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncMissHandler(priority, handler);

            return this;
        }

        #endregion

        #region Store

        public TypedCacheHandler OnStore<TResult>(Action<CacheStoreContext<TResult>> handler)
        {
            Settings.Handler.WithStoreHandler(handler);

            return this;
        }

        public TypedCacheHandler OnStore<TResult>(HandlerPriority priority, Action<CacheStoreContext<TResult>> handler)
        {
            Settings.Handler.WithStoreHandler(priority, handler);

            return this;
        }

        public TypedCacheHandler OnStoreAsync<TResult>(Func<CacheStoreContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncStoreHandler(handler);

            return this;
        }

        public TypedCacheHandler OnStoreAsync<TResult>(HandlerPriority priority, Func<CacheStoreContext<TResult>, Task> handler)
        {
            Settings.Handler.WithAsyncStoreHandler(priority, handler);

            return this;
        }

        #endregion

        #region Expiring

        public TypedCacheHandler OnExpiring(Action<CacheExpiringContext> handler)
        {
            Settings.Handler.WithExpiringHandler(handler);

            return this;
        }

        public TypedCacheHandler OnExpiring(HandlerPriority priority, Action<CacheExpiringContext> handler)
        {
            Settings.Handler.WithExpiringHandler(priority, handler);

            return this;
        }

        public TypedCacheHandler OnExpiringAsync(Func<CacheExpiringContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiringHandler(handler);

            return this;
        }

        public TypedCacheHandler OnExpiringAsync(HandlerPriority priority, Func<CacheExpiringContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiringHandler(priority, handler);

            return this;
        }

        #endregion

        #region Expired

        public TypedCacheHandler OnExpired(Action<CacheExpiredContext> handler)
        {
            Settings.Handler.WithExpiredHandler(handler);

            return this;
        }

        public TypedCacheHandler OnExpired(HandlerPriority priority, Action<CacheExpiredContext> handler)
        {
            Settings.Handler.WithExpiredHandler(priority, handler);

            return this;
        }

        public TypedCacheHandler OnExpiredAsync(Func<CacheExpiredContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiredHandler(handler);

            return this;
        }

        public TypedCacheHandler OnExpiredAsync(HandlerPriority priority, Func<CacheExpiredContext, Task> handler)
        {
            Settings.Handler.WithAsyncExpiredHandler(priority, handler);

            return this;
        }

        #endregion

        #endregion
    }
}