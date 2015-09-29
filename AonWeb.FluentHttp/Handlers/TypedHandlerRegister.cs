using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{


    public class TypedHandlerRegister
    {
        private delegate Task TypedHandlerDelegate(TypedHandlerContext context);
        private readonly ISet<ITypedHandler> _callHandlers;
        private readonly ConcurrentDictionary<HandlerType, ConcurrentDictionary<HandlerPriority, ICollection<TypedHandlerInfo>>> _handlers;
        private readonly ConcurrentDictionary<string, object> _contextConstructorCache;

        public TypedHandlerRegister()
        {
            _callHandlers = new HashSet<ITypedHandler>();

            _handlers = new ConcurrentDictionary<HandlerType, ConcurrentDictionary<HandlerPriority, ICollection<TypedHandlerInfo>>>();
            _contextConstructorCache = new ConcurrentDictionary<string, object>();

        }

        public async Task<Modifiable> OnSending(ITypedBuilderContext context, HttpRequestMessage request, object content, bool hasContent)
        {
            TypedSendingContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(HandlerType.Sending))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ITypedBuilderContext, HttpRequestMessage, object, bool, TypedSendingContext>)handlerInfo.InitialConstructor)(context, request, content, hasContent);
                else
                    handlerContext = ((Func<TypedSendingContext, TypedSendingContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return handlerContext?.GetHandlerResult();
        }

        public async Task<Modifiable> OnSent(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response)
        {
            TypedSentContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(HandlerType.Sent))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, TypedSentContext>)handlerInfo.InitialConstructor)(context, request, response);
                else
                    handlerContext = ((Func<TypedSentContext, TypedSentContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return handlerContext?.GetHandlerResult();
        }

        public async Task<Modifiable> OnResult(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, object result)
        {
            TypedResultContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(HandlerType.Result))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, object, TypedResultContext>)handlerInfo.InitialConstructor)(context, request, response, result);
                else
                    handlerContext = ((Func<TypedResultContext, TypedResultContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return handlerContext?.GetHandlerResult();
        }

        public async Task<Modifiable> OnError(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, object error)
        {
            TypedErrorContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(HandlerType.Error))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, object, TypedErrorContext>)handlerInfo.InitialConstructor)(context, request, response, error);
                else
                    handlerContext = ((Func<TypedErrorContext, TypedErrorContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return handlerContext?.GetHandlerResult();
        }

        public async Task<Modifiable> OnException(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, Exception exception)
        {
            TypedExceptionContext handlerContext = null;

            foreach (var handlerInfo in GetHandlerInfo(HandlerType.Exception))
            {
                if (handlerContext == null)
                    handlerContext = ((Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Exception, TypedExceptionContext>)handlerInfo.InitialConstructor)(context, request, response, exception);
                else
                    handlerContext = ((Func<TypedExceptionContext, TypedExceptionContext>)handlerInfo.ContinuationConstructor)(handlerContext);

                await handlerInfo.Handler(handlerContext);
            }

            return handlerContext?.GetHandlerResult();
        }

        public TypedHandlerRegister WithHandler<TResult, TContent, TError>(ITypedHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_callHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

            WithAsyncSendingHandler<TResult, TContent>(handler.GetPriority(HandlerType.Sending), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnSending(ctx);
            });

            WithAsyncSentHandler<TResult>(handler.GetPriority(HandlerType.Sent), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnSent(ctx);
                });

            WithAsyncResultHandler<TResult>(handler.GetPriority(HandlerType.Result), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnResult(ctx);
                });

            WithAsyncErrorHandler<TError>(handler.GetPriority(HandlerType.Error), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnError(ctx);
                });

            WithAsyncExceptionHandler(handler.GetPriority(HandlerType.Exception), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnException(ctx);
                });

            return this;
        }

        public TypedHandlerRegister WithConfiguration<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, ITypedHandler
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var handler = _callHandlers.OfType<THandler>().FirstOrDefault();

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

        #region Sending

        public TypedHandlerRegister WithSendingHandler<TResult, TContent>(Action<TypedSendingContext<TResult, TContent>> handler)
        {
            return WithSendingHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithSendingHandler<TResult, TContent>(HandlerPriority priority, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncSendingHandler<TResult, TContent>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHandlerRegister WithAsyncSendingHandler<TResult, TContent>(Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            return WithAsyncSendingHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithAsyncSendingHandler<TResult, TContent>(HandlerPriority priority, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new TypedHandlerInfo
            {
                Handler = context => handler((TypedSendingContext<TResult, TContent>)context),
                InitialConstructor = GetOrAddFromCtorCache(HandlerType.Sending, handler.GetType(), false, (Func<ITypedBuilderContext, HttpRequestMessage, object, bool, TypedSendingContext>)(
                    (ctx, request, content, hasContent) => 
                        new TypedSendingContext<TResult, TContent>(ctx, request, ObjectHelpers.CheckType<TContent>(content, ctx.SuppressTypeMismatchExceptions), hasContent))),
                ContinuationConstructor = GetOrAddFromCtorCache(HandlerType.Sending, handler.GetType(), true, (Func<TypedSendingContext, TypedSendingContext>)(
                    ctx => 
                        new TypedSendingContext<TResult, TContent>(ctx))),
            };

            WithHandler(HandlerType.Sending, priority, handlerInfo);

            return this;
        }



        #endregion

        #region Sent

        public TypedHandlerRegister WithSentHandler<TResult>(Action<TypedSentContext<TResult>> handler)
        {
            return WithSentHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithSentHandler<TResult>(HandlerPriority priority, Action<TypedSentContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncSentHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHandlerRegister WithAsyncSentHandler<TResult>(Func<TypedSentContext<TResult>, Task> handler)
        {
            return WithAsyncSentHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithAsyncSentHandler<TResult>(HandlerPriority priority, Func<TypedSentContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new TypedHandlerInfo
            {
                Handler = context => handler((TypedSentContext<TResult>)context),
                InitialConstructor = GetOrAddFromCtorCache(HandlerType.Sent, handler.GetType(), false, (Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, TypedSentContext>)((ctx, request, response) => new TypedSentContext<TResult>(ctx, request, response))),
                ContinuationConstructor = GetOrAddFromCtorCache(HandlerType.Sent, handler.GetType(), true, (Func<TypedSentContext, TypedSentContext>)(ctx => new TypedSentContext<TResult>(ctx))),
            };

            WithHandler(HandlerType.Sent, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Result

        public TypedHandlerRegister WithResultHandler<TResult>(Action<TypedResultContext<TResult>> handler)
        {
            return WithResultHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithResultHandler<TResult>(HandlerPriority priority, Action<TypedResultContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncResultHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHandlerRegister WithAsyncResultHandler<TResult>(Func<TypedResultContext<TResult>, Task> handler)
        {
            return WithAsyncResultHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithAsyncResultHandler<TResult>(HandlerPriority priority, Func<TypedResultContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new TypedHandlerInfo
            {
                Handler = context => handler((TypedResultContext<TResult>)context),
                InitialConstructor = GetOrAddFromCtorCache(HandlerType.Result, handler.GetType(), false, (Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, object, TypedResultContext>)((ctx, request, response, result) => new TypedResultContext<TResult>(ctx, request, response, ObjectHelpers.CheckType<TResult>( result, ctx.SuppressTypeMismatchExceptions)))),
                ContinuationConstructor = GetOrAddFromCtorCache(HandlerType.Result, handler.GetType(), true, (Func<TypedResultContext, TypedResultContext>)(ctx => new TypedResultContext<TResult>(ctx))),
            };

            WithHandler(HandlerType.Result, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Error

        public TypedHandlerRegister WithErrorHandler<TError>(Action<TypedErrorContext<TError>> handler)
        {
            return WithErrorHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithErrorHandler<TError>(HandlerPriority priority, Action<TypedErrorContext<TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncErrorHandler<TError>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHandlerRegister WithAsyncErrorHandler<TError>(Func<TypedErrorContext<TError>, Task> handler)
        {
            return WithAsyncErrorHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithAsyncErrorHandler<TError>(HandlerPriority priority, Func<TypedErrorContext<TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new TypedHandlerInfo
            {
                Handler = context => handler((TypedErrorContext<TError>)context),
                InitialConstructor = GetOrAddFromCtorCache(HandlerType.Error, handler.GetType(), false, (Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, object, TypedErrorContext>)((ctx, request, response, error) => new TypedErrorContext<TError>(ctx, request, response, ObjectHelpers.CheckType<TError>(error, ctx.SuppressTypeMismatchExceptions)))),
                ContinuationConstructor = GetOrAddFromCtorCache(HandlerType.Error, handler.GetType(), true, (Func<TypedErrorContext, TypedErrorContext>)(ctx => new TypedErrorContext<TError>(ctx))),
            };

            WithHandler(HandlerType.Error, priority, handlerInfo);

            return this;
        }

        #endregion

        #region Exception

        public TypedHandlerRegister WithExceptionHandler(Action<TypedExceptionContext> handler)
        {
            return WithExceptionHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithExceptionHandler(HandlerPriority priority, Action<TypedExceptionContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncExceptionHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHandlerRegister WithAsyncExceptionHandler(Func<TypedExceptionContext, Task> handler)
        {
            return WithAsyncExceptionHandler(HandlerPriority.Default, handler);
        }

        public TypedHandlerRegister WithAsyncExceptionHandler(HandlerPriority priority, Func<TypedExceptionContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var handlerInfo = new TypedHandlerInfo
            {
                Handler = context => handler((TypedExceptionContext)context),
                InitialConstructor = GetOrAddFromCtorCache(HandlerType.Exception, handler.GetType(), false, (Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Exception, TypedExceptionContext>)((ctx, request, response, exception) => new TypedExceptionContext(ctx, request, response, exception))),
                ContinuationConstructor = GetOrAddFromCtorCache(HandlerType.Exception, handler.GetType(), true, (Func<TypedExceptionContext, TypedExceptionContext>)(ctx => new TypedExceptionContext(ctx)))
            };

            WithHandler(HandlerType.Exception, priority, handlerInfo);

            return this;
        }

        #endregion

        private IEnumerable<TypedHandlerInfo> GetHandlerInfo(HandlerType type)
        {
            ConcurrentDictionary<HandlerPriority, ICollection<TypedHandlerInfo>> handlers;

            if (!_handlers.TryGetValue(type, out handlers))
            {
                return Enumerable.Empty<TypedHandlerInfo>();
            }

            return handlers.Keys.OrderBy(k => k).SelectMany(k =>
            {
                ICollection<TypedHandlerInfo> col;
                if (!handlers.TryGetValue(k, out col))
                {
                    return Enumerable.Empty<TypedHandlerInfo>();
                }

                return col;
            });
        }

        private void WithHandler(HandlerType type, HandlerPriority priority, TypedHandlerInfo handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handlers.AddOrUpdate(type, t => // Add dict of priority, handlers by type
            {
                var d = new ConcurrentDictionary<HandlerPriority, ICollection<TypedHandlerInfo>>();
                d.TryAdd(priority, new List<TypedHandlerInfo> { handler });
                return d;

            }, (t, priorityHandlers) => // Update dict of priority, handlers by type
            {
                priorityHandlers.AddOrUpdate(priority,
                    p => new List<TypedHandlerInfo> { handler }, // Add
                    (key, handlers) => // Update
                    {
                        handlers.Add(handler);
                        return handlers;

                    });

                return priorityHandlers;
            });
        }

        private object GetOrAddFromCtorCache(HandlerType type, Type handlerType, bool isContinuation, object ctor)
        {
            var ctorType = isContinuation ? "C" : "I";
            var key = $"{(int)type}:{handlerType.FormattedTypeName()}:{ctorType}";

            return _contextConstructorCache.GetOrAdd(key, k => ctor);
        }

        private class TypedHandlerInfo
        {
            public object InitialConstructor { get; set; }

            public object ContinuationConstructor { get; set; }

            public TypedHandlerDelegate Handler { get; set; }
        }
    }
}