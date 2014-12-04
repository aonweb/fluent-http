using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpCallHandlerRegister
    {
        private readonly ISet<ITypedHttpCallHandler> _callHandlers;

        private readonly IDictionary<HttpCallHandlerType, IList<KeyValuePair<HttpCallHandlerPriority, Delegate>>> _handlers;

        public TypedHttpCallHandlerRegister()
        {
            _callHandlers = new HashSet<ITypedHttpCallHandler>();

            _handlers = new Dictionary<HttpCallHandlerType, IList<KeyValuePair<HttpCallHandlerPriority, Delegate>>>();

            foreach (var callType in Enum.GetValues(typeof(HttpCallHandlerType)).Cast<HttpCallHandlerType>())
            {
                _handlers[callType] = new List<KeyValuePair<HttpCallHandlerPriority, Delegate>>();
            }
        }

        private void AddHandler(HttpCallHandlerType callType, HttpCallHandlerPriority priority, Delegate handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _handlers[callType].Add(new KeyValuePair<HttpCallHandlerPriority, Delegate>(priority, handler));
        }

        public async Task<ModifyTracker> OnSending(TypedHttpCallContext context, HttpRequestMessage request, object content, bool hasContent)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, HttpCallHandlerType.Sending, typeof(TypedHttpSendingContext<,>), new[] { context.ResultType, context.ContentType }, new []{context, request, content, hasContent}, context.SuppressHandlerTypeExceptions);
        }

        public async Task<ModifyTracker> OnSent(TypedHttpCallContext context, HttpResponseMessage response)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, HttpCallHandlerType.Sent, typeof(TypedHttpSentContext<>), new[] { context.ResultType }, new object[] { context, response }, context.SuppressHandlerTypeExceptions);
        }

        public async Task<ModifyTracker> OnResult(TypedHttpCallContext context, HttpResponseMessage response, object result)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, HttpCallHandlerType.Result, typeof(TypedHttpResultContext<>), new[] { context.ResultType }, new[] { context, response, result }, context.SuppressHandlerTypeExceptions);
        }

        public async Task<ModifyTracker> OnError(TypedHttpCallContext context, HttpResponseMessage response, object error)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, HttpCallHandlerType.Error, typeof(TypedHttpErrorContext<>), new[] { context.ErrorType }, new[] { context, response, error }, context.SuppressHandlerTypeExceptions);
        }

        public async Task<ModifyTracker> OnException(TypedHttpCallContext context, HttpResponseMessage response, Exception exception)
        {
            return await HandlerHelper.InvokeHandlers(_handlers, HttpCallHandlerType.Exception, typeof(TypedHttpExceptionContext), null, new object[] { context, response, exception }, context.SuppressHandlerTypeExceptions);
        }

        public TypedHttpCallHandlerRegister AddHandler<TResult, TContent, TError>(ITypedHttpCallHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (_callHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

            AddAsyncSendingHandler<TResult, TContent>(handler.GetPriority(HttpCallHandlerType.Sending), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnSending(ctx);
            });

            AddAsyncSentHandler<TResult>(handler.GetPriority(HttpCallHandlerType.Sent), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnSent(ctx);
                });

            AddAsyncResultHandler<TResult>(handler.GetPriority(HttpCallHandlerType.Result), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnResult(ctx);
                });

            AddAsyncErrorHandler<TError>(handler.GetPriority(HttpCallHandlerType.Error), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnError(ctx);
                });

            AddAsyncExceptionHandler(handler.GetPriority(HttpCallHandlerType.Exception), async ctx =>
                {
                    if (handler.Enabled)
                        await handler.OnException(ctx);
                });

            return this;
        }

        public TypedHttpCallHandlerRegister ConfigureHandler<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, ITypedHttpCallHandler
        {
            if (configure == null)
                throw new ArgumentNullException("configure");

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

        public TypedHttpCallHandlerRegister AddSendingHandler<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddSendingHandler<TResult, TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, TContent>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncSendingHandler<TResult, TContent>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHttpCallHandlerRegister AddAsyncSendingHandler<TResult, TContent>(Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
        {
            return AddAsyncSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddAsyncSendingHandler<TResult, TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(HttpCallHandlerType.Sending, priority, handler);

            return this;
        }

        #endregion

        #region Sent

        public TypedHttpCallHandlerRegister AddSentHandler<TResult>(Action<TypedHttpSentContext<TResult>> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddSentHandler<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSentContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncSentHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHttpCallHandlerRegister AddAsyncSentHandler<TResult>(Func<TypedHttpSentContext<TResult>, Task> handler)
        {
            return AddAsyncSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddAsyncSentHandler<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSentContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(HttpCallHandlerType.Sent, priority, handler);

            return this;
        }

        #endregion

        #region Result

        public TypedHttpCallHandlerRegister AddResultHandler<TResult>(Action<TypedHttpResultContext<TResult>> handler)
        {
            return AddResultHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddResultHandler<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpResultContext<TResult>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncResultHandler<TResult>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHttpCallHandlerRegister AddAsyncResultHandler<TResult>(Func<TypedHttpResultContext<TResult>, Task> handler)
        {
            return AddAsyncResultHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddAsyncResultHandler<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpResultContext<TResult>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(HttpCallHandlerType.Result, priority, handler);

            return this;
        }

        #endregion

        #region Error

        public TypedHttpCallHandlerRegister AddErrorHandler<TError>(Action<TypedHttpErrorContext<TError>> handler)
        {
            return AddErrorHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddErrorHandler<TError>(HttpCallHandlerPriority priority, Action<TypedHttpErrorContext<TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncErrorHandler<TError>(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHttpCallHandlerRegister AddAsyncErrorHandler<TError>(Func<TypedHttpErrorContext<TError>, Task> handler)
        {
            return AddAsyncErrorHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddAsyncErrorHandler<TError>(HttpCallHandlerPriority priority, Func<TypedHttpErrorContext<TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(HttpCallHandlerType.Error, priority, handler);

            return this;
        }

        #endregion

        #region Exception

        public TypedHttpCallHandlerRegister AddExceptionHandler(Action<TypedHttpExceptionContext> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddExceptionHandler(HttpCallHandlerPriority priority, Action<TypedHttpExceptionContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddAsyncExceptionHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public TypedHttpCallHandlerRegister AddAsyncExceptionHandler(Func<TypedHttpExceptionContext, Task> handler)
        {
            return AddAsyncExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public TypedHttpCallHandlerRegister AddAsyncExceptionHandler(HttpCallHandlerPriority priority, Func<TypedHttpExceptionContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            AddHandler(HttpCallHandlerType.Exception, priority, handler);

            return this;
        }

        #endregion
    }
}