using System.Collections;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AonWeb.FluentHttp.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading;

namespace AonWeb.FluentHttp
{
    public class HttpCallBuilderSettings
    {
        private UriBuilder _uriBuilder;
        private readonly IDictionary _items;

        public HttpCallBuilderSettings()
        {
            _uriBuilder = new UriBuilder();
            _items = new HybridDictionary();

            Method = HttpCallBuilderDefaults.DefaultHttpMethod;
            CompletionOption = HttpCallBuilderDefaults.DefaultCompletionOption;
            TokenSource = new CancellationTokenSource();
            Handler = new HttpCallHandlerRegister();
            
            QueryString = new NameValueCollection();
            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>();
            MediaType = HttpCallBuilderDefaults.DefaultMediaType;
            ContentEncoding = HttpCallBuilderDefaults.DefaultContentEncoding;

        }

        public IDictionary Items { get { return _items; } }
        public Uri Uri { get { return _uriBuilder.Uri; } set { _uriBuilder = new UriBuilder(value); } }
        public string Scheme { get { return _uriBuilder.Scheme; } set { _uriBuilder.Scheme = value; } }
        public string Host { get { return _uriBuilder.Host; } set { _uriBuilder.Host = value; } }
        public int Port { get { return _uriBuilder.Port; } set { _uriBuilder.Port = value; } }
        public string Path { get { return _uriBuilder.Path; } set { _uriBuilder.Path = value; } }
        public string UriBuilderQuery { set { _uriBuilder.Query = value; } }
        public NameValueCollection QueryString { get; set; }
        public HttpMethod Method { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Func<HttpContent> ContentFactory { get; set; }
        public HttpCallHandlerRegister Handler { get; private set; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; private set; }
        public Func<HttpResponseMessage, Exception> ExceptionFactory { get; set; }

        public void Reset()
        {
            _items.Clear();
        }

        public void ValidateSettings()
        {
            if (Uri == null)
                throw new InvalidOperationException("Uri not set");
        }
    }

    public class HttpCallBuilderSettings<TResult, TContent, TError>
    {
        private readonly IDictionary _items;

        public HttpCallBuilderSettings()
        {
            _items = new HybridDictionary();
            MediaType = HttpCallBuilderDefaults.DefaultMediaType;
            ContentEncoding = HttpCallBuilderDefaults.DefaultContentEncoding;
            MediaTypeFormatters = new MediaTypeFormatterCollection(HttpCallBuilderDefaults.DefaultMediaTypeFormatters);

            Handler = new HttpCallHandlerRegister<TResult, TContent, TError>();

            SuccessfulResponseValidators = new List<Func<HttpResponseMessage, bool>>
            {
                HttpCallBuilderDefaults.DefaultSuccessfulResponseValidator
            };

            ExceptionFactory = HttpCallBuilderDefaults.DefaultExceptionFactory;
            DefaultResultFactory = HttpCallBuilderDefaults.DefaultResultFactory<TResult>;
        }

        public IDictionary Items { get { return _items; } }
        public Func<TContent> ContentFactory { get; set; }
        public string MediaType { get; set; }
        public Encoding ContentEncoding { get; set; }
        public MediaTypeFormatterCollection MediaTypeFormatters { get; set; }
        public HttpCallHandlerRegister<TResult, TContent, TError> Handler { get; private set; }
        public IList<Func<HttpResponseMessage, bool>> SuccessfulResponseValidators { get; private set; }
        public Func<HttpErrorContext<TResult, TContent, TError>, Exception> ExceptionFactory { get; set; }

        public Func<TResult> DefaultResultFactory { get; set; }

        public void Reset()
        {
            _items.Clear();
        }
    }
}