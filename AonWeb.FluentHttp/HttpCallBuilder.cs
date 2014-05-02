using System;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
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
                var serializer = _serializerFactory.GetSerializer<TContent>(mediaType);

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

        public IHttpCallBuilder<TResult, TContent, TError> WithHandler(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler)
        {
            _settings.AddHandler(handlerType, priority, handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<HttpCallContext<TResult, TContent, TError>> handler)
        {
            _settings.AddHandler(handler);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithHandlers(params IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>[] handlers)
        {
            _settings.AddHandlers(handlers);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(HttpCallHandlerType handlerType, Action<THandler> configure)
            where THandler : class, IHttpCallHandler<HttpCallContext<TResult, TContent, TError>> 
        {
            if (!_settings.Handlers.ContainsKey(handlerType))
                throw new NotSupportedException(string.Format(SR.HandlerTypeNotSupportedErrorFormat, handlerType));

            var handler = _settings.Handlers[handlerType].OfType<THandler>().FirstOrDefault();

            if (handler != null)
                configure(handler);

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
                await Handle(HttpCallHandlerType.Sending, context);

                if (context.IsResultSet) 
                    return context.Result;

                var response = await _innerBuilder.ResultAsync();

                if (!IsSuccessfulResponse(response))
                {
                    var serializer = _serializerFactory.GetSerializer<TError>(response);

                    var error = await serializer.Deserialize(response.Content);

                    var errorCtx = new HttpErrorContext<TResult, TContent, TError>(context, error);

                    await Handle(HttpCallHandlerType.Error, errorCtx);

                    if (!errorCtx.ErrorHandled)
                        if (_settings.ExceptionFactory != null)
                            throw _settings.ExceptionFactory(errorCtx);
                }
                else
                {
                    await Handle(HttpCallHandlerType.Sent, context);

                    var serializer = _serializerFactory.GetSerializer<TResult>(response);

                    context.Result = await serializer.Deserialize(response.Content);

                    await Handle(HttpCallHandlerType.Result, context);

                    return context.Result;
                } 
            }
            catch (Exception ex)
            {
                var exCtx = new HttpExceptionContext<TResult, TContent, TError>(context, ex);

                Handle(HttpCallHandlerType.Exception, exCtx).Wait();

                if (!exCtx.ExceptionHandled)
                    throw;
            }

            return _settings.DefaultResult();
        }

        private static async Task Handle(HttpCallHandlerType handlerType, HttpCallContext<TResult, TContent, TError> context)
        {
            foreach (var handler in context.Handlers[handlerType].OrderBy(h => h.Priority))
                await handler.Handle(context);
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

        internal HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder, params IHttpCallHandler<HttpCallContext>[] defaultHandlers)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;

            _settings.AddHandlers(defaultHandlers);
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

        public IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration)
        {
            return ConfigureHandler(HttpCallHandlerType.Sent,  configuration);
        }

        public IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration)
        {
            return ConfigureHandler(HttpCallHandlerType.Sent, configuration);
        }

        public IHttpCallBuilder WithHandler(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler)
        {
            _settings.AddHandler(handlerType, priority, handler);

            return this;
        }

        public IHttpCallBuilder WithHandler(IHttpCallHandler<HttpCallContext> handler)
        {
            _settings.AddHandler(handler);

            return this;
        }

        public IHttpCallBuilder WithHandlers(params IHttpCallHandler<HttpCallContext>[] handlers)
        {
            _settings.AddHandlers(handlers);

            return this;
        }

        public IHttpCallBuilder ConfigureHandler<THandler>(
            HttpCallHandlerType handlerType,
            Action<THandler> configure) where THandler : class, IHttpCallHandler<HttpCallContext>
        {
            var handler = _settings.Handlers[handlerType].OfType<THandler>().FirstOrDefault();

            if (handler != null)
                configure(handler);

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
            _clientBuilder.WithCachePolicy(RequestCacheLevel.NoCacheNoStore);

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
                    using (var message = new HttpRequestMessage(context.Method, context.Uri))
                    {
                        if (context.Content != null)
                            message.Content = context.Content();

                        await Handle(HttpCallHandlerType.Sending, context);

                        response = context.Response = await client.SendAsync(message, context.CompletionOption, context.TokenSource.Token);

                        if (!IsSuccessfulResponse(context.Response))
                            if (_settings.ExceptionFactory != null)
                                throw _settings.ExceptionFactory(context.Response);

                        await Handle(HttpCallHandlerType.Sent, context);

                        response = context.Response;
                    }
                }
            }
            catch (Exception ex)
            {
                var exContext = new HttpExceptionContext(context, ex);

                Handle(HttpCallHandlerType.Exception, exContext).Wait();

                if (!exContext.ExceptionHandled)
                    throw;
            }

            return response;
        }

        private static async Task Handle(HttpCallHandlerType handlerType, HttpCallContext context)
        {
            foreach (var handler in context.Handlers[handlerType].OrderBy(h => h.Priority))
                await handler.Handle(context);
        }

        private bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return !_settings.SuccessfulResponseValidators.Any() || _settings.SuccessfulResponseValidators.All(v => v(response));
        }
    }
}
