using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.Fluent.Http
{
   
    public class HttpCallBuilder<TResult> : HttpCallBuilder
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
            :this(new HttpCallBuilderSettings(), new HttpClientBuilder()) { }

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

        public HttpCallBuilder WithClientConfiguration(Action<IHttpClient> configuration)
        {
            _clientBuilder.WithConfiguration(configuration);

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

        public Task<HttpResponseMessage> ResultAsync()
        {
            return ResultAsync(_settings.TokenSource.Token);
        }

        public Task<HttpResponseMessage> ResultAsync(CancellationToken token)
        {
            using (var client = _clientBuilder.Create())
            {
                using (var message = new HttpRequestMessage(_settings.Method, _settings.Uri))
                {
                    return client.SendAsync(message, _settings.CompletionOption, token);
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
        public HttpMethod Method { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
    }
}
