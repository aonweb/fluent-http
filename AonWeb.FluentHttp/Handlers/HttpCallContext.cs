using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace AonWeb.FluentHttp.Handlers
{
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

        public Func<HttpContent> ContentFactory { get { return _settings.ContentFactory; } }

        public HttpMethod Method { get { return _settings.Method; } }

        public Uri Uri { get { return _settings.Uri; } }

        public HttpCallHandlerRegister Handler { get { return _settings.Handler; } }

        public HttpCompletionOption CompletionOption { get { return _settings.CompletionOption; } }

        public CancellationTokenSource TokenSource { get { return _settings.TokenSource; } }

        public IDictionary Items { get { return _items; } }

        public IHttpCallBuilder Builder { get { return _builder; } }
    }

    public class HttpCallContext<TResult, TContent, TError>
    {
        private readonly HttpCallBuilderSettings<TResult, TContent, TError> _settings;
        private readonly IHttpCallBuilder<TResult, TContent, TError> _builder;
        private readonly IDictionary _items;

        public HttpCallContext(HttpCallContext<TResult, TContent, TError> context)
            : this(context.Builder, context._settings) { }

        public HttpCallContext(IHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings)
        {
            _builder = builder;
            _settings = settings;
            _items = new HybridDictionary();
        }

        public Func<TContent> ContentFactory { get { return _settings.ContentFactory; } }

        public HttpCallHandlerRegister<TResult, TContent, TError> Handler { get { return _settings.Handler; } }

        public IDictionary Items { get { return _items; } }

        public IHttpCallBuilder<TResult, TContent, TError> Builder { get { return _builder; } }
        public string MediaType { get { return _settings.MediaType; } }
        public Encoding ContentEncoding { get { return _settings.ContentEncoding; } }
    }
}