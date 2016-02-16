using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Settings
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
        Action<IHttpClient> ClientConfiguration { get; set; }
    }
}