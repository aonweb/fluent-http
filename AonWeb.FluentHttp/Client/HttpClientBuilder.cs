using System;
using System.Net.Http;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Client
{
    public class HttpClientBuilder : IHttpClientBuilder
    {
        public HttpClientBuilder(IHttpClientSettings settings)
        {
            Settings = settings;
        }

        private IHttpClientSettings Settings { get; }

        public IHttpClientBuilder WithConfiguration(Action<IHttpClientSettings> configuration)
        {
            configuration?.Invoke(Settings);

            return this;
        }

        public IHttpClientBuilder WithConfiguration(Action<IHttpClient> configuration)
        {
            Settings.ClientConfiguration = (Action<IHttpClient>)Delegate.Combine(Settings.ClientConfiguration, configuration);

            return this;
        }

        public IHttpClient Build()
        {
            // TODO: should we pool these client or handler
            var handler = CreateHandler(Settings);

            return GetClientInstance(handler, Settings);
        }

        protected virtual IHttpClient GetClientInstance(HttpMessageHandler handler, IHttpClientSettings settings)
        {
            var client = new HttpClientWrapper(new HttpClient(handler));

            if (Settings.Timeout.HasValue)
                client.Timeout = Settings.Timeout.Value;

            Settings.RequestHeaderConfiguration?.Invoke(client.DefaultRequestHeaders);

            Settings.ClientConfiguration?.Invoke(client);

            return client;
        }
        
        protected virtual HttpMessageHandler CreateHandler(IHttpClientSettings settings)
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
                handler.UseProxy = true;
                handler.Proxy = settings.Proxy;
            }
            
            return handler;
        }

        void IConfigurable<IHttpClientSettings>.WithConfiguration(Action<IHttpClientSettings> configuration)
        {
            WithConfiguration(configuration);
        }

        void IConfigurable<IHttpClient>.WithConfiguration(Action<IHttpClient> configuration)
        {
            WithConfiguration(configuration);
        }
    }
}