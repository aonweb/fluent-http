using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpCallHandlerRegister
    {
        private readonly ISet<IHttpCallHandler> _callHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpSendingContext, Task>>> _sendingHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpSentContext, Task>>> _sentHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext, Task>>> _exceptionHandlers;

        public HttpCallHandlerRegister()
        {
            _callHandlers = new HashSet<IHttpCallHandler>();
            _sendingHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpSendingContext, Task>>>();
            _sentHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpSentContext, Task>>>();
            _exceptionHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext, Task>>>();
        }

        public async Task OnSending(HttpSendingContext context)
        {
            foreach (var handler in _sendingHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnSent(HttpSentContext context)
        {
            foreach (var handler in _sentHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnException(HttpExceptionContext context)
        {
            foreach (var handler in _exceptionHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public HttpCallHandlerRegister AddHandler(IHttpCallHandler handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            if (_callHandlers.Contains(handler)) throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

            AddSendingHandler(
                handler.GetPriority(HttpCallHandlerType.Sending), async ctx =>
                {  
                    if (handler.Enabled)
                        await handler.OnSending(ctx);
                });

            AddSentHandler(handler.GetPriority(HttpCallHandlerType.Sent), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnSent(ctx);
            });

            AddExceptionHandler(handler.GetPriority(HttpCallHandlerType.Exception), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnException(ctx);
            });

            return this;
        }

        public HttpCallHandlerRegister ConfigureHandler<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, IHttpCallHandler
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

        public HttpCallHandlerRegister AddSendingHandler(Action<HttpSendingContext> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSendingHandler(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddSendingHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister AddSendingHandler(Func<HttpSendingContext, Task> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSendingHandler(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _sendingHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpSendingContext, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Sent

        public HttpCallHandlerRegister AddSentHandler(Action<HttpSentContext> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSentHandler(HttpCallHandlerPriority priority, Action<HttpSentContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddSentHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister AddSentHandler(Func<HttpSentContext, Task> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSentHandler(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _sentHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpSentContext, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Exception

        public HttpCallHandlerRegister AddExceptionHandler(Action<HttpExceptionContext> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddExceptionHandler(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddExceptionHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister AddExceptionHandler(Func<HttpExceptionContext, Task> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddExceptionHandler(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _exceptionHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext, Task>>(priority, handler));

            return this;
        }

        #endregion
    }

    public class HttpCallHandlerRegister<TResult, TContent, TError>
    {
        private readonly ISet<IHttpCallHandler<TResult, TContent, TError>> _callHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpSendingContext<TResult, TContent, TError>, Task>>> _sendingHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpSentContext<TResult, TContent, TError>, Task>>> _sentHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpResultContext<TResult, TContent, TError>, Task>>> _resultHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpErrorContext<TResult, TContent, TError>, Task>>> _errorHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext<TResult, TContent, TError>, Task>>> _exceptionHandlers;

        public HttpCallHandlerRegister()
        {
            _callHandlers = new HashSet<IHttpCallHandler<TResult, TContent, TError>>();
            _sendingHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpSendingContext<TResult, TContent, TError>, Task>>>();
            _sentHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpSentContext<TResult, TContent, TError>, Task>>>();
            _resultHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpResultContext<TResult, TContent, TError>, Task>>>();
            _errorHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpErrorContext<TResult, TContent, TError>, Task>>>();
            _exceptionHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext<TResult, TContent, TError>, Task>>>();
        }

        public async Task OnSending(HttpSendingContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _sendingHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnSent(HttpSentContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _sentHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnResult(HttpResultContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _resultHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnError(HttpErrorContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _errorHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnException(HttpExceptionContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _exceptionHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddHandler(IHttpCallHandler<TResult, TContent, TError> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (_callHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

            AddSendingHandler(handler.GetPriority(HttpCallHandlerType.Sending), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnSending(ctx);
            });

            AddSentHandler(handler.GetPriority(HttpCallHandlerType.Sent), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnSent(ctx);
            });

            AddResultHandler(handler.GetPriority(HttpCallHandlerType.Result), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnResult(ctx);
            });

            AddErrorHandler(handler.GetPriority(HttpCallHandlerType.Error), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnError(ctx);
            });

            AddExceptionHandler(handler.GetPriority(HttpCallHandlerType.Exception), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnException(ctx);
            });

            return this;
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>
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

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddSendingHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _sendingHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpSendingContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Sent

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddSentHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _sentHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpSentContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Result

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            return AddResultHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddResultHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            return AddResultHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _resultHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpResultContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Error

        public HttpCallHandlerRegister<TResult, TContent, TError> AddErrorHandler(Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            return AddErrorHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddErrorHandler(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddErrorHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddErrorHandler(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            return AddErrorHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddErrorHandler(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _errorHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpErrorContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Exception

        public HttpCallHandlerRegister<TResult, TContent, TError> AddExceptionHandler(Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddExceptionHandler(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            return AddExceptionHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddExceptionHandler(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddExceptionHandler(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            _exceptionHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion
    }
}