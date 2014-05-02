using System;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Client
{
    public class HttpClientSettings
    {
        public HttpClientSettings()
        {
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
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
        public RequestCachePolicy  CachePolicy { get; internal set; }

    }
}