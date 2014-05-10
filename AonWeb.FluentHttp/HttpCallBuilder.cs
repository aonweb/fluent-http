using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
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
    public class HttpCallBuilder<TResult, TContent, TError> : IRecursiveHttpCallBuilder<TResult, TContent, TError>, IAdvancedHttpCallBuilder<TResult, TContent, TError>
    {
        private readonly HttpCallBuilderSettings<TResult, TContent, TError> _settings;
        private readonly IChildHttpCallBuilder _innerBuilder;

        protected HttpCallBuilder()
            : this(new HttpCallBuilderSettings<TResult, TContent, TError>(), HttpCallBuilder.CreateAsChild()) { }

        private HttpCallBuilder(HttpCallBuilderSettings<TResult, TContent, TError> settings, IChildHttpCallBuilder builder)
            : this(settings, builder, new CacheHandler<TResult, TContent, TError>()) { }

        private HttpCallBuilder(HttpCallBuilderSettings<TResult, TContent, TError> settings, IChildHttpCallBuilder builder, params IHttpCallHandler<TResult, TContent, TError>[] defaultHandlers)
        {
            _settings = settings;
            _innerBuilder = builder;

            foreach (var handler in defaultHandlers)
                _settings.Handler.AddHandler(handler);
        }

        public static IHttpCallBuilder<TResult, TContent, TError> Create()
        {
            return new HttpCallBuilder<TResult, TContent, TError>();
        }

        public static IHttpCallBuilder<TResult, TContent, TError> Create(string baseUri)
        {
            return Create().WithBaseUri(baseUri);
        }

        public static IHttpCallBuilder<TResult, TContent, TError> Create(Uri baseUri)
        {
            return Create().WithBaseUri(baseUri);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithUri(string uri)
        {
            _innerBuilder.WithUri(uri);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithUri(Uri uri)
        {
            _innerBuilder.WithUri(uri);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithBaseUri(string uri)
        {
            _innerBuilder.WithBaseUri(uri);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithBaseUri(Uri uri)
        {
            _innerBuilder.WithBaseUri(uri);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithRelativePath(string pathAndQuery)
        {
            _innerBuilder.WithRelativePath(pathAndQuery);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value)
        {
            _innerBuilder.WithQueryString(name, value);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithQueryString(NameValueCollection values)
        {
            _innerBuilder.WithQueryString(values);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> AsGet()
        {
            return WithMethod(HttpMethod.Get);
        }

        public IHttpCallBuilder<TResult, TContent, TError> AsPut()
        {
            return WithMethod(HttpMethod.Put);
        }

        public IHttpCallBuilder<TResult, TContent, TError> AsPost()
        {
            return WithMethod(HttpMethod.Post);
        }

        public IHttpCallBuilder<TResult, TContent, TError> AsDelete()
        {
            return WithMethod(HttpMethod.Delete);
        }

        public IHttpCallBuilder<TResult, TContent, TError> AsPatch()
        {
            return WithMethod(new HttpMethod("PATCH"));
        }

        public IHttpCallBuilder<TResult, TContent, TError> AsHead()
        {
            return WithMethod(HttpMethod.Head);
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithScheme(string scheme)
        {
            _innerBuilder.WithScheme(scheme);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithHost(string host)
        {
            _innerBuilder.WithHost(host);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithPort(int port)
        {
            _innerBuilder.WithPort(port);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithPath(string absolutePathAndQuery)
        {
            _innerBuilder.WithPath(absolutePathAndQuery);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithEncoding(Encoding encoding)
        {
            _settings.ContentEncoding = encoding;

            _innerBuilder.WithEncoding(encoding);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMediaType(string mediaType)
        {
            _settings.MediaType = mediaType;

            _innerBuilder.WithMediaType(mediaType);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMethod(string method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithAcceptHeader(string mediaType)
        {
            _innerBuilder.WithAcceptHeader(mediaType);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithAcceptCharSet(Encoding encoding)
        {
            _innerBuilder.WithAcceptCharSet(encoding);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithAcceptCharSet(string charSet)
        {
            _innerBuilder.WithAcceptCharSet(charSet);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content)
        {

            return WithContent(content, _settings.ContentEncoding, _settings.MediaType);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding)
        {

            return WithContent(content, encoding, _settings.MediaType);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType)
        {

            return WithContent(() => content, encoding, mediaType);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory)
        {
            return WithContent(contentFactory, _settings.ContentEncoding, _settings.MediaType);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding)
        {
            return WithContent(contentFactory, encoding, _settings.MediaType);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding, string mediaType)
        {
            if (contentFactory == null)
                throw new ArgumentNullException("contentFactory");

            _settings.ContentFactory = contentFactory;

            WithEncoding(encoding);
            WithMediaType(mediaType);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result)
        {
            return WithDefaultResult(() => result);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFactory)
        {
            _settings.DefaultResultFactory = resultFactory;

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.ConfigureClient(configuration);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMediaTypeFormatter(MediaTypeFormatter formatter)
        {
            _settings.MediaTypeFormatters.Add(formatter);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure) where TFormatter : MediaTypeFormatter
        {
            if (configure == null)
                throw new ArgumentNullException("configure");

            var formatter = _settings.MediaTypeFormatters.OfType<TFormatter>().FirstOrDefault();

            if (formatter != null)
                configure(formatter);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler)
        {
            _settings.Handler.AddHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>
        {
            _settings.Handler.ConfigureHandler(configure);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> TryConfigureHandler<THandler>(Action<THandler> configure) where THandler : class, IHttpCallHandler<TResult, TContent, TError>
        {
            _settings.Handler.ConfigureHandler(configure, false);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            _settings.SuccessfulResponseValidators.Add(validator);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory)
        {
            _settings.ExceptionFactory = factory;

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithCaching(bool enabled = true)
        {

            ConfigureHandler<CacheHandler<TResult, TContent, TError>>(handler => handler.WithCaching(enabled));

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithNoCache(bool nocache = true)
        {
            _innerBuilder.WithNoCache(nocache);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithDependentUri(string uri)
        {
            return TryConfigureHandler<CacheHandler<TResult, TContent, TError>>(h => h.WithDependentUri(uri));
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithDependentUris(IEnumerable<string> uris)
        {
            return TryConfigureHandler<CacheHandler<TResult, TContent, TError>>(h => h.WithDependentUris(uris));
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddResultHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddResultHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddResultHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddResultHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddErrorHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddErrorHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddErrorHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddErrorHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithSuppressCancellationErrors(bool suppress = true)
        {
            _innerBuilder.WithSuppressCancellationErrors(suppress);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> WithTimeout(TimeSpan? timeout)
        {
            _innerBuilder.WithTimeout(timeout);

            return this;
        }

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> Advanced
        {
            get
            {
                return this;
            }
        }

        public IHttpCallBuilder<TResult, TContent, TError> CancelRequest()
        {
            _innerBuilder.CancelRequest();

            return this;
        }

        public TResult Result()
        {
            return Task.Run(() => ResultAsync()).Result;
        }

        public async Task<TResult> ResultAsync()
        {
            var result = await RecursiveResultAsync();

            _settings.Reset();

            return result;
        }

        public async Task<TResult> RecursiveResultAsync()
        {
            return await ResultAsync(new HttpCallContext<TResult, TContent, TError>(this, _settings));
        }

        private async Task<TResult> ResultAsync(HttpCallContext<TResult, TContent, TError> context)
        {
            HttpResponseMessage response = null;
            ExceptionDispatchInfo capturedException = null;
            try
            {
                //build content before creating request
                TContent content = default(TContent);
                if (context.ContentFactory != null)
                {
                    content = context.ContentFactory();

                    var httpContent = await CreateContent(content, context);

                    _innerBuilder.WithContent(() => httpContent);

                }

                using (var request = _innerBuilder.CreateRequest())
                {
                    var sendingContext = new HttpSendingContext<TResult, TContent, TError>(context, request);

                    if (context.ContentFactory != null)
                        sendingContext.Content = content;

                    await context.Handler.OnSending(sendingContext);

                    if (sendingContext.IsResultSet)
                        return sendingContext.Result;

                    response = await _innerBuilder.ResultFromRequestAsync(request);
                }

                if (!IsSuccessfulResponse(response))
                {
                    var error = await response.Content.ReadAsAsync<TError>(context.MediaTypeFormatters);

                    var errorCtx = new HttpErrorContext<TResult, TContent, TError>(context, error, response);

                    await context.Handler.OnError(errorCtx);

                    if (!errorCtx.ErrorHandled) if (_settings.ExceptionFactory != null) throw _settings.ExceptionFactory(errorCtx);
                }
                else
                {
                    var sentContext = new HttpSentContext<TResult, TContent, TError>(context, response);

                    await context.Handler.OnSent(sentContext);

                    TResult result;

                    if (sentContext.IsResultSet)
                    {
                        result = sentContext.Result;
                    }
                    else
                    {
                        result = await response.Content.ReadAsAsync<TResult>(context.MediaTypeFormatters);
                    }

                    var resultContext = new HttpResultContext<TResult, TContent, TError>(context, result, response);

                    await context.Handler.OnResult(resultContext);

                    return resultContext.Result;
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                Helper.DisposeResponse(response);
            }

            if (capturedException != null)
            {
                var exCtx = new HttpExceptionContext<TResult, TContent, TError>(context, capturedException.SourceException);

                context.Handler.OnException(exCtx).Wait();

                if (!exCtx.ExceptionHandled)
                    capturedException.Throw();
            }

            return _settings.DefaultResultFactory();
        }

        private bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }

        private async Task<HttpContent> CreateContent<T>(T value, HttpCallContext<TResult, TContent, TError> context)
        {
            var type = typeof(T);
            var mediaType = context.MediaType;
            var header = new MediaTypeHeaderValue(mediaType);
            var formatter = context.MediaTypeFormatters.FindWriter(type, header);

            if (formatter == null)
                throw new ArgumentException(string.Format(SR.NoFormatterForMimeTypeErrorFormat, mediaType));

            HttpContent content;
            using (var stream = new MemoryStream())
            {
                await formatter.WriteToStreamAsync(type, value, stream, null, null);

                content = new ByteArrayContent(stream.ToArray());
            }

            formatter.SetDefaultContentHeaders(type, content.Headers, header);

            return content;
        }
    }

    public class HttpCallBuilder : IChildHttpCallBuilder, IRecursiveHttpCallBuilder, IAdvancedHttpCallBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private readonly HttpCallBuilderSettings _settings;

        protected HttpCallBuilder()
            : this(new HttpClientBuilder()) { }

        protected HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder, new RetryHandler(), new RedirectHandler(), new CacheHandler()) { }

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

        public IAdvancedHttpCallBuilder WithEncoding(Encoding encoding)
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

            WithEncoding(encoding);
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

        public IAdvancedHttpCallBuilder WithDependentUri(string uri)
        {
            return TryConfigureHandler<CacheHandler>(h => h.WithDependentUri(uri));
        }

        public IAdvancedHttpCallBuilder WithDependentUris(IEnumerable<string> uris)
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

        public HttpResponseMessage Result()
        {
            return ResultAsync().Result;
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
                return await ResultFromRequestAsync(context, request).ConfigureAwait(false);
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
            return await ResultFromRequestAsync(context, request);
        }

        private HttpRequestMessage CreateRequest(HttpCallContext context)
        {
            context.ValidateSettings();

            var request = new HttpRequestMessage(context.Method, context.Uri);

            if (context.ContentFactory != null)
                request.Content = context.ContentFactory();

            if (request.Headers.Accept.Count == 0 && !string.IsNullOrWhiteSpace(context.MediaType))
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(context.MediaType));

            _clientBuilder.ApplyRequestHeaders(request);

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

                    if (!IsSuccessfulResponse(response))
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

        private bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }
    }
}
