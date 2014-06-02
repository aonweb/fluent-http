using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Client
{
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
            _settings.ClientConfiguration = Helper.MergeAction(_settings.ClientConfiguration, configuration);

            return this;
        }

        public IHttpClientBuilder WithHeaders(Action<HttpRequestHeaders> configuration)
        {
            _settings.HeaderConfiguration = Helper.MergeAction(_settings.HeaderConfiguration, configuration);

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

        public IHttpClientBuilder WithTimeout(TimeSpan? timeout)
        {
            _settings.Timeout = timeout;

            return this;
        }

        public IHttpClientBuilder WithDecompressionMethods(DecompressionMethods options)
        {
            _settings.DecompressionMethods = options;

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

        public IHttpClientBuilder WithNoCache(bool nocache = true)
        {
            return WithHeaders(
                h =>
                    {
                        if (h.CacheControl == null)
                            h.CacheControl = new CacheControlHeaderValue();

                        h.CacheControl.NoCache = nocache;
                        h.CacheControl.NoStore = nocache;
                    });
        }

        public void ApplyRequestHeaders(HttpRequestMessage request)
        {
            if (_settings.HeaderConfiguration != null)
                _settings.HeaderConfiguration(request.Headers);
        }

        public IHttpClient Create()
        {
            // should we pool these client or handler
            var handler = CreateHandler(_settings);

            var client = GetClientInstance(handler);

            if (_settings.Timeout.HasValue)
                client.Timeout = _settings.Timeout.Value;

            if (_settings.HeaderConfiguration != null) 
                _settings.HeaderConfiguration(client.DefaultRequestHeaders);

            if (_settings.ClientConfiguration != null)
                _settings.ClientConfiguration(client);

            return client;
        }

        protected virtual IHttpClient GetClientInstance(HttpMessageHandler handler)
        {
            return new HttpClientWrapper(new HttpClient(handler));
        }
        
        protected virtual HttpMessageHandler CreateHandler(HttpClientSettings settings)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false, //this will be handled by the consuming code
            };

            if (settings.DecompressionMethods.HasValue)
                handler.AutomaticDecompression = settings.DecompressionMethods.Value;

            if (settings.ClientCertificateOptions != null)
                handler.ClientCertificateOptions = settings.ClientCertificateOptions.Value;

            if (settings.CookieContainer != null)
            {
                handler.CookieContainer = settings.CookieContainer;
                handler.UseCookies = true;
            }

            if (settings.Credentials != null)
            {
                handler.Credentials = settings.Credentials;
                handler.UseDefaultCredentials = true;
                handler.PreAuthenticate = true;
            }

            if (settings.MaxRequestContentBufferSize.HasValue)
                handler.MaxRequestContentBufferSize = settings.MaxRequestContentBufferSize.Value;

            if (settings.Proxy != null)
            {
                handler.Proxy = settings.Proxy;
                handler.UseProxy = true;
            }
            
            return handler;
        }
    }
}