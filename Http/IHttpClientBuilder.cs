using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.Fluent.Http
{
    public interface IHttpClientBuilder
    {
        HttpClientSettings Settings { get; }

        IHttpClient Create();
        IHttpClientBuilder Configure(Action<IHttpClient> configuration);
        IHttpClientBuilder WithHeaders(Action<HttpRequestHeaders> configuration);
        IHttpClientBuilder WithHeaders(string name, string value);
        IHttpClientBuilder WithHeaders(string name, IEnumerable<string> values);
        IHttpClientBuilder WithTimeout(TimeSpan timeout);
        IHttpClientBuilder WithAutoRedirect();
        IHttpClientBuilder WithAutoRedirect(int maxAutomaticRedirections);
        IHttpClientBuilder WithDecompressionMethods(DecompressionMethods options);
        IHttpClientBuilder WithClientCertificateOptions(ClientCertificateOption options);
        IHttpClientBuilder WithUseCookies();
        IHttpClientBuilder WithUseCookies(CookieContainer container);
        IHttpClientBuilder WithCredentials(ICredentials credentials);
        IHttpClientBuilder WithMaxBufferSize(long bufferSize);
        IHttpClientBuilder WithProxy(IWebProxy proxy);
    }

    public class HttpClientSettings
    {
        public HttpClientSettings()
        {
            AllowAutoRedirect = true;
            MaxAutomaticRedirections = 5;
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
        }

        public Action<IHttpClient> ClientConfiguration { get; internal set; }
        public Action<HttpRequestHeaders> HeaderConfiguration { get; internal set; }
        public bool AllowAutoRedirect { get; internal set; }
        public DecompressionMethods? AutomaticDecompression { get; internal set; }
        public ClientCertificateOption? ClientCertificateOptions { get; internal set; }
        public CookieContainer CookieContainer { get; internal set; }
        public ICredentials Credentials { get; internal set; }
        public int MaxAutomaticRedirections { get; internal set; }
        public long? MaxRequestContentBufferSize { get; internal set; }
        public IWebProxy Proxy { get; internal set; }
        public TimeSpan? Timeout { get; internal set; }
        public RequestCachePolicy  CachePolicy { get; internal set; }
    }

    public class HttpClientBuilder : IHttpClientBuilder
    {
        private readonly HttpClientSettings _settings;

        public HttpClientBuilder()
            : this(new HttpClientSettings()) { }

        internal HttpClientBuilder(HttpClientSettings settings)
        {
            _settings = settings;
        }

        public HttpClientSettings Settings { get { return _settings; } }

        public IHttpClientBuilder Configure(Action<IHttpClient> configuration)
        {
            _settings.ClientConfiguration = Utils.MergeAction(_settings.ClientConfiguration, configuration);

            return this;
        }

        public IHttpClientBuilder WithHeaders(Action<HttpRequestHeaders> configuration)
        {
            _settings.HeaderConfiguration = Utils.MergeAction(_settings.HeaderConfiguration, configuration);

            return this;
        }

        public IHttpClientBuilder WithHeaders(string name, string value)
        {
            return WithHeaders(h => h.Add(name, value));
        }

        public IHttpClientBuilder WithHeaders(string name, IEnumerable<string> values)
        {
            return WithHeaders(h => h.Add(name, values));
        }

        public IHttpClientBuilder WithTimeout(TimeSpan timeout)
        {
            _settings.Timeout = timeout;

            return this;
        }

        public IHttpClientBuilder WithAutoRedirect()
        {
            return WithAutoRedirect(-1);
        }

        public IHttpClientBuilder WithAutoRedirect(int maxAutomaticRedirections)
        {
            _settings.AllowAutoRedirect = true;

            if (maxAutomaticRedirections > 0)
                _settings.MaxAutomaticRedirections = maxAutomaticRedirections;

            return this;
        }

        public IHttpClientBuilder WithDecompressionMethods(DecompressionMethods options)
        {
            _settings.AutomaticDecompression = options;

            return this;
        }

        public IHttpClientBuilder WithClientCertificateOptions(ClientCertificateOption options)
        {
            _settings.ClientCertificateOptions = options;

            return this;
        }

        public IHttpClientBuilder WithUseCookies()
        {
            return WithUseCookies(new CookieContainer());
        }

        public IHttpClientBuilder WithUseCookies(CookieContainer container)
        {
            _settings.CookieContainer = container;

            return this;
        }

        public IHttpClientBuilder WithCredentials(ICredentials credentials)
        {
            _settings.Credentials = credentials;

            return this;
        }

        public IHttpClientBuilder WithMaxBufferSize(long bufferSize)
        {
            _settings.MaxRequestContentBufferSize = bufferSize;

            return this;
        }

        public IHttpClientBuilder WithProxy(IWebProxy proxy)
        {
            _settings.Proxy = proxy;

            return this;
        }

        public IHttpClient Create()
        {
            var handler = CreateHandler();

            var client = new HttpClient(handler);

            if (_settings.Timeout.HasValue)
                client.Timeout = _settings.Timeout.Value;

            if (_settings.HeaderConfiguration != null) 
                _settings.HeaderConfiguration(client.DefaultRequestHeaders);

            var wrapper = new HttpClientWrapper(client);

            if (_settings.ClientConfiguration != null)
                _settings.ClientConfiguration(wrapper);

            return wrapper;
        }
        
        private HttpMessageHandler CreateHandler()
        {
            var handler = new WebRequestHandler();

            //this will be handled by the consuming code
            handler.AllowAutoRedirect = false;

            if (_settings.AutomaticDecompression.HasValue)
                handler.AutomaticDecompression = _settings.AutomaticDecompression.Value;

            if (_settings.ClientCertificateOptions != null)
                handler.ClientCertificateOptions = _settings.ClientCertificateOptions.Value;

            if (_settings.CookieContainer != null)
            {
                handler.CookieContainer = _settings.CookieContainer;
                handler.UseCookies = true;
            }

            if (_settings.Credentials != null)
            {
                handler.Credentials = _settings.Credentials;
                handler.UseDefaultCredentials = true;
                handler.PreAuthenticate = true;
            }

            if (_settings.MaxRequestContentBufferSize.HasValue)
                handler.MaxRequestContentBufferSize = _settings.MaxRequestContentBufferSize.Value;

            if (_settings.Proxy != null)
            {
                handler.Proxy = _settings.Proxy;
                handler.UseProxy = true;
            }
            return handler;
        }
    }
}