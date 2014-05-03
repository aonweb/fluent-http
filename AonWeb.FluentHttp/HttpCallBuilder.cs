using System;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp
{
    public class HttpCallBuilder<TResult, TContent, TError> : IAdvancedHttpCallBuilder<TResult, TContent, TError>
    {
        private readonly HttpCallBuilderSettings<TResult, TContent, TError> _settings;
        private readonly IHttpCallBuilder _innerBuilder;
        private readonly ISerializerFactory _serializerFactory;

        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings<TResult, TContent, TError>(), new HttpCallBuilder()) { }

        internal HttpCallBuilder(IHttpCallBuilder builder)
            : this(new HttpCallBuilderSettings<TResult, TContent, TError>(), builder) { }

        internal HttpCallBuilder(HttpCallBuilderSettings<TResult, TContent, TError> settings, IHttpCallBuilder builder)
            : this(settings, builder, new SerializerFactory()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings<TResult, TContent, TError> settings, IHttpCallBuilder builder, ISerializerFactory serializerFactory)
        {
            _settings = settings;
            _innerBuilder = builder;
            _serializerFactory = serializerFactory;
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

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory)
        {
            return WithContent(contentFactory, null, null);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding)
        {
            return WithContent(contentFactory, encoding, null);
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding, string mediaType)
        {
            _settings.ContentFactory = contentFactory;

            WithEncoding(encoding);
            WithMediaType(mediaType);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithEncoding(Encoding encoding)
        {
            _settings.ContentEncoding = encoding;

            _innerBuilder.Advanced.WithEncoding(encoding);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithMediaType(string mediaType)
        {
            _settings.MediaType = mediaType;

            _innerBuilder.Advanced.WithMediaType(mediaType);

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

        public IHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler)
        {
            _settings.Handler.AddHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>
        {
            _settings.Handler.ConfigureHandler(configure);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            _settings.AddSuccessfulResponseValidator(validator);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory)
        {
            _settings.ExceptionFactory = factory;

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithNoCache()
        {
            _innerBuilder.Advanced.WithNoCache();

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSending(Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSending(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSent(Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSent(Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnResult(Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddResultHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddResultHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnResult(Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddResultHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddResultHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnError(Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddErrorHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddErrorHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnError(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddErrorHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddErrorHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnException(Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnException(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

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
            return ResultAsync().Result;
        }

        public async Task<TResult> ResultAsync()
        {
            return await ResultAsync(new HttpCallContext<TResult, TContent, TError>(this, _settings));
        }

        internal async Task<TResult> ResultAsync(HttpCallContext<TResult, TContent, TError> context)
        {
            try
            {
                var content = context.ContentFactory();

                var sendingContext = new HttpSendingContext<TResult, TContent, TError>(context, content);

                await context.Handler.OnSending(sendingContext);

                _innerBuilder.WithContent(() =>
                {
                    var serializer = _serializerFactory.GetSerializer<TContent>(context.MediaType);

                    var contentString = serializer.Serialize(sendingContext.Content).Result;

                    return contentString;

                }, context.ContentEncoding, context.MediaType);

                if (sendingContext.IsResultSet)
                    return sendingContext.Result;

                var response = await _innerBuilder.ResultAsync();

                if (!IsSuccessfulResponse(response))
                {
                    var serializer = _serializerFactory.GetSerializer<TError>(response);

                    var error = await serializer.Deserialize(response.Content);

                    var errorCtx = new HttpErrorContext<TResult, TContent, TError>(context, error, response);

                    await context.Handler.OnError(errorCtx);

                    if (!errorCtx.ErrorHandled)
                        if (_settings.ExceptionFactory != null)
                            throw _settings.ExceptionFactory(errorCtx);
                }
                else
                {
                    var sentContext = new HttpSentContext<TResult, TContent, TError>(context, response);

                    await context.Handler.OnSent(sentContext);

                    var serializer = _serializerFactory.GetSerializer<TResult>(response);

                    var result = await serializer.Deserialize(response.Content);

                    var resultContext = new HttpResultContext<TResult, TContent, TError>(context, result);

                    await context.Handler.OnResult(resultContext);

                    return resultContext.Result;
                } 
            }
            catch (Exception ex)
            {
                var exCtx = new HttpExceptionContext<TResult, TContent, TError>(context, ex);

                context.Handler.OnException(exCtx).Wait();

                if (!exCtx.ExceptionHandled)
                    throw;
            }

            return _settings.DefaultResultFactory();
        }

        private bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }
    }

    public class HttpCallBuilder : IAdvancedHttpCallBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private readonly HttpCallBuilderSettings _settings;

        public HttpCallBuilder()
            : this(new HttpClientBuilder()) { }

        public HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder, new RetryHandler(), new RedirectHandler()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder, params IHttpCallHandler[] defaultHandlers)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;

            foreach (var handler in defaultHandlers)
                _settings.Handler.AddHandler(handler);
            
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
            _settings.Uri = Helper.AppendToQueryString(_settings.Uri, name, value);

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

        public IHttpCallBuilder WithContent(Func<string> contentFactory)
        {
            return WithContent(contentFactory, null, null);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding)
        {
            return WithContent(contentFactory, encoding, null);
        }

        public IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding, string mediaType)
        {
            return WithContent(() =>
            {
                var content = contentFactory();

                WithEncoding(encoding);
                WithMediaType(mediaType);

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

        public IHttpCallBuilder WithEncoding(Encoding encoding)
        {
            _settings.ContentEncoding = encoding;

            //TODO: set char set and accept encoding char set?

            return this;
        }

        public IHttpCallBuilder WithMediaType(string mediaType)
        {
            _settings.MediaType = mediaType;

            //TODO: set accepts type?

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

        public IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration)
        {
            return ConfigureHandler(configuration);
        }

        public IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration)
        {
            return ConfigureHandler(configuration);
        }

        public IHttpCallBuilder WithHandler(IHttpCallHandler handler)
        {
            _settings.Handler.AddHandler(handler);

            return this;
        }

        public IHttpCallBuilder ConfigureHandler<THandler>( Action<THandler> configure) where THandler : class, IHttpCallHandler
        {
            _settings.Handler.ConfigureHandler(configure);

            return this;
        }

        public IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            _settings.AddSuccessfulResponseValidator(validator);

            return this;
        }

        public IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory)
        {
            _settings.ExceptionFactory = factory;

            return this;
        }

        public IHttpCallBuilder WithNoCache()
        {
            _clientBuilder
                .WithCachePolicy(RequestCacheLevel.NoCacheNoStore)
                .WithHeaders(h => h.CacheControl = new CacheControlHeaderValue{ NoCache = true});

            return this;
        }

        public IHttpCallBuilder OnSending(Action<HttpSendingContext> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler)
        {
            _settings.Handler.AddSendingHandler(handler);

            return this;
        }

        public IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler)
        {
            _settings.Handler.AddSendingHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder OnSent(Action<HttpSentContext> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler)
        {
            _settings.Handler.AddSentHandler(handler);

            return this;
        }

        public IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler)
        {
            _settings.Handler.AddSentHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder OnException(Action<HttpExceptionContext> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
        }

        public IHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(handler);

            return this;
        }

        public IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler)
        {
            _settings.Handler.AddExceptionHandler(priority, handler);

            return this;
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
            return await ResultAsync(new HttpCallContext(this, _settings));
        }

        internal async Task<HttpResponseMessage> ResultAsync(HttpCallContext context)
        {
            context.ValidateSettings();

            HttpResponseMessage response = null;

            try
            {
                using (var client = _clientBuilder.Create())
                {
                    using (var request = new HttpRequestMessage(context.Method, context.Uri))
                    {
                        if (context.ContentFactory != null)
                            request.Content = context.ContentFactory();

                        var sendingContext = new HttpSendingContext(context, request);

                        await context.Handler.OnSending(sendingContext);

                        response = await client.SendAsync(request, context.CompletionOption, context.TokenSource.Token);

                        if (!IsSuccessfulResponse(response))
                            if (_settings.ExceptionFactory != null)
                                throw _settings.ExceptionFactory(response);

                        var sentContext = new HttpSentContext(context, response);

                        await context.Handler.OnSent(sentContext);
                    }
                }
            }
            catch (Exception ex)
            {
                var exContext = new HttpExceptionContext(context, ex);

                context.Handler.OnException(exContext).Wait();

                if (!exContext.ExceptionHandled)
                    throw;
            }

            return response;
        }

        private bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }
    }
}
