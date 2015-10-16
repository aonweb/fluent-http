using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Client
{
    public class HttpClientSettings : IHttpClientSettings
    {
        public HttpClientSettings()
            : this(true) { }

        private HttpClientSettings(bool init)
        {
            if (init)
                Init();
        }

        private void Init()
        {
            Timeout = Defaults.Current.GetClientDefaults().Timeout;
            MaxRequestContentBufferSize = Defaults.Current.GetClientDefaults().MaxRequestContentBufferSize;
            ClientConfiguration = Defaults.Current.GetClientDefaults().ClientConfiguration;
            RequestHeaderConfiguration = Defaults.Current.GetClientDefaults().RequestHeaderConfiguration;
            DecompressionMethods = Defaults.Current.GetClientDefaults().DecompressionMethods;
            ClientCertificateOptions = Defaults.Current.GetClientDefaults().ClientCertificateOptions;
            Credentials = Defaults.Current.GetClientDefaults().Credentials;
            CookieContainer = Defaults.Current.GetClientDefaults().CookieContainer;
            Proxy = Defaults.Current.GetClientDefaults().Proxy;
        }

        public Action<IHttpClient> ClientConfiguration { get; set; }
        public Action<HttpRequestHeaders> RequestHeaderConfiguration { get; set; }
        public DecompressionMethods? DecompressionMethods { get; set; }
        public ClientCertificateOption? ClientCertificateOptions { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public ICredentials Credentials { get; set; }
        public long? MaxRequestContentBufferSize { get; set; }
        public IWebProxy Proxy { get; set; }
        public TimeSpan? Timeout { get; set; }

        public static HttpClientSettings Empty => new HttpClientSettings(false);
    }
}