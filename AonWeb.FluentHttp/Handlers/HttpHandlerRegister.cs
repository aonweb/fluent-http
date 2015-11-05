using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpHandlerRegister
    {
        private delegate Task HandlerDelegate(HttpHandlerContext context);
        private readonly ISet<IHttpHandler> _handlerInstances;
        private readonly ConcurrentDictionary<HandlerType, ConcurrentDictionary<HandlerPriority, ICollection<HandlerDelegate>>> _handlers;

        public HttpHandlerRegister()
        {
            _handlerInstances = new HashSet<IHttpHandler>();
            _handlers = new ConcurrentDictionary<HandlerType, ConcurrentDictionary<HandlerPriority, ICollection<HandlerDelegate>>>();
        }

        public async Task OnSending(HttpSendingContext context)
        {
            foreach (var handler in GetHandlers(HandlerType.Sending))
                await handler(context);
        }

        public async Task OnSent(HttpSentContext context)
        {
            foreach (var handler in GetHandlers(HandlerType.Sent))
                await handler(context);
        }

        public async Task OnException(HttpExceptionContext context)
        {
            foreach (var handler in GetHandlers(HandlerType.Exception))
                await handler(context);
        }

        public HttpHandlerRegister WithHandler(IHttpHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_handlerInstances.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _handlerInstances.Add(handler);

            WithAsyncSendingHandler(
                handler.GetPriority(HandlerType.Sending), async ctx =>
                {  
                    if (handler.Enabled)
                        await handler.OnSending(ctx);
                });

            WithAsyncSentHandler(handler.GetPriority(HandlerType.Sent), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnSent(ctx);
            });

            WithAsyncExceptionHandler(handler.GetPriority(HandlerType.Exception), async ctx =>
            {
                if (handler.Enabled)
                    await handler.OnException(ctx);
            });

            return this;
        }

        public HttpHandlerRegister WithConfiguration<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, IHttpHandler
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var handler = _handlerInstances.OfType<THandler>().FirstOrDefault();

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

        public HttpHandlerRegister WithSendingHandler(Action<HttpSendingContext> handler)
        {
            return WithSendingHandler(HandlerPriority.Default, handler);
        }

        public HttpHandlerRegister WithSendingHandler(HandlerPriority priority, Action<HttpSendingContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncSendingHandler(priority, handler.ToTask);
        }

        public HttpHandlerRegister WithAsyncSendingHandler(Func<HttpSendingContext, Task> handler)
        {
            return WithAsyncSendingHandler(HandlerPriority.Default, handler);
        }

        public HttpHandlerRegister WithAsyncSendingHandler(HandlerPriority priority, Func<HttpSendingContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            WithHandler(HandlerType.Sending, priority, context => handler((HttpSendingContext)context));

            return this;
        }

        #endregion

        #region Sent

        public HttpHandlerRegister WithSentHandler(Action<HttpSentContext> handler)
        {
            return WithSentHandler(HandlerPriority.Default, handler);
        }

        public HttpHandlerRegister WithSentHandler(HandlerPriority priority, Action<HttpSentContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncSentHandler(priority, handler.ToTask);
        }

        public HttpHandlerRegister WithAsyncSentHandler(Func<HttpSentContext, Task> handler)
        {
            return WithAsyncSentHandler(HandlerPriority.Default, handler);
        }

        public HttpHandlerRegister WithAsyncSentHandler(HandlerPriority priority, Func<HttpSentContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            WithHandler(HandlerType.Sent, priority, context => handler((HttpSentContext)context));

            return this;
        }

        #endregion

        #region Exception

        public HttpHandlerRegister WithExceptionHandler(Action<HttpExceptionContext> handler)
        {
            return WithExceptionHandler(HandlerPriority.Default, handler);
        }

        public HttpHandlerRegister WithExceptionHandler(HandlerPriority priority, Action<HttpExceptionContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncExceptionHandler(priority, handler.ToTask);
        }

        public HttpHandlerRegister WithAsyncExceptionHandler(Func<HttpExceptionContext, Task> handler)
        {
            return WithAsyncExceptionHandler(HandlerPriority.Default, handler);
        }

        public HttpHandlerRegister WithAsyncExceptionHandler(HandlerPriority priority, Func<HttpExceptionContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            WithHandler(HandlerType.Exception, priority, context => handler((HttpExceptionContext)context));

            return this;
        }

        #endregion

        private IEnumerable<HandlerDelegate> GetHandlers(HandlerType type)
        {
            ConcurrentDictionary<HandlerPriority, ICollection<HandlerDelegate>> handlers;

            if (!_handlers.TryGetValue(type, out handlers))
            {
                return Enumerable.Empty<HandlerDelegate>();
            }

            return handlers.Keys.OrderBy(k => k).SelectMany(k =>
            {
                ICollection<HandlerDelegate> col;
                if (!handlers.TryGetValue(k, out col))
                {
                    return Enumerable.Empty<HandlerDelegate>();
                }

                return col;
            });
        }

        private void WithHandler(HandlerType type, HandlerPriority priority, HandlerDelegate handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _handlers.AddOrUpdate(type, t => // Add dict of priority, handlers by type
            {
                var d = new ConcurrentDictionary<HandlerPriority, ICollection<HandlerDelegate>>();
                d.TryAdd(priority, new List<HandlerDelegate> { handler});
                return d;

            }, (t, priorityHandlers) => // Update dict of priority, handlers by type
            {
                priorityHandlers.AddOrUpdate(priority,
                    p => new List<HandlerDelegate> { handler }, // Add
                    (key, handlers) => // Update
                    {
                        handlers.Add(handler);
                        return handlers;
                   
                    });

                return priorityHandlers;
            });
        }
    }
}