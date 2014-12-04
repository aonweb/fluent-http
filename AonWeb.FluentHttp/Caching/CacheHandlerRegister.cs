using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheHandlerRegister
    {
        private readonly ISet<ICacheHandler> _cacheHandlers;

        private readonly IDictionary<CacheHandlerType, IList<KeyValuePair<HttpCallHandlerPriority, Delegate>>> _handlers;

        public CacheHandlerRegister()
        {
            _cacheHandlers = new HashSet<ICacheHandler>();

            _handlers = new Dictionary<CacheHandlerType, IList<KeyValuePair<HttpCallHandlerPriority, Delegate>>>();

            foreach (var callType in Enum.GetValues(typeof(CacheHandlerType)).Cast<CacheHandlerType>())
            {
                _handlers[callType] = new List<KeyValuePair<HttpCallHandlerPriority, Delegate>>();
            }
        }

        private void AddHandler(CacheHandlerType callType, HttpCallHandlerPriority priority, Delegate handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _handlers[callType].Add(new KeyValuePair<HttpCallHandlerPriority, Delegate>(priority, handler));
        }

        public CacheHandlerRegister AddHandler<TResult>(ICacheHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (_cacheHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _cacheHandlers.Add(handler);

            AddAsyncHitHandler<TResult>(handler.GetPriority(CacheHandlerType.Hit), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnHit(ctx);
            });

            AddAsyncMissHandler<TResult>(handler.GetPriority(CacheHandlerType.Miss), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnMiss(ctx);
            });

            AddAsyncStoreHandler<TResult>(handler.GetPriority(CacheHandlerType.Store), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnStore(ctx);
            });

            AddAsyncExpiringHandler(handler.GetPriority(CacheHandlerType.Expiring), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnExpiring(ctx);
            });

            AddAsyncExpiredHandler(handler.GetPriority(CacheHandlerType.Expired), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnExpired(ctx);
            });

            return this;
        }

        public CacheHandlerRegister ConfigureHandler<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, ICacheHandler
        {
            if (configure == null)
                throw new ArgumentNullException("configure");

            var handler = _cacheHandlers.OfType<THandler>().FirstOrDefault();

            if (handler == null)
            {
                if (throwOnNotFound)
                    throw new KeyNotFoundException(string.Format(SR.HanderDoesNotExistErrorFormat, typeof(THandler).Name));
            }
            else
            {
                configure(handler);
            }

            return this;
        }

        public async Task<ModifyTracker> OnHit(CacheContext context, object result)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, CacheHandlerType.Hit, typeof(CacheHitContext<>), new[] { context.ResultType }, new[] { context, result }, false);
        }

        public async Task<ModifyTracker> OnMiss(CacheContext context)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, CacheHandlerType.Miss, typeof(CacheMissContext<>), new[] { context.ResultType }, new object [] { context }, false);
        }

        public async Task<ModifyTracker> OnStore(CacheContext context, object result)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, CacheHandlerType.Store, typeof(CacheStoreContext<>), new[] { context.ResultType }, new[] { context, result }, false);
        }

        public async Task<ModifyTracker> OnExpiring(CacheContext context)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, CacheHandlerType.Expiring, typeof(CacheExpiringContext), null, new object[] { context }, false);
        }

        public async Task<ModifyTracker> OnExpired(CacheContext context, IEnumerable<string> relatedKeys)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, CacheHandlerType.Expired, typeof(CacheExpiredContext), null, new object[] { context, relatedKeys }, false);
        }

        #region Hit

        public CacheHandlerRegister AddHitHandler<TResult>(Action<CacheHitContext<TResult>> handler)
        {
            return AddHitHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddHitHandler<TResult>(HttpCallHandlerPriority priority, Action<CacheHitContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncHitHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public CacheHandlerRegister AddAsyncHitHandler<TResult>(Func<CacheHitContext<TResult>, Task> handler)
        {
            return AddAsyncHitHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddAsyncHitHandler<TResult>(HttpCallHandlerPriority priority, Func<CacheHitContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(CacheHandlerType.Hit, priority, handler);

            return this;
        }

        #endregion

        #region Miss

        public CacheHandlerRegister AddMissHandler<TResult>(Action<CacheMissContext<TResult>> handler)
        {
            return AddMissHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddMissHandler<TResult>(HttpCallHandlerPriority priority, Action<CacheMissContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncMissHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public CacheHandlerRegister AddAsyncMissHandler<TResult>(Func<CacheMissContext<TResult>, Task> handler)
        {
            return AddAsyncMissHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddAsyncMissHandler<TResult>(HttpCallHandlerPriority priority, Func<CacheMissContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(CacheHandlerType.Miss, priority, handler);

            return this;
        }

        #endregion

        #region Store

        public CacheHandlerRegister AddStoreHandler<TResult>(Action<CacheStoreContext<TResult>> handler)
        {
            return AddStoreHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddStoreHandler<TResult>(HttpCallHandlerPriority priority, Action<CacheStoreContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncStoreHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public CacheHandlerRegister AddAsyncStoreHandler<TResult>(Func<CacheStoreContext<TResult>, Task> handler)
        {
            return AddAsyncStoreHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddAsyncStoreHandler<TResult>(HttpCallHandlerPriority priority, Func<CacheStoreContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(CacheHandlerType.Store, priority, handler);

            return this;
        }

        #endregion

        #region Expiring

        public CacheHandlerRegister AddExpiringHandler(Action<CacheExpiringContext> handler)
        {
            return AddExpiringHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddExpiringHandler(HttpCallHandlerPriority priority, Action<CacheExpiringContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncExpiringHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public CacheHandlerRegister AddAsyncExpiringHandler(Func<CacheExpiringContext, Task> handler)
        {
            return AddAsyncExpiringHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddAsyncExpiringHandler(HttpCallHandlerPriority priority, Func<CacheExpiringContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(CacheHandlerType.Expiring, priority, handler);

            return this;
        }

        #endregion

        #region Expired

        public CacheHandlerRegister AddExpiredHandler(Action<CacheExpiredContext> handler)
        {
            return AddExpiredHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddExpiredHandler(HttpCallHandlerPriority priority, Action<CacheExpiredContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncExpiredHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public CacheHandlerRegister AddAsyncExpiredHandler(Func<CacheExpiredContext, Task> handler)
        {
            return AddAsyncExpiredHandler(HttpCallHandlerPriority.Default, handler);
        }

        public CacheHandlerRegister AddAsyncExpiredHandler(HttpCallHandlerPriority priority, Func<CacheExpiredContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(CacheHandlerType.Expired, priority, handler);

            return this;
        }

        #endregion

    }
}