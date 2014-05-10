using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Client
{
    public class HttpClientSettings
    {
        public HttpClientSettings()
        {
            Timeout = HttpCallBuilderDefaults.DefaultClientTimeout;
            MaxRequestContentBufferSize = HttpCallBuilderDefaults.DefaultMaxRequestContentBufferSize;
            ClientConfiguration = HttpCallBuilderDefaults.DefaultClientConfiguration;
            HeaderConfiguration = HttpCallBuilderDefaults.DefaultRequestHeadersConfiguration;
            AutomaticDecompression = HttpCallBuilderDefaults.DefaultDecompressionMethods;
            ClientCertificateOptions = HttpCallBuilderDefaults.DefaultClientCertificateOptions;
            Credentials = HttpCallBuilderDefaults.DefaultCredentials;
        }

        public Action<IHttpClient> ClientConfiguration { get; internal set; }
        public Action<HttpRequestHeaders> HeaderConfiguration { get; internal set; }
        public DecompressionMethods? AutomaticDecompression { get; internal set; }
        public ClientCertificateOption? ClientCertificateOptions { get; internal set; }
        public CookieContainer CookieContainer { get; internal set; }
        public ICredentials Credentials { get; internal set; }
        public long? MaxRequestContentBufferSize { get; internal set; }
        public IWebProxy Proxy { get; internal set; }
        public TimeSpan? Timeout { get; internal set; }
    }
}