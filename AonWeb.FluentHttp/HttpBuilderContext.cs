using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderContext : IHttpBuilderContext
    {
        public HttpBuilderContext(IHttpBuilderSettings settings)
            :this((IHttpBuilderContext)settings)
        {
            settings.ValidateSettings();

            CacheMetadata = new CacheMetadata(settings.CacheSettings);
        }

        public HttpBuilderContext(IHttpBuilderContext context)
        {
            Items = context.Items;
            ResultType = context.ResultType;
            Builder = context.Builder;
            SuppressCancellationErrors = context.SuppressCancellationErrors;
            Uri = context.Uri;
            Method = context.Method;
            MediaType = context.MediaType;
            ContentEncoding = context.ContentEncoding;
            CompletionOption = context.CompletionOption;
            AutoDecompression = context.AutoDecompression;
            ContentFactory = context.ContentFactory;
            ExceptionFactory = context.ExceptionFactory;
            HandlerRegister = context.HandlerRegister;
            Token = context.Token;
            CacheMetadata = context.CacheMetadata;
            ResponseValidator = context.ResponseValidator;
        }

        public IDictionary Items { get; }
        public Type ResultType { get; }
        public IRecursiveHttpBuilder Builder { get; }
        public bool SuppressCancellationErrors { get; }
        public Uri Uri { get; }
        public HttpMethod Method { get; }
        public string MediaType { get; }
        public Encoding ContentEncoding { get; }
        public HttpCompletionOption CompletionOption { get; }
        public bool AutoDecompression { get; }
        public Func<IHttpBuilderContext, Task<HttpContent>> ContentFactory { get; }
        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; }
        public HttpHandlerRegister HandlerRegister { get; }
        public CancellationToken Token { get; }
        public ResponseValidatorCollection ResponseValidator { get; }
        public ICacheMetadata CacheMetadata { get; }
        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return ResponseValidator.IsValid(response);
        }
    }
}