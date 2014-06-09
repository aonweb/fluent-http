using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace AonWeb.FluentHttp.Handlers
{
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

        public Type ResultType
        {
            get { return typeof(HttpResponseMessage); }
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

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }
    }
}