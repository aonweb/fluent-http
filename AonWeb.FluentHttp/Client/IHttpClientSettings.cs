using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Client
{
    public interface IHttpClientSettings
    {
        DecompressionMethods? DecompressionMethods { get; set; }
        ClientCertificateOption? ClientCertificateOptions { get; set; }
        CookieContainer CookieContainer { get; set; }
        ICredentials Credentials { get; set; }
        long? MaxRequestContentBufferSize { get; set; }
        IWebProxy Proxy { get; set; }
        TimeSpan? Timeout { get; set; }
        Action<HttpRequestHeaders> RequestHeaderConfiguration { get; set; }
    }
}