using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpCallHandlerRegister
    {
        private readonly ISet<IHttpCallHandler> _callHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext, Task>>> _sendingHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext, Task>>> _sentHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext, Task>>> _exceptionHandlers;

        public HttpCallHandlerRegister()
        {
            _callHandlers = new HashSet<IHttpCallHandler>();
            _sendingHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext, Task>>>();
            _sentHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext, Task>>>();
            _exceptionHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext, Task>>>();
        }

        public async Task OnSending(HttpCallContext context)
        {
            foreach (var handler in _sendingHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnSent(HttpCallContext context)
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
            if (_callHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

            AddSendingHandler(handler.GetPriority(HttpCallHandlerType.Sending), ctx => handler.OnSending(ctx));
            AddSentHandler(handler.GetPriority(HttpCallHandlerType.Sent), ctx => handler.OnSending(ctx));
            AddExceptionHandler(handler.GetPriority(HttpCallHandlerType.Exception), ctx => handler.OnSending(ctx));

            return this;
        }

        public HttpCallHandlerRegister ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler
        {
            if (configure == null)
                throw new ArgumentNullException("configure");

            var handler = _callHandlers.OfType<THandler>().FirstOrDefault();

            if (handler == null)
                throw new KeyNotFoundException(string.Format(SR.HanderDoesNotExistErrorFormat, typeof(THandler).Name));

            configure(handler);

            return this;
        }

        #region Sending

        public HttpCallHandlerRegister AddSendingHandler(Action<HttpCallContext> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSendingHandler(HttpCallHandlerPriority priority, Action<HttpCallContext> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister AddSendingHandler(Func<HttpCallContext, Task> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSendingHandler(HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler)
        {
            _sendingHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Sent

        public HttpCallHandlerRegister AddSentHandler(Action<HttpCallContext> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSentHandler(HttpCallHandlerPriority priority, Action<HttpCallContext> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister AddSentHandler(Func<HttpCallContext, Task> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddSentHandler(HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler)
        {
            _sentHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext, Task>>(priority, handler));

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
            return AddExceptionHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister AddExceptionHandler(Func<HttpExceptionContext, Task> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister AddExceptionHandler(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler)
        {
            _exceptionHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext, Task>>(priority, handler));

            return this;
        }

        #endregion
    }

    public class HttpCallHandlerRegister<TResult, TContent, TError>
    {
        private readonly ISet<IHttpCallHandler<TResult, TContent, TError>> _callHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>> _sendingHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>> _sentHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>> _resultHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpErrorContext<TResult, TContent, TError>, Task>>> _errorHandlers;
        private readonly IList<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext<TResult, TContent, TError>, Task>>> _exceptionHandlers;

        public HttpCallHandlerRegister()
        {
            _callHandlers = new HashSet<IHttpCallHandler<TResult, TContent, TError>>();
            _sendingHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>>();
            _sentHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>>();
            _resultHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>>();
            _errorHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpErrorContext<TResult, TContent, TError>, Task>>>();
            _exceptionHandlers = new List<KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext<TResult, TContent, TError>, Task>>>();
        }

        public async Task OnSending(HttpCallContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _sendingHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnSent(HttpCallContext<TResult, TContent, TError> context)
        {
            foreach (var handler in _sentHandlers.OrderBy(kp => kp.Key).Select(kp => kp.Value))
                await handler(context);
        }

        public async Task OnResult(HttpCallContext<TResult, TContent, TError> context)
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
            if (_callHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

            AddSendingHandler(handler.GetPriority(HttpCallHandlerType.Sending), ctx => handler.OnSending(ctx));
            AddSentHandler(handler.GetPriority(HttpCallHandlerType.Sent), ctx => handler.OnSending(ctx));
            AddResultHandler(handler.GetPriority(HttpCallHandlerType.Result), ctx => handler.OnResult(ctx));
            AddErrorHandler(handler.GetPriority(HttpCallHandlerType.Error), ctx => handler.OnError(ctx));
            AddExceptionHandler(handler.GetPriority(HttpCallHandlerType.Exception), ctx => handler.OnSending(ctx));

            return this;
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>
        {
            if (configure == null)
                throw new ArgumentNullException("configure");

            var handler = _callHandlers.OfType<THandler>().FirstOrDefault();

            if (handler == null)
                throw new KeyNotFoundException(string.Format(SR.HanderDoesNotExistErrorFormat, typeof(THandler).Name));

            configure(handler);

            return this;
        }

        #region Sending

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(Action<HttpCallContext<TResult, TContent, TError>> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(HttpCallHandlerPriority priority, Action<HttpCallContext<TResult, TContent, TError>> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            return AddSendingHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSendingHandler(HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            _sendingHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Sent

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(Action<HttpCallContext<TResult, TContent, TError>> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(HttpCallHandlerPriority priority, Action<HttpCallContext<TResult, TContent, TError>> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            return AddSentHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddSentHandler(HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            _sentHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion

        #region Result

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(Action<HttpCallContext<TResult, TContent, TError>> handler)
        {
            return AddResultHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(HttpCallHandlerPriority priority, Action<HttpCallContext<TResult, TContent, TError>> handler)
        {
            return AddResultHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            return AddResultHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddResultHandler(HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            _resultHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpCallContext<TResult, TContent, TError>, Task>>(priority, handler));

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
            return AddErrorHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddErrorHandler(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            return AddErrorHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddErrorHandler(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
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
            return AddExceptionHandler(HttpCallHandlerPriority.Default, ctx => Task.Run(() => handler(ctx)));
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddExceptionHandler(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            return AddExceptionHandler(HttpCallHandlerPriority.Default, handler);
        }

        public HttpCallHandlerRegister<TResult, TContent, TError> AddExceptionHandler(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _exceptionHandlers.Add(new KeyValuePair<HttpCallHandlerPriority, Func<HttpExceptionContext<TResult, TContent, TError>, Task>>(priority, handler));

            return this;
        }

        #endregion
    }
}