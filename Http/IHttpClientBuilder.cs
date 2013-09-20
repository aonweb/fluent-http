using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.Fluent.Http
{
    public interface IHttpClientBuilder
    {
        IHttpClient Create();
        IHttpClientBuilder WithConfiguration(Action<IHttpClient> configuration);
        IHttpClientBuilder WithAutoRedirect();
        IHttpClientBuilder WithAutoRedirect(int maxAutomaticRedirections);
        IHttpClientBuilder WithDecompression(DecompressionMethods options);
        IHttpClientBuilder WithClientCertificateOptions(ClientCertificateOption options);
        IHttpClientBuilder WithCookieContainer(CookieContainer container);
        IHttpClientBuilder WithCredentials(ICredentials credentials);
        IHttpClientBuilder WithMaxBuffer(long bufferSize);
        IHttpClientBuilder WithProxy(IWebProxy proxy);
    }

    public class HttpClientSettings
    {
        public Action<IHttpClient> ClientConfiguration { get; set; }
        public bool? AllowAutoRedirect { get; set; }
        public DecompressionMethods? AutomaticDecompression { get; set; }
        public ClientCertificateOption? ClientCertificateOptions { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public ICredentials Credentials { get; set; }
        public int? MaxAutomaticRedirections { get; set; }
        public long? MaxRequestContentBufferSize { get; set; }
        public IWebProxy Proxy { get; set; }
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

        public IHttpClientBuilder WithConfiguration(Action<IHttpClient> configuration)
        {
            if (configuration != null)
            {
                //Merge actions if previous exists
                if (_settings.ClientConfiguration != null)
                {
                    var config1 = _settings.ClientConfiguration;
                    var config2 = configuration;

                    configuration = client =>
                    {
                        config1(client);
                        config2(client);
                    };
                }

                _settings.ClientConfiguration = configuration;
            }

            return this;
        }

        public IHttpClientBuilder WithAutoRedirect()
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithAutoRedirect(int maxAutomaticRedirections)
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithDecompression(DecompressionMethods options)
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithClientCertificateOptions(ClientCertificateOption options)
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithCookieContainer(CookieContainer container)
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithCredentials(ICredentials credentials)
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithMaxBuffer(long bufferSize)
        {
            throw new NotImplementedException();
        }

        public IHttpClientBuilder WithProxy(IWebProxy proxy)
        {
            throw new NotImplementedException();
        }

        public IHttpClient Create()
        {
            var handler = CreateHandler();

            var client = new HttpClient(handler);

            var wrapper = new HttpClientWrapper(client);

            if (_settings.ClientConfiguration != null)
                _settings.ClientConfiguration(wrapper);

            return wrapper;
        }
        
        private HttpClientHandler CreateHandler()
        {
            var handler = new HttpClientHandler();

            if (_settings.AllowAutoRedirect.HasValue)
                handler.AllowAutoRedirect = _settings.AllowAutoRedirect.Value;
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

            if (_settings.MaxAutomaticRedirections.HasValue)
                handler.MaxAutomaticRedirections = _settings.MaxAutomaticRedirections.Value;

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