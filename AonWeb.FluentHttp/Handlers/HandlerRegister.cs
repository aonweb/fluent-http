using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    

    public class HandlerRegister
    {
        private delegate Task HandlerDelegate(HandlerContext context);
        private readonly ISet<IHandler> _callHandlers;
        private readonly ConcurrentDictionary<HandlerType, ConcurrentDictionary<HandlerPriority, ICollection<HandlerDelegate>>> _handlers;

        public HandlerRegister()
        {
            _callHandlers = new HashSet<IHandler>();
            _handlers = new ConcurrentDictionary<HandlerType, ConcurrentDictionary<HandlerPriority, ICollection<HandlerDelegate>>>();
        }

        public async Task OnSending(SendingContext context)
        {
            foreach (var handler in GetHandlers(HandlerType.Sending))
                await handler(context);
        }

        public async Task OnSent(SentContext context)
        {
            foreach (var handler in GetHandlers(HandlerType.Sent))
                await handler(context);
        }

        public async Task OnException(ExceptionContext context)
        {
            foreach (var handler in GetHandlers(HandlerType.Exception))
                await handler(context);
        }

        public HandlerRegister WithHandler(IHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            if (_callHandlers.Contains(handler))
                throw new InvalidOperationException(SR.HanderAlreadyExistsError);

            _callHandlers.Add(handler);

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

        public HandlerRegister WithConfiguration<THandler>(Action<THandler> configure, bool throwOnNotFound = true)
            where THandler : class, IHandler
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

        public HandlerRegister WithSendingHandler(Action<SendingContext> handler)
        {
            return WithSendingHandler(HandlerPriority.Default, handler);
        }

        public HandlerRegister WithSendingHandler(HandlerPriority priority, Action<SendingContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncSendingHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public HandlerRegister WithAsyncSendingHandler(Func<SendingContext, Task> handler)
        {
            return WithAsyncSendingHandler(HandlerPriority.Default, handler);
        }

        public HandlerRegister WithAsyncSendingHandler(HandlerPriority priority, Func<SendingContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            WithHandler(HandlerType.Sending, priority, context => handler((SendingContext)context));

            return this;
        }

        #endregion

        #region Sent

        public HandlerRegister WithSentHandler(Action<SentContext> handler)
        {
            return WithSentHandler(HandlerPriority.Default, handler);
        }

        public HandlerRegister WithSentHandler(HandlerPriority priority, Action<SentContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncSentHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public HandlerRegister WithAsyncSentHandler(Func<SentContext, Task> handler)
        {
            return WithAsyncSentHandler(HandlerPriority.Default, handler);
        }

        public HandlerRegister WithAsyncSentHandler(HandlerPriority priority, Func<SentContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            WithHandler(HandlerType.Sent, priority, context => handler((SentContext)context));

            return this;
        }

        #endregion

        #region Exception

        public HandlerRegister WithExceptionHandler(Action<ExceptionContext> handler)
        {
            return WithExceptionHandler(HandlerPriority.Default, handler);
        }

        public HandlerRegister WithExceptionHandler(HandlerPriority priority, Action<ExceptionContext> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return WithAsyncExceptionHandler(priority, ctx => Task.Run(() => handler(ctx)));
        }

        public HandlerRegister WithAsyncExceptionHandler(Func<ExceptionContext, Task> handler)
        {
            return WithAsyncExceptionHandler(HandlerPriority.Default, handler);
        }

        public HandlerRegister WithAsyncExceptionHandler(HandlerPriority priority, Func<ExceptionContext, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            WithHandler(HandlerType.Exception, priority, context => handler((ExceptionContext)context));

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