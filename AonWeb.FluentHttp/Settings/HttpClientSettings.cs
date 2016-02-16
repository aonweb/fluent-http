using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Settings
{
    public class HttpClientSettings : IHttpClientSettings
    {
        public HttpClientSettings()
        {
            DecompressionMethods = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
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
    }
}