using System;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.Fluent.Http.Client;
using AonWeb.Fluent.Http.Handlers;
using AonWeb.Fluent.Http.Serialization;

namespace AonWeb.Fluent.Http
{
    public class HttpCallBuilder<TResult, TContent, TError> : IAdvancedHttpCallBuilder<TResult, TContent, TError>
    {
        private readonly HttpCallBuilderSettings<TResult> _settings;
        private readonly IHttpCallBuilder _innerBuilder;
        private readonly ISerializerFactory<TResult> _serializerFactory;
        private readonly IErrorHandler<TError> _errorHandler;

        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings<TResult>(), new HttpCallBuilder()) { }

        internal HttpCallBuilder(IHttpCallBuilder builder)
            : this(new HttpCallBuilderSettings<TResult>(), builder) { }

        internal HttpCallBuilder(HttpCallBuilderSettings<TResult> settings, IHttpCallBuilder builder)
            : this(settings, builder, new SerializerFactory<TResult>(), new ErrorHandler<TError>()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings<TResult> settings, IHttpCallBuilder builder, IErrorHandler<TError> errorHandler)
            : this(settings, builder, new SerializerFactory<TResult>(), errorHandler) { }

        internal HttpCallBuilder(HttpCallBuilderSettings<TResult> settings, IHttpCallBuilder builder, ISerializerFactory<TResult> serializerFactory)
            : this(settings, builder, serializerFactory, new ErrorHandler<TError>()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings<TResult> settings, IHttpCallBuilder builder, ISerializerFactory<TResult> serializerFactory, IErrorHandler<TError> errorHandler)
        {
            _settings = settings;
            _innerBuilder = builder;
            _serializerFactory = serializerFactory;
            _errorHandler = errorHandler;
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

        public IHttpCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value)
        {
            _innerBuilder.WithQueryString(name, value);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithMethod(string method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content)
        {

            return WithContent(content, null, null);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding)
        {

            return WithContent(content, encoding, null);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType)
        {

            return WithContent(() => content, encoding, mediaType);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc)
        {
            return WithContent(contentFunc, null, null);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding)
        {
            return WithContent(contentFunc, encoding, null);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding, string mediaType)
        {
            _innerBuilder.WithContent(() =>
            {
                var serializer = _serializerFactory.GetSerializer(mediaType);

                var content = contentFunc();

                var contentString = serializer.Serialize(content).Result;

                return contentString;

            }, encoding, mediaType);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result)
        {
            return WithDefaultResult(() => result);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFunc)
        {
            _settings.DefaultResult = resultFunc;

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration)
        {
            _innerBuilder.Advanced.ConfigureClient(configuration);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.Advanced.ConfigureClient(configuration);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureRedirect(Action<IRedirectHandler> configuration)
        {
            _innerBuilder.Advanced.ConfigureRedirect(configuration);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithRedirectHandler(Action<HttpRedirectContext> handler)
        {
            _innerBuilder.Advanced.WithRedirectHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureErrorHandling(Action<IErrorHandler<TError>> configuration)
        {

            if (configuration != null)
                configuration(_errorHandler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler)
        {
            _errorHandler.WithErrorHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithExceptionHandler(Action<HttpExceptionContext> handler)
        {
            _errorHandler.WithExceptionHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithNoCache()
        {
            _innerBuilder.Advanced.WithNoCache();

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> CancelRequest()
        {
            _innerBuilder.CancelRequest();

            return this;
        }

        public TResult Result()
        {
            return ResultAsync().Result;
        }

        public async Task<TResult> ResultAsync()
        {
            // TODO: caching
            // TODO: circuit breaker
            // TODO: logging
            try
            {
                var response = await _innerBuilder.ResultAsync();

                var serializer = _serializerFactory.GetSerializer(response);

                var result = await serializer.Deserialize(response.Content);

                return result;
            }
            catch (Exception ex)
            {
                var ctx = _errorHandler.HandleException(ex);

                if (!ctx.ExceptionHandled)
                    throw;
            }

            return _settings.DefaultResult();
        }

        #region Transforms

        public IAdvancedHttpCallBuilder<TResult, TContent, TError> Advanced
        {
            get
            {
                return this;
            }
        }

        public IHttpCallBuilder<T, TContent, TError> WithResultOfType<T>()
        {
            return new HttpCallBuilder<T, TContent, TError>(new HttpCallBuilderSettings<T>(), _innerBuilder, _errorHandler);
        }

        public IHttpCallBuilder<TResult, T, TError> WithContentOfType<T>()
        {
            return new HttpCallBuilder<TResult, T, TError>(_settings, _innerBuilder, _serializerFactory, _errorHandler);
        }

        public IHttpCallBuilder<TResult, TContent, T> WithErrorsOfType<T>()
        {
            return new HttpCallBuilder<TResult, TContent, T>(_settings, _innerBuilder, _serializerFactory);
        }

        #endregion
    }

    public class HttpCallBuilder : IAdvancedHttpCallBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private readonly IRedirectHandler _redirectionHandler;
        private readonly IRetryHandler _retryHandler;
        private readonly HttpCallBuilderSettings _settings;

        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings(), new HttpClientBuilder(), new RetryHandler(), new RedirectHandler()) { }

        public HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder, new RetryHandler(),  new RedirectHandler()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder, IRetryHandler retryHandler, IRedirectHandler redirectionHandler)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;
            _retryHandler = retryHandler;
            _redirectionHandler = redirectionHandler;
        }

        public IAdvancedHttpCallBuilder Advanced
        {
            get { return this; }
        }

        internal HttpCallBuilderSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public IHttpCallBuilder WithUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException(SR.ArgumentUriNullOrEmpty, "uri");

            return WithUri(new Uri(uri));
        }

        public IHttpCallBuilder WithUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            _settings.Uri = uri;

            return this;
        }

        public IHttpCallBuilder WithQueryString(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            // TODO: should be delay execution to allow uri to set after?

            _settings.Uri = Utils.AppendToQueryString(_settings.Uri, name, value);

            return this;
        }

        public IHttpCallBuilder WithMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException(SR.ArgumentMethodNullOrEmpty, "method");

            return WithMethod(new HttpMethod(method));
        }

        public IHttpCallBuilder WithMethod(HttpMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _settings.Method = method;

            return this;
        }

        public IHttpCallBuilder WithContent(string content)
        {

            return WithContent(content, null, null);
        }

        public IHttpCallBuilder WithContent(string content, Encoding encoding)
        {

            return WithContent(content, encoding, null);
        }

        public IHttpCallBuilder WithContent(string content, Encoding encoding, string mediaType)
        {

            return WithContent(() => content, encoding, mediaType);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFunc)
        {
            return WithContent(contentFunc, null, null);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFunc, Encoding encoding)
        {
            return WithContent(contentFunc, encoding, null);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFunc, Encoding encoding, string mediaType)
        {
            return WithContent(() =>
            {
                var content = contentFunc();
                return new StringContent(content, encoding, mediaType);
            });
        }

        public IHttpCallBuilder WithContent(Func<HttpContent> contentFunc)
        {
            if (contentFunc == null)
                throw new ArgumentNullException("contentFunc");

            _settings.Content = contentFunc;

            return this;
        }

        public IHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration)
        {
            _clientBuilder.Configure(configuration);

            return this;
        }

        public IHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            if (configuration != null)
                configuration(_clientBuilder);

            return this;
        }

        public IHttpCallBuilder ConfigureRetries(Action<IRetryHandler> configuration)
        {
            if (configuration != null)
                configuration(_retryHandler);

            return this;
        }

        public IHttpCallBuilder WithRetryHandler(Action<HttpRetryContext> handler)
        {
            _retryHandler.WithHandler(handler);

            return this;
        }

        public IHttpCallBuilder ConfigureRedirect(Action<IRedirectHandler> configuration)
        {
            if (configuration != null)
                configuration(_redirectionHandler);

            return this;
        }

        public IHttpCallBuilder WithRedirectHandler(Action<HttpRedirectContext> handler)
        {
            _redirectionHandler.WithHandler(handler);

            return this;
        }

        public IHttpCallBuilder WithNoCache()
        {
            _clientBuilder.WithCachePolicy(RequestCacheLevel.NoCacheNoStore);

            return this;
        }

        public IHttpCallBuilder CancelRequest()
        {
            _settings.TokenSource.Cancel();

            return this;
        }

        #region IHttpCallBuilder<TResult,TContent, TError> Methods

        public IHttpCallBuilder<T, string, string> WithResultOfType<T>()
        {
            return new HttpCallBuilder<T, string, string>(this);
        }

        public IHttpCallBuilder<HttpResponseMessage, T, string> WithContentOfType<T>()
        {
            return new HttpCallBuilder<HttpResponseMessage, T, string>(this);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, T> WithErrorsOfType<T>()
        {
            return new HttpCallBuilder<HttpResponseMessage, string, T>(this);
        }

        #endregion

        public HttpResponseMessage Result()
        {
            return ResultAsync().Result;
        }

        public async Task<HttpResponseMessage> ResultAsync()
        {
            return await ResultAsync(_settings.TokenSource.Token);
        }

        internal async Task<HttpResponseMessage> ResultAsync(CancellationToken token, int retryCount = 0, int redirectCount = 0)
        {
            _settings.Validate();

            using (var client = _clientBuilder.Create())
            {
                using (var message = new HttpRequestMessage(_settings.Method, _settings.Uri))
                {
                    if (_settings.Content != null)
                        message.Content = _settings.Content();

                    var response = await client.SendAsync(message, _settings.CompletionOption, token);

                    var retryCtx = _retryHandler.HandleRetry(this, response, retryCount);

                    if (retryCtx != null && retryCtx.ShouldRetry)
                    {
                        if (retryCtx.RetryAfter > 0)
                            await Task.Delay(retryCtx.RetryAfter, token);

                        response = await ResultAsync(token, retryCount + 1, redirectCount);
                    }

                    var redirectCtx = _redirectionHandler.HandleRedirect(this, response, redirectCount);

                    if (redirectCtx != null)
                    {
                        WithUri(redirectCtx.RedirectUri);
                        response = await ResultAsync(token, retryCount, redirectCount + 1);
                    }

                    return response;
                }
            }
        }
    }
}
