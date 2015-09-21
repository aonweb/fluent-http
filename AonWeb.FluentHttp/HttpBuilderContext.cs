using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderContext : IHttpBuilderContext
    {
        private readonly IHttpBuilderSettings _settings;

        public HttpBuilderContext(IHttpBuilderContext context)
            : this(context.GetSettings()) { }

        public HttpBuilderContext(IHttpBuilderSettings settings)
        {
            _settings = settings;
        }

        public IDictionary Items => _settings.Items;
        public Uri Uri => _settings.Uri;
        public HttpMethod Method => _settings.Method;
        public string MediaType => _settings.MediaType;
        public Encoding ContentEncoding => _settings.ContentEncoding;
        public HttpCompletionOption CompletionOption => _settings.CompletionOption;
        public bool SuppressCancellationErrors => _settings.SuppressCancellationErrors;
        public bool AutoDecompression => _settings.AutoDecompression;
        public Func<HttpContent> ContentFactory => _settings.ContentFactory;
        public Func<HttpResponseMessage, Exception> ExceptionFactory => _settings.ExceptionFactory;
        public IRecursiveHttpBuilder Builder => _settings.Builder;
        public Type ResultType => _settings.ResultType;
        public HandlerRegister Handler => _settings.Handler;
        public CancellationToken Token => _settings.Token;

        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return _settings.IsSuccessfulResponse(response);
        }


        public void ValidateSettings()
        {
            _settings.ValidateSettings();
        }

        public IHttpBuilderSettings GetSettings()
        {
            return _settings.GetSettings();
        }
    }
}