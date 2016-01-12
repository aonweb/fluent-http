using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Settings
{
    public class HttpBuilderSettings : IHttpBuilderSettings
    {
        protected HttpBuilderSettings()
        {
            UriBuilder = new HttpUriBuilder();
            Items = new Dictionary<string, object>();
            HandlerRegister = new HttpHandlerRegister();
            NormalizedUriQuery = new NormalizedUriQueryCollection();
            Method = HttpMethod.Get;
            CompletionOption = HttpCompletionOption.ResponseContentRead;
            SuppressCancellationErrors = false;
            MediaType = "application/json";
            ContentEncoding = Encoding.UTF8;
            ExceptionFactory = ObjectHelpers.CreateHttpException;
        }

        public HttpBuilderSettings(ICacheSettings cacheSettings, IEnumerable<IHttpHandler> handlers, IEnumerable<IHttpResponseValidator> responseValidators)
            : this()
        {
            CacheSettings = cacheSettings;
            ResponseValidator = new ResponseValidatorCollection(responseValidators);

            if (handlers != null)
            {
                foreach (var handler in handlers)
                    HandlerRegister.WithHandler(handler);
            }
        }

        public IDictionary Items { get; }
        public Type ResultType => typeof(HttpResponseMessage);

        public HttpUriBuilder UriBuilder { get; }
        public Uri Uri => UriBuilder.Uri;
        public NormalizedUriQueryCollection NormalizedUriQuery { get; set; }
        public HttpMethod Method { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public IRecursiveHttpBuilder Builder { get; set; }
        public Func<IHttpBuilderContext, Task<HttpContent>> ContentFactory { get; set; }
        public HttpHandlerRegister HandlerRegister { get; }
        public ResponseValidatorCollection ResponseValidator { get; }
        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }
        public bool SuppressCancellationErrors { get; set; }
        public bool AutoDecompression { get; set; }
        public CancellationToken Token { get; set; }
        public ICacheSettings CacheSettings { get; }
        public ICacheMetadata CacheMetadata => CacheSettings;
        public bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return ResponseValidator.IsValid(response);
        }

        public void Reset()
        {
            Items.Clear();
        }

        public void ValidateSettings()
        {
            if (!UriBuilder.IsSet)
                throw new InvalidOperationException("Uri not set");
        }
        
        public IHttpBuilderSettings GetSettings()
        {
            return this;
        }
    }
}