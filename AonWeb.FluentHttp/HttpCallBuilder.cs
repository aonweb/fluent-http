using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpCallBuilder : IChildHttpCallBuilder, IRecursiveHttpCallBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private readonly HttpCallBuilderSettings _settings;

        protected HttpCallBuilder()
            : this(new HttpClientBuilder()) { }

        protected HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder, HttpCallBuilderDefaults.DefaultHandlerFactory()) { }

        private HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder, params IHttpCallHandler[] defaultHandlers)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;

            foreach (var handler in defaultHandlers)
                _settings.Handler.AddHandler(handler);
        }

        public static IHttpCallBuilder Create()
        {
            return new HttpCallBuilder();
        }

        internal static IChildHttpCallBuilder CreateAsChild()
        {
            return new HttpCallBuilder(new HttpCallBuilderSettings(), new HttpClientBuilder(), new RetryHandler(), new RedirectHandler());
        }

        public static IHttpCallBuilder Create(string baseUri)
        {
            return Create().WithBaseUri(baseUri);
        }

        public static IHttpCallBuilder Create(Uri baseUri)
        {
            return Create().WithBaseUri(baseUri);
        }

        public IAdvancedHttpCallBuilder Advanced
        {
            get { return this; }
        }

        public IHttpCallBuilder WithUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException(SR.ArgumentUriNullOrEmptyError, "uri");

            return WithUri(new Uri(uri));
        }

        public IHttpCallBuilder WithUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            var querystring = uri.ParseQueryString();

            _settings.Uri = uri;
            _settings.QueryString.Clear();

            return WithQueryString(querystring);
        }

        public IHttpCallBuilder WithBaseUri(string uri)
        {
            return WithUri(uri);
        }

        public IHttpCallBuilder WithBaseUri(Uri uri)
        {
            return WithUri(uri);
        }

        public IHttpCallBuilder WithRelativePath(string pathAndQuery)
        {
            if (string.IsNullOrEmpty(pathAndQuery))
                return this;

            var path = pathAndQuery;

            if (!VirtualPathUtility.IsAbsolute(pathAndQuery))
                path = Helper.CombineVirtualPaths(_settings.Path, pathAndQuery);

            return WithPath(path);

        }

        public IHttpCallBuilder WithQueryString(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            _settings.QueryString.Set(name, value);

            _settings.UriBuilderQuery = _settings.QueryString.ToEncodedString();

            return this;
        }

        public IHttpCallBuilder WithQueryString(NameValueCollection values)
        {
            if (values != null)
            {
                for (var i = 0; i < values.Count; i++)
                    _settings.QueryString.Set(values.GetKey(i), values.Get(i));
            }

            _settings.UriBuilderQuery = _settings.QueryString.ToEncodedString();

            return this;
        }

        public IHttpCallBuilder AsGet()
        {
            return WithMethod(HttpMethod.Get);
        }

        public IHttpCallBuilder AsPut()
        {
            return WithMethod(HttpMethod.Put);
        }

        public IHttpCallBuilder AsPost()
        {
            return WithMethod(HttpMethod.Post);
        }

        public IHttpCallBuilder AsDelete()
        {
            return WithMethod(HttpMethod.Delete);
        }

        public IHttpCallBuilder AsPatch()
        {
            return WithMethod(new HttpMethod("PATCH"));
        }

        public IHttpCallBuilder AsHead()
        {
            return WithMethod(HttpMethod.Head);
        }

        public IAdvancedHttpCallBuilder WithScheme(string scheme)
        {
            if (string.IsNullOrEmpty(scheme))
                throw new ArgumentNullException("scheme");

            _settings.Scheme = scheme;

            return this;
        }

        public IAdvancedHttpCallBuilder WithHost(string host)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host");

            _settings.Host = host;

            return this;
        }

        public IAdvancedHttpCallBuilder WithPort(int port)
        {
            _settings.Port = port;

            return this;
        }

        public IAdvancedHttpCallBuilder WithPath(string absolutePathAndQuery)
        {
            if (string.IsNullOrEmpty(absolutePathAndQuery))
                absolutePathAndQuery = "/";

            if (!VirtualPathUtility.IsAbsolute(absolutePathAndQuery))
                throw new ArgumentException(SR.ArgumentPathMustBeAbsoluteError, "absolutePathAndQuery");

            var index = absolutePathAndQuery.IndexOf("?", StringComparison.Ordinal);
            if (index > -1)
            {
                var querystring = HttpUtility.ParseQueryString(absolutePathAndQuery.Substring(index + 1));
                absolutePathAndQuery = absolutePathAndQuery.Substring(0, index);

                WithQueryString(querystring);
            }

            _settings.Path = absolutePathAndQuery;

            return this;
        }

        public IAdvancedHttpCallBuilder WithContentEncoding(Encoding encoding)
        {
            if (encoding != null)
                _settings.ContentEncoding = encoding;

            WithAcceptCharSet(encoding);

            return this;
        }

        public IAdvancedHttpCallBuilder WithMediaType(string mediaType)
        {
            if (mediaType != null)
                _settings.MediaType = mediaType;

            WithAcceptHeader(mediaType);

            return this;
        }

        public IAdvancedHttpCallBuilder WithMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException(SR.ArgumentMethodNullOrEmptyError, "method");

            return WithMethod(new HttpMethod(method.ToUpper()));
        }

        public IAdvancedHttpCallBuilder WithMethod(HttpMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _settings.Method = method;

            return this;
        }

        public IAdvancedHttpCallBuilder WithAcceptHeader(string mediaType)
        {
            return ConfigureClient(c => c.WithHeaders(h => h.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType))));
        }

        public IAdvancedHttpCallBuilder WithAcceptCharSet(Encoding encoding)
        {
            return WithAcceptCharSet(encoding.WebName);
        }

        public IAdvancedHttpCallBuilder WithAcceptCharSet(string charSet)
        {
            return ConfigureClient(c => c.WithHeaders(h => h.AcceptCharset.Add(new StringWithQualityHeaderValue(charSet))));
        }

        public IHttpCallBuilder WithContent(string content)
        {
            return WithContent(content, _settings.ContentEncoding, _settings.MediaType);
        }

        public IHttpCallBuilder WithContent(string content, Encoding encoding)
        {

            return WithContent(content, encoding, _settings.MediaType);
        }

        public IHttpCallBuilder WithContent(string content, Encoding encoding, string mediaType)
        {

            return WithContent(() => content, encoding, mediaType);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFactory)
        {
            return WithContent(contentFactory, _settings.ContentEncoding, _settings.MediaType);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding)
        {
            return WithContent(contentFactory, encoding, _settings.MediaType);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding, string mediaType)
        {
            if (contentFactory == null)
                throw new ArgumentNullException("contentFactory");

            WithContentEncoding(encoding);
            WithMediaType(mediaType);

            return WithContent(() =>
                {
                    var content = contentFactory() ?? string.Empty;

                    return new StringContent(content, encoding, mediaType);
                });


        }

        public IHttpCallBuilder WithContent(Func<HttpContent> contentFactory)
        {
            if (contentFactory == null)
                throw new ArgumentNullException("contentFactory");

            _settings.ContentFactory = contentFactory;

            return this;
        }

        public IAdvancedHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            if (configuration != null)
                configuration(_clientBuilder);

            return this;
        }

        public IAdvancedHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration)
        {
            return ConfigureHandler(configuration);
        }

        public IAdvancedHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration)
        {
            return ConfigureHandler(configuration);
        }

        public IAdvancedHttpCallBuilder WithHandler(IHttpCallHandler handler)
        {
            _settings.Handler.AddHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure) where THandler : class, IHttpCallHandler
        {
            _settings.Handler.ConfigureHandler(configure);

            return this;
        }

        public IAdvancedHttpCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure) where THandler : class, IHttpCallHandler
        {
            _settings.Handler.ConfigureHandler(configure, false);

            return this;
        }

        public IAdvancedHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            _settings.SuccessfulResponseValidators.Add(validator);

            return this;
        }

        public IAdvancedHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory)
        {
            _settings.ExceptionFactory = factory;

            return this;
        }

        public IAdvancedHttpCallBuilder WithCaching(bool enabled = true)
        {
            ConfigureHandler<CacheHandler>(handler => handler.WithCaching(enabled));

            return this;
        }

        public IAdvancedHttpCallBuilder WithNoCache(bool nocache = true)
        {
            _clientBuilder.WithNoCache(nocache);

            return this;
        }

        public IAdvancedHttpCallBuilder WithDependentUri(Uri uri)
        {
            return TryConfigureHandler<CacheHandler>(h => h.WithDependentUri(uri));
        }

        public IAdvancedHttpCallBuilder WithDependentUris(IEnumerable<Uri> uris)
        {
            return TryConfigureHandler<CacheHandler>(h => h.WithDependentUris(uris));
        }

        public IAdvancedHttpCallBuilder OnSending(Action<HttpSendingContext> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSent(Action<HttpSentContext> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnException(Action<HttpExceptionContext> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder WithAutoDecompression(bool enabled = true)
        {
            _settings.AutoDecompression = true;

            _clientBuilder.WithDecompressionMethods(enabled
                                                        ? DecompressionMethods.GZip | DecompressionMethods.Deflate
                                                        : DecompressionMethods.None);

            return this;
        }

        public IAdvancedHttpCallBuilder WithSuppressCancellationErrors(bool suppress = true)
        {
            _settings.SuppressCancellationErrors = suppress;

            return this;
        }

        public IAdvancedHttpCallBuilder WithTimeout(TimeSpan? timeout)
        {
            return ConfigureClient(c => c.WithTimeout(timeout));
        }

        public IHttpCallBuilder CancelRequest()
        {
            _settings.TokenSource.Cancel();

            return this;
        }

        public async Task<HttpResponseMessage> ResultAsync()
        {
            HttpResponseMessage response = null;

            try
            {
                response = await RecursiveResultAsync();
            }
            catch (TaskCanceledException)
            {
                if (!_settings.SuppressCancellationErrors)
                    throw;
            }

            _settings.Reset();

            return response;
        }

        public async Task<HttpResponseMessage> RecursiveResultAsync()
        {
            var context = new HttpCallContext(this, _settings);

            using (var request = CreateRequest(context))
            {
                return await ResultFromRequestAsync(context, request);
            }
        }

        public HttpRequestMessage CreateRequest()
        {
            var context = new HttpCallContext(this, _settings);

            return CreateRequest(context);
        }

        public async Task<HttpResponseMessage> ResultFromRequestAsync(HttpRequestMessage request)
        {
            var context = new HttpCallContext(this, _settings);
            return await ResultFromRequestAsync(context, request).ConfigureAwait(false);
        }

        public void ApplySettings(TypedHttpCallBuilderSettings settings)
        {
            _settings.ApplySettings(settings);
        }

        private HttpRequestMessage CreateRequest(HttpCallContext context)
        {
            context.ValidateSettings();

            var request = new HttpRequestMessage(context.Method, context.Uri);

            if (context.ContentFactory != null)
                request.Content = context.ContentFactory();

            _clientBuilder.ApplyRequestHeaders(request);

            // if we haven't added an accept header, add a default
            if (!string.IsNullOrWhiteSpace(context.MediaType))
                request.Headers.Accept.AddDistinct(h => h.MediaType, context.MediaType);


            //if we haven't added a char-set, add a default
            if (context.AutoDecompression)
            {
                request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "gzip");
                request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "deflate");
                request.Headers.AcceptEncoding.AddDistinct(h => h.Value, "identity");
            }

            return request;
        }

        private async Task<HttpResponseMessage> ResultFromRequestAsync(HttpCallContext context, HttpRequestMessage request)
        {
            ExceptionDispatchInfo capturedException = null;
            HttpResponseMessage response = null;

            try
            {
                using (var client = _clientBuilder.Create())
                {
                    var sendingContext = new HttpSendingContext(context, request);

                    await context.Handler.OnSending(sendingContext);

                    if (sendingContext.Response != null)
                    {
                        response = sendingContext.Response;
                    }
                    else
                    {
                        response = await client.SendAsync(request, context.CompletionOption, context.TokenSource.Token);
                    }

                    if (!context.IsSuccessfulResponse(response))
                        if (_settings.ExceptionFactory != null)
                            throw _settings.ExceptionFactory(response);

                    var sentContext = new HttpSentContext(context, response);

                    await context.Handler.OnSent(sentContext);

                    response = sentContext.Response;
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                var exContext = new HttpExceptionContext(context, capturedException.SourceException);

                await context.Handler.OnException(exContext);

                if (!exContext.ExceptionHandled)
                    capturedException.Throw();
            }

            return response;
        }
    }
}