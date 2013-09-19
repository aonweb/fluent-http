using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.Fluent.Http
{
    public interface IBuilder
    {
        IBuilder WithUri(string uri);
        IBuilder WithUri(Uri uri);
        IBuilder WithMethod(string method);
        IBuilder WithMethod(HttpMethod method);
        IBuilder WithConfiguration(Action<HttpClient> configuration);
        IBuilder<TResult> WithResultOfType<TResult>();
        HttpResponseMessage Result();
        Task<HttpResponseMessage> ResultAsync();
        Task<HttpResponseMessage> ResultAsync(CancellationToken token);
    }

    public interface IBuilder<TResult>
    {
        IBuilder<TResult> WithUri(string uri);
        IBuilder<TResult> WithUri(Uri uri);
        IBuilder<TResult> WithMethod(string method);
        IBuilder<TResult> WithMethod(HttpMethod method);
        IBuilder<TResult, TModel> WithModel<TModel>(TModel model);
        TResult Result();
        Task<TResult> ResultAsync();
        Task<TResult> ResultAsync(CancellationToken token);
    }

    public interface IBuilder<TResult, in TModel>
    {
        IBuilder<TResult, TModel> WithUri(string uri);
        IBuilder<TResult, TModel> WithUri(Uri uri);
        IBuilder<TResult, TModel> WithMethod(string method);
        IBuilder<TResult, TModel> WithMethod(HttpMethod method);
        IBuilder<TResult, TModel> WithModel(TModel model);
        Task<TResult> ResultAsync();
        Task<TResult> ResultAsync(CancellationToken token);
    }

    public class Builder<TResult> : Builder, IBuilder<TResult>
    {
        private readonly HttpBuilderSettings _settings;

        public Builder()
            : this(new HttpBuilderSettings()) { }

        internal Builder(HttpBuilderSettings settings)
        {
            _settings = settings;
        }
    }

    public class Builder<TResult, TModel> : Builder<TResult>, IBuilder<TResult, TModel>
    {
        private readonly HttpBuilderSettings _settings;

        public Builder()
            : this(new HttpBuilderSettings()) { }

        internal Builder(HttpBuilderSettings settings)
        {
            _settings = settings;
        }
    }

    public class Builder: IBuilder
    {
        private readonly HttpBuilderSettings _settings;

        public Builder()
            :this(new HttpBuilderSettings()) { }

        internal Builder(HttpBuilderSettings settings)
        {
            _settings = settings;
        }

        public IBuilder WithUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException("uri can not be null or empty", "uri");

            return WithUri(new Uri(uri));
        }

        public IBuilder WithUri(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            _settings.Uri = uri;

            return this;
        }

        public IBuilder WithMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException("method can not be null or empty", "method");

            return WithMethod(new HttpMethod(method));
        }

        public IBuilder WithMethod(HttpMethod method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            _settings.Method = method;

            return this;
        }

        public IBuilder WithConfiguration(Action<HttpClient> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _settings.HttpClientConfiguration = configuration;

            return this;
        }

        public IBuilder<TResult> WithResultOfType<TResult>()
        {
            return new Builder<TResult>(_settings);
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
            using (var client = GetClient())
            {
                using (var message = new HttpRequestMessage(_settings.Method, _settings.Uri))
                {
                    return client.SendAsync(message, _settings.CompletionOption, token);
                } 
            }
        }

        private HttpClientHandler GetHandler()
        {
            var handler = new HttpClientHandler();

            return handler;
        }

        private HttpClient GetClient()
        {
            var handler = GetHandler();

            var client = new HttpClient(handler);

            return client;
        }
    }

    public class HttpBuilderSettings
    {
        public HttpBuilderSettings()
        {
            Method = HttpMethod.Get;
            TokenSource = new CancellationTokenSource();
        }
        public Uri Uri { get; set; }
        public HttpMethod Method { get; set; }
        public HttpCompletionOption CompletionOption { get; set; }
        public Action<HttpClient> HttpClientConfiguration { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
    }
}
