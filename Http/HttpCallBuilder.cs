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

    public class HttpCallBuilder<TResult>
    {
        private readonly HttpCallBuilderSettings _settings;

        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings)
        {
            _settings = settings;
        }

        public HttpCallBuilder<TResult> WithUri(string uri)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult> WithUri(Uri uri)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult> WithMethod(string method)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult> WithMethod(HttpMethod method)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult, TModel> WithModel<TModel>(TModel model)
        {
            throw new NotImplementedException();
        }

        public TResult Result()
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ResultAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ResultAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

    public class HttpCallBuilder<TResult, TModel> : HttpCallBuilder<TResult>
    {
        private readonly HttpCallBuilderSettings _settings;

        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings)
        {
            _settings = settings;
        }

        public HttpCallBuilder<TResult, TModel> WithUri(string uri)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult, TModel> WithUri(Uri uri)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult, TModel> WithMethod(string method)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult, TModel> WithMethod(HttpMethod method)
        {
            throw new NotImplementedException();
        }

        public HttpCallBuilder<TResult, TModel> WithModel(TModel model)
        {
            throw new NotImplementedException();
        }
    }

    public class HttpCallBuilder
    {
        private readonly IHttpClientBuilder _clientBuilder;
        private readonly RedirectionHandler _redirectionHandler;
        private readonly HttpCallBuilderSettings _settings;


        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings(), new HttpClientBuilder(), new RedirectionHandler()) { }

        public HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder, new RedirectionHandler()) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder, RedirectionHandler redirectionHandler)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;
            _redirectionHandler = redirectionHandler;
        }

        internal HttpCallBuilderSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public HttpCallBuilder WithUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException(SR.ArgumentUriNullOrEmpty, "uri");

            return WithUri(new Uri(uri));
        }

        public HttpCallBuilder WithUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            _settings.Uri = uri;

            return this;
        }

        public HttpCallBuilder WithQueryString(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                return this;

            //TODO: should be delay execution to allow uri to set after?

            _settings.Uri = Utils.AppendToQueryString(_settings.Uri, name, value);

            return this;
        }

        public HttpCallBuilder WithMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException(SR.ArgumentMethodNullOrEmpty, "method");

            return WithMethod(new HttpMethod(method));
        }

        public HttpCallBuilder WithMethod(HttpMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _settings.Method = method;

            return this;
        }

        public HttpCallBuilder WithContent(string content)
        {

            return WithContent(content, null, null);
        }

        public HttpCallBuilder WithContent(string content, Encoding encoding)
        {

            return WithContent(content, encoding, null);
        }

        public HttpCallBuilder WithContent(string content, Encoding encoding, string mediaType)
        {

            return WithContent(() => content, encoding, mediaType);
        }

        public HttpCallBuilder WithContent(Func<string> contentFunc)
        {
            return WithContent(contentFunc, null, null);
        }

        public HttpCallBuilder WithContent(Func<string> contentFunc, Encoding encoding)
        {
            return WithContent(contentFunc, encoding, null);
        }

        public HttpCallBuilder WithContent(Func<string> contentFunc, Encoding encoding, string mediaType)
        {
            return WithContent(() => new StringContent(contentFunc(), encoding, mediaType));
        }

        public HttpCallBuilder WithContent(Func<HttpContent> contentFunc)
        {
            if (contentFunc == null)
                throw new ArgumentNullException("contentFunc");

            _settings.Content = contentFunc;

            return this;
        }

        public HttpCallBuilder ConfigureClient(Action<IHttpClient> configuration)
        {
            _clientBuilder.Configure(configuration);

            return this;
        }

        public HttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            if (configuration != null)
                configuration(_clientBuilder);

            return this;
        }

        public HttpCallBuilder ConfigureRedirection(Action<RedirectionHandler> configuration)
        {
            if (configuration != null)
                configuration(_redirectionHandler);

            return this;
        }

        public HttpCallBuilder WithRedirectionHandler(Action<HttpRedirectionContext> handler)
        {
            _redirectionHandler.WithRedirectionHandler(handler);

            return this;
        }

        public HttpCallBuilder WithNoCache()
        {
            _clientBuilder.WithCachePolicy(RequestCacheLevel.NoCacheNoStore);

            return this;
        }

        public HttpCallBuilder<TResult> WithResultOfType<TResult>()
        {
            return new HttpCallBuilder<TResult>(_settings);
        }

        public HttpResponseMessage Result()
        {
            return ResultAsync().Result;
        }

        public async Task<HttpResponseMessage> ResultAsync()
        {
            return await ResultAsync(_settings.TokenSource.Token);
        }

        private async Task<HttpResponseMessage> ResultAsync(CancellationToken token, int redirectCount = 0)
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
