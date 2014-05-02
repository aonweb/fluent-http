using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public enum HttpCallHandlerPriority
    {
        First = 1,
        High = 2,
        Default = 3,
        Low = 4,
        Last = 5,
        None = int.MaxValue
    }

    public enum HttpCallHandlerType
    {
        Sending,
        Sent,
        Result,
        Error,
        Exception,
    }

    public interface IHttpCallHandler<in TContext>
    {
        Task Handle(TContext context);

        HttpCallHandlerPriority Priority { get; }
        HttpCallHandlerType HandlerType { get; }
    }

    public class HttpCallHandlerFunc : IHttpCallHandler<HttpCallContext>
    {
        private readonly Func<HttpCallContext, Task> _handler;

        public HttpCallHandlerFunc(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler)
        {
            HandlerType = handlerType;
            Priority = priority;
            _handler = handler;
        }

        public Task Handle(HttpCallContext context)
        {
            return _handler(context);
        }

        public HttpCallHandlerPriority Priority { get; private set; }

        public HttpCallHandlerType HandlerType { get; private set; }
    }

    public class HttpCallHandlerFunc<TResult, TContent, TError> : IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>
    {
        private readonly Func<HttpCallContext<TResult, TContent, TError>, Task> _handler;

        public HttpCallHandlerFunc(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            HandlerType = handlerType;
            Priority = priority;
            _handler = handler;
        }

        public Task Handle(HttpCallContext<TResult, TContent, TError> context)
        {
            return _handler(context);
        }

        public HttpCallHandlerPriority Priority { get; private set; }

        public HttpCallHandlerType HandlerType { get; private set; }
    }

    public class HttpCallContext<TResult, TContent, TError>
    {
        private readonly HttpCallBuilderSettings<TResult, TContent, TError> _settings;
        private readonly IHttpCallBuilder<TResult, TContent, TError> _builder;
        private readonly IDictionary<string, object> _items;

        private TResult _result;

        public HttpCallContext(HttpCallContext<TResult, TContent, TError> context)
            : this(context.Builder, context._settings) { }

        public HttpCallContext(IHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings)
        {
            _builder = builder;
            _settings = settings;
            _items = new ConcurrentDictionary<string, object>();
        }

        public HttpResponseMessage Response { get; set; }
        public TError Error { get; set; }

        public TResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                IsResultSet = true;

            }
        }

        public IDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>>> Handlers { get { return _settings.Handlers; } }

        public IDictionary<string, object> Items { get { return _items; } }

        public IHttpCallBuilder<TResult, TContent, TError> Builder { get { return _builder; } }

        public bool IsResultSet { get; set; }
    }

    public class HttpCallContext
    {
        private readonly HttpCallBuilderSettings _settings;
        private readonly IHttpCallBuilder _builder;
        private readonly IDictionary _items;

        public HttpCallContext(HttpCallContext context)
            : this(context.Builder, context._settings) { }

        public HttpCallContext(IHttpCallBuilder builder, HttpCallBuilderSettings settings)
        {
            _builder = builder;
            _settings = settings;
            _items = new HybridDictionary();
        }

        public void ValidateSettings()
        {
            _settings.ValidateSettings();
        }

        public Func<HttpContent> Content { get { return _settings.Content; } }

        public HttpMethod Method { get { return _settings.Method; } }

        public Uri Uri { get { return _settings.Uri; } }

        public IDictionary<HttpCallHandlerType, ISet<IHttpCallHandler<HttpCallContext>>> Handlers { get { return _settings.Handlers; } }

        public HttpCompletionOption CompletionOption { get { return _settings.CompletionOption; } }

        public CancellationTokenSource TokenSource { get { return _settings.TokenSource; } }

        public HttpResponseMessage Response { get; set; }

        public IDictionary Items { get { return _items; } }

        public IHttpCallBuilder Builder { get { return _builder; } }
    }
}