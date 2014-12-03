using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class TypedHttpCallCacheHandler : CacheHandlerBase, IBoxedHttpCallHandler
    {
        public TypedHttpCallCacheHandler() { }

        protected TypedHttpCallCacheHandler(CacheSettings settings)
            : base(settings) { }

        #region Configuration Methods

        public TypedHttpCallCacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public TypedHttpCallCacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public TypedHttpCallCacheHandler WithDependentUri(Uri uri)
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

        public Task OnSending(TypedHttpSendingContext<object, object> context)
        {
            return TryGetFromCache(context);
        }

        public Task OnSent(TypedHttpSentContext<object> context)
        {
            return TryGetRevalidatedResult(context, context.Response);
        }

        public Task OnResult(TypedHttpResultContext<object> context)
        {
            return TryCacheResult(context, context.Result, context.Response);
        }

        public Task OnError(TypedHttpErrorContext<object> context)
        {
            return ExpireResult(context);
        }

        public Task OnException(TypedHttpExceptionContext context)
        {
            return ExpireResult(context);
        }

        #endregion

        #region Handlers and Events

        Task ITypedHttpCallHandler.OnSending<TResult, TContent>(TypedHttpSendingContext<TResult, TContent> context)
        {
            var boxedContext = context as TypedHttpSendingContext<object, object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpSendingContext<object, object>(context, context.Request, context.Content, context.HasContent);

            return OnSending(boxedContext);
        }

        Task ITypedHttpCallHandler.OnSent<TResult>(TypedHttpSentContext<TResult> context)
        {
            var boxedContext = context as TypedHttpSentContext<object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpSentContext<object>(context, context.Response);

            return OnSent(boxedContext);
        }

        Task ITypedHttpCallHandler.OnResult<TResult>(TypedHttpResultContext<TResult> context)
        {
            var boxedContext = context as TypedHttpResultContext<object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpResultContext<object>(context, context.Response, context.Result);

            return OnResult(boxedContext);
        }

        Task ITypedHttpCallHandler.OnError<TError>(TypedHttpErrorContext<TError> context)
        {
            var boxedContext = context as TypedHttpErrorContext<object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpErrorContext<object>(context, context.Response, context.Error);

            return OnError(boxedContext);
        }

        public TypedHttpCallCacheHandler WithHandler<TResult>(ICacheHandler handler)
        {
            Settings.Handler.AddHandler<TResult>(handler);

            return this;
        }

        public TypedHttpCallCacheHandler ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ICacheHandler
        {
            Settings.Handler.ConfigureHandler(configure);

            return this;
        }

        public TypedHttpCallCacheHandler TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ICacheHandler
        {
            Settings.Handler.ConfigureHandler(configure, false);

            return this;
        }

        #region Hit

        public TypedHttpCallCacheHandler OnHit<TResult>(Action<CacheHitContext<TResult>> handler)
        {
            Settings.Handler.AddHitHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnHit<TResult>(HttpCallHandlerPriority priority, Action<CacheHitContext<TResult>> handler)
        {
            Settings.Handler.AddHitHandler(priority, handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnHitAsync<TResult>(Func<CacheHitContext<TResult>, Task> handler)
        {
            Settings.Handler.AddAsyncHitHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnHitAsync<TResult>(HttpCallHandlerPriority priority, Func<CacheHitContext<TResult>, Task> handler)
        {
            Settings.Handler.AddAsyncHitHandler(priority, handler);

            return this;
        }

        #endregion

        #region Miss

        public TypedHttpCallCacheHandler OnMiss<TResult>(Action<CacheMissContext<TResult>> handler)
        {
            Settings.Handler.AddMissHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnMiss<TResult>(HttpCallHandlerPriority priority, Action<CacheMissContext<TResult>> handler)
        {
            Settings.Handler.AddMissHandler(priority, handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnMissAsync<TResult>(Func<CacheMissContext<TResult>, Task> handler)
        {
            Settings.Handler.AddAsyncMissHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnMissAsync<TResult>(HttpCallHandlerPriority priority, Func<CacheMissContext<TResult>, Task> handler)
        {
            Settings.Handler.AddAsyncMissHandler(priority, handler);

            return this;
        }

        #endregion

        #region Store

        public TypedHttpCallCacheHandler OnStore<TResult>(Action<CacheStoreContext<TResult>> handler)
        {
            Settings.Handler.AddStoreHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnStore<TResult>(HttpCallHandlerPriority priority, Action<CacheStoreContext<TResult>> handler)
        {
            Settings.Handler.AddStoreHandler(priority, handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnStoreAsync<TResult>(Func<CacheStoreContext<TResult>, Task> handler)
        {
            Settings.Handler.AddAsyncStoreHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnStoreAsync<TResult>(HttpCallHandlerPriority priority, Func<CacheStoreContext<TResult>, Task> handler)
        {
            Settings.Handler.AddAsyncStoreHandler(priority, handler);

            return this;
        }

        #endregion

        #region Expiring

        public TypedHttpCallCacheHandler OnExpiring(Action<CacheExpiringContext> handler)
        {
            Settings.Handler.AddExpiringHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnExpiring(HttpCallHandlerPriority priority, Action<CacheExpiringContext> handler)
        {
            Settings.Handler.AddExpiringHandler(priority, handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnExpiringAsync(Func<CacheExpiringContext, Task> handler)
        {
            Settings.Handler.AddAsyncExpiringHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnExpiringAsync(HttpCallHandlerPriority priority, Func<CacheExpiringContext, Task> handler)
        {
            Settings.Handler.AddAsyncExpiringHandler(priority, handler);

            return this;
        }

        #endregion

        #region Expired

        public TypedHttpCallCacheHandler OnExpired(Action<CacheExpiredContext> handler)
        {
            Settings.Handler.AddExpiredHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnExpired(HttpCallHandlerPriority priority, Action<CacheExpiredContext> handler)
        {
            Settings.Handler.AddExpiredHandler(priority, handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnExpiredAsync(Func<CacheExpiredContext, Task> handler)
        {
            Settings.Handler.AddAsyncExpiredHandler(handler);

            return this;
        }

        public TypedHttpCallCacheHandler OnExpiredAsync(HttpCallHandlerPriority priority, Func<CacheExpiredContext, Task> handler)
        {
            Settings.Handler.AddAsyncExpiredHandler(priority, handler);

            return this;
        }

        #endregion

        #endregion
    }
}