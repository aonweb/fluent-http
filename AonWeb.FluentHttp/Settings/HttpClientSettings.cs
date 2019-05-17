using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Settings
{
    public class HttpClientSettings : IHttpClientSettings
    {
        public HttpClientSettings()
        {
            DecompressionMethods = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate;
            ClientCertificates = new List<X509Certificate>();
        }

        public ClientCertificateOption? ClientCertificateOptions { get; set; }
        public bool? CheckCertificateRevocationList { get; set; }
        public IList<X509Certificate> ClientCertificates { get; }
        public Action<IHttpClient> ClientConfiguration { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public ICredentials Credentials { get; set; }
        public DecompressionMethods? DecompressionMethods { get; set; }
        public long? MaxRequestContentBufferSize { get; set; }
        public IWebProxy Proxy { get; set; }
        public Action<HttpRequestHeaders> RequestHeaderConfiguration { get; set; }
        public TimeSpan? Timeout { get; set; }
    }
}