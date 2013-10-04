using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly HttpCallBuilderSettings _settings;

        public HttpCallBuilder()
            : this(new HttpCallBuilderSettings(), new HttpClientBuilder()) { }

        public HttpCallBuilder(IHttpClientBuilder clientBuilder)
            : this(new HttpCallBuilderSettings(), clientBuilder) { }

        internal HttpCallBuilder(HttpCallBuilderSettings settings, IHttpClientBuilder clientBuilder)
        {
            _settings = settings;
            _clientBuilder = clientBuilder;
        }

        public HttpCallBuilder WithUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException("uri can not be null or empty", "uri");

            return WithUri(new Uri(uri));
        }

        public HttpCallBuilder WithUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            _settings.Uri = uri;

            return this;
        }

        public HttpCallBuilder WithMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException("method can not be null or empty", "method");

            return WithMethod(new HttpMethod(method));
        }

        public HttpCallBuilder WithMethod(HttpMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _settings.Method = method;

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

        public HttpCallBuilder<TResult> WithResultOfType<TResult>()
        {
            return new HttpCallBuilder<TResult>(_settings);
        }

        public HttpResponseMessage Result()
        {
            return ResultAsync(_settings.TokenSource.Token).Result;
        }

        private async Task<HttpResponseMessage> ResultAsync(CancellationToken token)
        {
            HttpResponseMessage response;

            if (TryGetFromCache(out response))
                return response;

            var r = await ResultAsyncImpl(token, 0);

            HandleResponse(r);

            return r;
        }

        private async Task<HttpResponseMessage> ResultAsyncImpl(CancellationToken token, int redirectCount)
        {
           using (var client = _clientBuilder.Create())
            {
                using (var message = new HttpRequestMessage(_settings.Method, _settings.Uri))
                {
                    var r = await client.SendAsync(message, _settings.CompletionOption, token);

                    if (_clientBuilder.Settings.AllowAutoRedirect && IsRedirect(r))
                    {
                        if (redirectCount > _clientBuilder.Settings.MaxAutomaticRedirections)
                            throw new MaximumAutomaticRedirectsException(string.Format(SR.MaxAutoRedirectsErrorFormat, redirectCount, _settings.Uri));

                        var ctx = HandleRedirection(r);

                        _settings.Uri = ctx.RedirectionUri;

                        return await ResultAsyncImpl(token, redirectCount + 1);
                    }
                }
            } 
        }

        private HttpRedirectionContext HandleRedirection(HttpResponseMessage response)
        {
            var newUri = GetRedirectUri(_settings.Uri, response);

            var ctx = new HttpRedirectionContext
            {
                StatusCode = response.StatusCode,
                RequestMessage = response.RequestMessage,
                RedirectionUri = newUri,
                CurrentUri = uri
            };

            if (_settings.RedirectHandler != null)
                _settings.RedirectHandler(ctx);

            return ctx;
        }

        private bool TryGetFromCache(out HttpResponseMessage response)
        {
            response = null;

            //TODO: implement caching
            //if (!_settings.ShouldGetFromCache())
            //    return false;

            return false;
        }

        private static bool IsRedirect(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently;
        }

        private static Uri GetRedirectUri(Uri originalUri, HttpResponseMessage response)
        {
            var locationUri = response.Headers.Location;

            if (locationUri.IsAbsoluteUri)
                return locationUri;

            return new Uri(originalUri.GetLeftPart(UriPartial.Authority) + locationUri.PathAndQuery);
        }
    }

    public class HttpRedirectionContext
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }
        public Uri RedirectionUri { get; set; }
        public Uri CurrentUri { get; set; }
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
        public HttpMethod Method { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Action<HttpRedirectionContext> RedirectHandler { get; set; }
    }
}
