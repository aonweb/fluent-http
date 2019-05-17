using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheHandlerRegister
    {
        private delegate Task CacheHandlerDelegate(CacheHandlerContext context);
        private readonly ISet<ICacheHandler> _cacheHandlers;
        private readonly IDictionary<CacheHandlerType, IDictionary<HandlerPriority, ICollection<CacheHandlerInfo>>> _handlers;
        private readonly IDictionary<string, object> _contextConstructorCache;

        public CacheHandlerRegister()
        {
            _cacheHandlers = new HashSet<ICacheHandler>();

            _handlers = new Dictionary<CacheHandlerType, IDictionary<HandlerPriority, ICollection<CacheHandlerInfo>>>();
            _contextConstructorCache = new Dictionary<string, object>();
        }

        public CacheHandlerRegister WithHandler(ICacheHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_cacheHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _cacheHandlers.Add(handler);

            WithAsyncHitHandler<object>(handler.GetPriority(CacheHandlerType.Hit), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnHit(ctx);
            });

            WithAsyncMissHandler<object>(handler.GetPriority(CacheHandlerType.Miss), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnMiss(ctx);
            });

            WithAsyncStoreHandler<object>(handler.GetPriority(CacheHandlerType.Store), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnStore(ctx);
            });

            WithAsyncExpiringHandler(handler.GetPriority(CacheHandlerType.Expiring), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnExpiring(ctx);
            });

            WithAsyncExpiredHandler(handler.GetPriority(CacheHandlerType.Expired), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnExpired(ctx);
            });

            return this;
        }

        public CacheHandlerRegister WithHandlerConfiguration<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, ICacheHandler
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

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

        public async Task<Modifiable> OnHit(ICacheContext context, object result)
        {
            CacheHitContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(CacheHandlerType.Hit))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ICacheContext, object, CacheHitContext>)handlerInfo.InitialConstructor)(context, result);
                else
                    handlerContext = ((Func<CacheHitContext, CacheHitContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return (handlerContext ?? new CacheHitContext<object>(context, null)).GetHandlerResult();
        }

        public async Task<Modifiable> OnMiss(ICacheContext context)
        {
            CacheMissContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(CacheHandlerType.Miss))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ICacheContext, CacheMissContext>)handlerInfo.InitialConstructor)(context);
                else
                    handlerContext = ((Func<CacheMissContext, CacheMissContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return (handlerContext ?? new CacheMissContext<object>(context)).GetHandlerResult();
        }

        public async Task<Modifiable> OnStore(ICacheContext context, object result)
        {
            CacheStoreContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(CacheHandlerType.Store))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ICacheContext, object, CacheStoreContext>)handlerInfo.InitialConstructor)(context, result);
                else
                    handlerContext = ((Func<CacheStoreContext, CacheStoreContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return (handlerContext ?? new CacheStoreContext<object>(context, null)).GetHandlerResult();
        }

        public async Task<Modifiable> OnExpiring(ICacheContext context, RequestValidationResult reason)
        {
            CacheExpiringContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(CacheHandlerType.Expiring))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ICacheContext, RequestValidationResult, CacheExpiringContext>)handlerInfo.InitialConstructor)(context, reason);
                else
                    handlerContext = ((Func<CacheExpiringContext, CacheExpiringContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return (handlerContext ?? new CacheExpiringContext(context, 0)).GetHandlerResult();
        }

        public async Task<Modifiable> OnExpired(ICacheContext context, RequestValidationResult reason, IReadOnlyCollection<Uri> expiredUris)
        {
            CacheExpiredContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(CacheHandlerType.Expired))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ICacheContext, RequestValidationResult, IReadOnlyCollection<Uri>,  CacheExpiredContext>)handlerInfo.InitialConstructor)(context, reason, expiredUris);
                else
                    handlerContext = ((Func<CacheExpiredContext, CacheExpiredContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return (handlerContext ?? new CacheExpiredContext(context, 0, null)).GetHandlerResult();
        }

        #region Hit

        public CacheHandlerRegister WithHitHandler<TResult>(Action<CacheHitContext<TResult>> handler)
        {
            return WithHitHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithHitHandler<TResult>(HandlerPriority priority, Action<CacheHitContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncHitHandler<TResult>(priority, handler.ToTask);
        }

        public CacheHandlerRegister WithAsyncHitHandler<TResult>(Func<CacheHitContext<TResult>, Task> handler)
        {
            return WithAsyncHitHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithAsyncHitHandler<TResult>(HandlerPriority priority, Func<CacheHitContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new CacheHandlerInfo
            {
                Handler = context => handler((CacheHitContext<TResult>)context),
                InitialConstructor = GetOrAddFromCtorCache(CacheHandlerType.Hit, handler.GetType(), false, (Func<ICacheContext, object, CacheHitContext>)((ctx, result) => new CacheHitContext<TResult>(ctx, TypeHelpers.CheckType<TResult>(result, ctx.SuppressTypeMismatchExceptions)))),
                ContinuationConstructor = GetOrAddFromCtorCache(CacheHandlerType.Hit, handler.GetType(), true, (Func<CacheHitContext, CacheHitContext>)(ctx => new CacheHitContext<TResult>(ctx))),
            };

            WithHandler(CacheHandlerType.Hit, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Miss

        

        public CacheHandlerRegister WithMissHandler<TResult>(Action<CacheMissContext<TResult>> handler)
        {
            return WithMissHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithMissHandler<TResult>(HandlerPriority priority, Action<CacheMissContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncMissHandler<TResult>(priority, handler.ToTask);
        }

        public CacheHandlerRegister WithAsyncMissHandler<TResult>(Func<CacheMissContext<TResult>, Task> handler)
        {
            return WithAsyncMissHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithAsyncMissHandler<TResult>(HandlerPriority priority, Func<CacheMissContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new CacheHandlerInfo
            {
                Handler = context => handler((CacheMissContext<TResult>)context),
                InitialConstructor = GetOrAddFromCtorCache(CacheHandlerType.Miss, handler.GetType(), false, (Func<ICacheContext, CacheMissContext>)(ctx => new CacheMissContext<TResult>(ctx))),
                ContinuationConstructor = GetOrAddFromCtorCache(CacheHandlerType.Miss, handler.GetType(), true, (Func<CacheMissContext, CacheMissContext>)(ctx => new CacheMissContext<TResult>(ctx))),
            };

            WithHandler(CacheHandlerType.Miss, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Store

        public CacheHandlerRegister WithStoreHandler<TResult>(Action<CacheStoreContext<TResult>> handler)
        {
            return WithStoreHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithStoreHandler<TResult>(HandlerPriority priority, Action<CacheStoreContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncStoreHandler<TResult>(priority, handler.ToTask);
        }

        public CacheHandlerRegister WithAsyncStoreHandler<TResult>(Func<CacheStoreContext<TResult>, Task> handler)
        {
            return WithAsyncStoreHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithAsyncStoreHandler<TResult>(HandlerPriority priority, Func<CacheStoreContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new CacheHandlerInfo
            {
                Handler = context => handler((CacheStoreContext<TResult>)context),
                InitialConstructor = GetOrAddFromCtorCache(CacheHandlerType.Store, handler.GetType(), false, (Func<ICacheContext, object, CacheStoreContext>)((ctx, result) => new CacheStoreContext<TResult>(ctx, TypeHelpers.CheckType<TResult>(result, ctx.SuppressTypeMismatchExceptions)))),
                ContinuationConstructor = GetOrAddFromCtorCache(CacheHandlerType.Store, handler.GetType(), true, (Func<CacheStoreContext, CacheStoreContext>)(ctx => new CacheStoreContext<TResult>(ctx))),
            };

            WithHandler(CacheHandlerType.Store, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Expiring

        public CacheHandlerRegister WithExpiringHandler(Action<CacheExpiringContext> handler)
        {
            return WithExpiringHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithExpiringHandler(HandlerPriority priority, Action<CacheExpiringContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncExpiringHandler(priority, handler.ToTask);
        }

        public CacheHandlerRegister WithAsyncExpiringHandler(Func<CacheExpiringContext, Task> handler)
        {
            return WithAsyncExpiringHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithAsyncExpiringHandler(HandlerPriority priority, Func<CacheExpiringContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new CacheHandlerInfo
            {
                Handler = context => handler((CacheExpiringContext)context),
                InitialConstructor = GetOrAddFromCtorCache(CacheHandlerType.Expiring, handler.GetType(), false, (Func<ICacheContext, RequestValidationResult , CacheExpiringContext>)((ctx, reason) => new CacheExpiringContext(ctx, reason))),
                ContinuationConstructor = GetOrAddFromCtorCache(CacheHandlerType.Expiring, handler.GetType(), true, (Func<CacheExpiringContext, CacheExpiringContext>)(ctx => new CacheExpiringContext(ctx))),
            };

            WithHandler(CacheHandlerType.Expiring, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Expired

        public CacheHandlerRegister WithExpiredHandler(Action<CacheExpiredContext> handler)
        {
            return WithExpiredHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithExpiredHandler(HandlerPriority priority, Action<CacheExpiredContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncExpiredHandler(priority, handler.ToTask);
        }

        public CacheHandlerRegister WithAsyncExpiredHandler(Func<CacheExpiredContext, Task> handler)
        {
            return WithAsyncExpiredHandler(HandlerPriority.Default, handler);
        }

        public CacheHandlerRegister WithAsyncExpiredHandler(HandlerPriority priority, Func<CacheExpiredContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new CacheHandlerInfo
            {
                Handler = context => handler((CacheExpiredContext)context),
                InitialConstructor = GetOrAddFromCtorCache(CacheHandlerType.Expired, handler.GetType(), false, (Func<ICacheContext, RequestValidationResult, IReadOnlyCollection<Uri>,  CacheExpiredContext>)((ctx, reason, expiredUris) => new CacheExpiredContext(ctx, reason, expiredUris))),
                ContinuationConstructor = GetOrAddFromCtorCache(CacheHandlerType.Expired, handler.GetType(), true, (Func<CacheExpiredContext, CacheExpiredContext>)(ctx => new CacheExpiredContext(ctx))),
            };

            WithHandler(CacheHandlerType.Expired, priority, handlerInfo);

            return this;
        }

        #endregion

        private IEnumerable<CacheHandlerInfo> GetHandlerInfo(CacheHandlerType type)
        {
            IDictionary<HandlerPriority, ICollection<CacheHandlerInfo>> handlers;

            lock (_handlers)
            {
                if (!_handlers.TryGetValue(type, out handlers))
                {
                    return Enumerable.Empty<CacheHandlerInfo>();
                }

                return handlers.Keys.OrderBy(k => k).SelectMany(k =>
                {
                    ICollection<CacheHandlerInfo> col;
                    if (!handlers.TryGetValue(k, out col))
                    {
                        return Enumerable.Empty<CacheHandlerInfo>();
                    }

                    return col;
                });
            }
        }

        private void WithHandler(CacheHandlerType type, HandlerPriority priority, CacheHandlerInfo handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_handlers)
            {
                if (!_handlers.ContainsKey(type))
                {
                    _handlers[type] = new Dictionary<HandlerPriority, ICollection<CacheHandlerInfo>>()
                    {
                        {priority, new List<CacheHandlerInfo> {handler}}
                    };
                }
                else
                {
                    var handlersForType = _handlers[type];

                    if (!handlersForType.ContainsKey(priority))
                    {
                        handlersForType[priority] = new List<CacheHandlerInfo> {handler};
                    }
                    else
                    {
                        handlersForType[priority].Add(handler);
                    }
                }
            }
        }

        private object GetOrAddFromCtorCache(CacheHandlerType type, Type handlerType, bool isContinuation, object ctor)
        {
            var ctorType = isContinuation ? "C" : "I";
            var key = $"{(int)type}:{handlerType.FormattedTypeName()}:{ctorType}";

            lock (_contextConstructorCache)
            {
                if (!_contextConstructorCache.ContainsKey(key))
                    _contextConstructorCache[key] = ctor;

                return ctor;
            }
        }

        private class CacheHandlerInfo
        {
            public object InitialConstructor { get; set; }

            public object ContinuationConstructor { get; set; }

            public CacheHandlerDelegate Handler { get; set; }
        }
    }
}