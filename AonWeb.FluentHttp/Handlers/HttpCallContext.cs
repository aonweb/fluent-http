using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpCallContext
    {
        IDictionary Items { get; }
    }

    public class HttpCallContext : IHttpCallContext
    {
        private readonly HttpCallBuilderSettings _settings;
        private readonly IRecursiveHttpCallBuilder _builder;

        public HttpCallContext(HttpCallContext context)
            : this(context.Builder, context._settings) { }

        public HttpCallContext(IRecursiveHttpCallBuilder builder, HttpCallBuilderSettings settings)
        {
            _builder = builder;
            _settings = settings;
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

        public IDictionary Items { get { return _settings.Items; } }

        public IRecursiveHttpCallBuilder Builder { get { return _builder; } }

        public string MediaType { get { return _settings.MediaType; } }
        public Encoding ContentEncoding { get { return _settings.ContentEncoding; } }
        public bool AutoDecompression { get { return _settings.AutoDecompression; } }
    }

    public class HttpCallContext<TResult, TContent, TError> : IHttpCallContext
    {
        private readonly HttpCallBuilderSettings<TResult, TContent, TError> _settings;
        private readonly IRecursiveHttpCallBuilder<TResult, TContent, TError> _builder;

        public HttpCallContext(HttpCallContext<TResult, TContent, TError> context)
            : this(context.Builder, context._settings) { }

        public HttpCallContext(IRecursiveHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings)
        {
            _builder = builder;
            _settings = settings;
        }

        public Func<TContent> ContentFactory { get { return _settings.ContentFactory; } }

        public HttpCallHandlerRegister<TResult, TContent, TError> Handler { get { return _settings.Handler; } }

        public IDictionary Items { get { return _settings.Items; } }

        public IRecursiveHttpCallBuilder<TResult, TContent, TError> Builder { get { return _builder; } }
        public string MediaType { get { return _settings.MediaType; } }
        public Encoding ContentEncoding { get { return _settings.ContentEncoding; } }
        public MediaTypeFormatterCollection MediaTypeFormatters { get { return _settings.MediaTypeFormatters; } }

        public bool DeserializeResult
        {
            get
            {
                return _settings.DeserializeResult;
            }
        }
    }
}