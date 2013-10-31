using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.Fluent.Http.Exceptions;

namespace AonWeb.Fluent.Http
{

    public class HttpCallBuilder<TResult, TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        private readonly IHttpCallBuilder _innerBuilder;
        private readonly ISerializerFactory _serializerFactory;
        private readonly IErrorHandler<TError> _errorHandler;


        public HttpCallBuilder()
            : this(new HttpCallBuilder()) { }

        internal HttpCallBuilder(IHttpCallBuilder builder)
            : this(builder, new SerializerFactory(), new ErrorHandler<TError>()) { }

        internal HttpCallBuilder(IHttpCallBuilder builder, ISerializerFactory serializerFactory, IErrorHandler<TError> errorHandler)
        {
            _innerBuilder = builder;
            _serializerFactory = serializerFactory;
            _errorHandler = errorHandler;

            builder.ConfigureErrorHandling(ConfigureErrorHandling);
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

                var contentString = serializer.Serialize(content, encoding);

                return contentString;

            }, encoding, mediaType);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration)
        {
            _innerBuilder.ConfigureClient(configuration);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.ConfigureClient(configuration);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureRedirection(Action<IRedirectionHandler> configuration)
        {
            _innerBuilder.ConfigureRedirection(configuration);

            return this;
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithRedirectionHandler(Action<HttpRedirectionContext> handler)
        {
            _innerBuilder.WithRedirectionHandler(handler);

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

        public IHttpCallBuilder<TResult, TContent, TError> WithNoCache()
        {
            _innerBuilder.WithNoCache();

            return this;
        }

        public TResult Result()
        {
            return ResultAsync().Result;
        }

        public async Task<TResult> ResultAsync()
        {
            //TODO: caching

            var response = await _innerBuilder.ResultAsync();

            var serializer = _serializerFactory.GetSerializer(response.Content.Headers.ContentType.MediaType);

            var result = serializer.Deserialize<TResult>(response.Content);

            return result;
        }

        public IHttpCallBuilder<TResult, TContent, TError> CancelRequest()
        {
            _innerBuilder.CancelRequest();

            return this;
        }

        public IHttpCallBuilder<T, TContent, TError> WithResultOfType<T>()
        {
            return new HttpCallBuilder<T, TContent, TError>(_innerBuilder);
        }

        public IHttpCallBuilder<TResult, T, TError> WithContentOfType<T>()
        {
            return new HttpCallBuilder<TResult, T, TError>(_innerBuilder);
        }

        public IHttpCallBuilder<TResult, TContent, T> WithErrorsOfType<T>()
        {
            return new HttpCallBuilder<TResult, TContent, T>(_innerBuilder);
        }

        private void ConfigureErrorHandling(IErrorHandler<string> handler)
        {
            handler.WithNewErrorHandler(ctx =>
            {
                //validate response

                //if validation fails, create error object - need serialization factory

                //call handler

                //if not handled throw
            });
        }
    }

    public interface ISerializerFactory
    {
        ISerializer GetSerializer(string mediaType);
    }

    public class SerializerFactory : ISerializerFactory
    {
        public ISerializer GetSerializer(string mediaType)
        {
            //TODO: more serializers
            //ProtoBuf, Message Pack, XML
            switch (mediaType)
            {
                default:
                    return new JsonSerializer();
            }
        }
    }

    public interface ISerializer
    {
        string Serialize(object obj, Encoding encoding);
        T Deserialize<T>(HttpContent content);
    }

    public class JsonSerializer : ISerializer
    {
        public string Serialize(object obj, Encoding encoding)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public T Deserialize<T>(HttpContent content)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
    }

    public interface IHttpCallBuilder : IHttpCallBuilder<HttpResponseMessage, string, string>
    {
        IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(Func<HttpContent> contentFunc);
    }

    public interface IHttpCallBuilder<TResult, in TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> WithUri(string uri);
        IHttpCallBuilder<TResult, TContent, TError> WithUri(Uri uri);
        IHttpCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value);
        IHttpCallBuilder<TResult, TContent, TError> WithMethod(string method);
        IHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding, string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureRedirection(Action<IRedirectionHandler> configuration);
        IHttpCallBuilder<TResult, TContent, TError> WithRedirectionHandler(Action<HttpRedirectionContext> handler);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureErrorHandling(Action<IErrorHandler<TError>> configuration);
        IHttpCallBuilder<TResult, TContent, TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> WithNoCache();
        TResult Result();
        Task<TResult> ResultAsync();

        IHttpCallBuilder<TResult, TContent, TError> CancelRequest();

        //conversion methods

        IHttpCallBuilder<T, TContent, TError> WithResultOfType<T>();
        IHttpCallBuilder<TResult, T, TError> WithContentOfType<T>();
        IHttpCallBuilder<TResult, TContent, T> WithErrorsOfType<T>();
    }

    public class HttpCallBuilder : IHttpCallBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private readonly IRedirectionHandler _redirectionHandler;
        private readonly IErrorHandler<string> _errorHandler;
        private readonly HttpCallBuilderSettings _settings;


        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings(), new HttpClientBuilder(), new RedirectionHandler(), new ErrorHandler<string>()) { }

        public HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder, new RedirectionHandler(), new ErrorHandler<string>()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder, IRedirectionHandler redirectionHandler, IErrorHandler<string> errorHandler)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;
            _redirectionHandler = redirectionHandler;
            _errorHandler = errorHandler;
        }

        internal HttpCallBuilderSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException(SR.ArgumentUriNullOrEmpty, "uri");

            return WithUri(new Uri(uri));
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            _settings.Uri = uri;

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithQueryString(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            //TODO: should be delay execution to allow uri to set after?

            _settings.Uri = Utils.AppendToQueryString(_settings.Uri, name, value);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException(SR.ArgumentMethodNullOrEmpty, "method");

            return WithMethod(new HttpMethod(method));
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithMethod(HttpMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _settings.Method = method;

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(string content)
        {

            return WithContent(content, null, null);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(string content, Encoding encoding)
        {

            return WithContent(content, encoding, null);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(string content, Encoding encoding, string mediaType)
        {

            return WithContent(() => content, encoding, mediaType);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(Func<string> contentFunc)
        {
            return WithContent(contentFunc, null, null);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(Func<string> contentFunc, Encoding encoding)
        {
            return WithContent(contentFunc, encoding, null);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(Func<string> contentFunc, Encoding encoding, string mediaType)
        {
            return WithContent(() =>
            {
                var content = contentFunc();
                return new StringContent(content, encoding, mediaType);
            });
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithContent(Func<HttpContent> contentFunc)
        {
            if (contentFunc == null)
                throw new ArgumentNullException("contentFunc");

            _settings.Content = contentFunc;

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> ConfigureClient(Action<IHttpClient> configuration)
        {
            _clientBuilder.Configure(configuration);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            if (configuration != null)
                configuration(_clientBuilder);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> ConfigureRedirection(Action<IRedirectionHandler> configuration)
        {
            if (configuration != null)
                configuration(_redirectionHandler);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithRedirectionHandler(Action<HttpRedirectionContext> handler)
        {
            _redirectionHandler.WithRedirectionHandler(handler);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithNoCache()
        {
            _clientBuilder.WithCachePolicy(RequestCacheLevel.NoCacheNoStore);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> ConfigureErrorHandling(Action<IErrorHandler<string>> configuration)
        {
            if (configuration != null)
                configuration(_errorHandler);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> WithErrorHandler(Action<HttpErrorContext<string>> handler)
        {
            _errorHandler.WithErrorHandler(handler);

            return this;
        }

        public IHttpCallBuilder<HttpResponseMessage, string, string> CancelRequest()
        {
            _settings.TokenSource.Cancel();

            return this;
        }

        public IHttpCallBuilder<T, string, string> WithResultOfType<T>()
        {
            return new HttpCallBuilder<T, string, string>();
        }

        public HttpResponseMessage Result()
        {
            return ResultAsync().Result;
        }

        public async Task<HttpResponseMessage> ResultAsync()
        {
            return await ResultAsync(_settings.TokenSource.Token);
        }

        internal async Task<HttpResponseMessage> ResultAsync(CancellationToken token, int redirectCount = 0)
        {
            _settings.Validate();

            using (var client = _clientBuilder.Create())
            {
                using (var message = new HttpRequestMessage(_settings.Method, _settings.Uri))
                {
                    if (_settings.Content != null)
                        message.Content = _settings.Content();

                    var response = await client.SendAsync(message, _settings.CompletionOption, token);

                    var ctx = _redirectionHandler.HandleRedirection(this, response, redirectCount);

                    if (ctx != null)
                    {
                        WithUri(ctx.RedirectionUri);
                        response = await ResultAsync(token, redirectCount + 1);
                    }

                    return response;
                }
            }
        }

        #region Unimplemented IHttpCallBuilder<TResult,TContent, TError> Methods

        public IHttpCallBuilder<HttpResponseMessage, T, string> WithContentOfType<T>()
        {
            return new HttpCallBuilder<HttpResponseMessage, T, string>(this);
        }

        public IHttpCallBuilder<HttpResponseMessage, string, T> WithErrorsOfType<T>()
        {
            return new HttpCallBuilder<HttpResponseMessage, string, T>(this);
        }

        #endregion
    }

    public class HttpCallBuilderSettings
    {
        public HttpCallBuilderSettings()
        {
            Method = HttpMethod.Get;
            CompletionOption = HttpCompletionOption.ResponseContentRead;
            TokenSource = new CancellationTokenSource();
        }

        public Uri Uri { get; set; }
        public NameValueCollection QueryString { get; set; }
        public HttpMethod Method { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Func<HttpContent> Content { get; set; }

        public void Validate()
        {
            if (Uri == null)
                throw new InvalidOperationException("Uri not set");
        }
    }
}
