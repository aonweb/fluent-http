using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
            // TODO: should we pool these clients or handlers
            var handler = CreateHandler(GetHttpClientHandler, Settings);
            
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

        protected virtual HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler();
        }

        protected virtual HttpMessageHandler CreateHandler(Func<HttpClientHandler> httpClientHandler, IHttpClientSettings settings)
        {
            var handler = httpClientHandler();

            if (handler.SupportsRedirectConfiguration)
                handler.AllowAutoRedirect = false; //this will be handled by the consuming code

            if (handler.SupportsAutomaticDecompression && settings.DecompressionMethods.HasValue)
                handler.AutomaticDecompression = settings.DecompressionMethods.Value;
            
            if (settings.ClientCertificateOptions != null)
                handler.ClientCertificateOptions = settings.ClientCertificateOptions.Value;

            if (settings.CookieContainer != null)
            {
                handler.CookieContainer = settings.CookieContainer;
                handler.UseCookies = true;
            }

            if (settings.CheckCertificateRevocationList.HasValue)
                handler.CheckCertificateRevocationList = settings.CheckCertificateRevocationList.Value;

            if (settings.ClientCertificates != null && settings.ClientCertificates.Count > 0)
            {
                handler.ClientCertificates.AddRange(settings.ClientCertificates.ToArray());
            }

            if (settings.Credentials != null)
            {
                handler.Credentials = settings.Credentials;
                handler.UseDefaultCredentials = true;
                handler.PreAuthenticate = true;
            }

            if (settings.MaxRequestContentBufferSize.HasValue)
                handler.MaxRequestContentBufferSize = settings.MaxRequestContentBufferSize.Value;

            if (handler.SupportsProxy && settings.Proxy != null)
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