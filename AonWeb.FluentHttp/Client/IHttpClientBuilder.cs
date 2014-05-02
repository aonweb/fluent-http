using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Client
{
    public interface IHttpClientBuilder
    {
        HttpClientSettings Settings { get; }
        IHttpClient Create();
        IHttpClientBuilder Configure(Action<IHttpClient> configuration);
        IHttpClientBuilder WithHeaders(Action<HttpRequestHeaders> configuration);
        IHttpClientBuilder WithHeaders(string name, string value);
        IHttpClientBuilder WithHeaders(string name, IEnumerable<string> values);
        IHttpClientBuilder WithTimeout(TimeSpan timeout);
        IHttpClientBuilder WithDecompressionMethods(DecompressionMethods options);
        IHttpClientBuilder WithClientCertificateOptions(ClientCertificateOption options);
        IHttpClientBuilder WithUseCookies();
        IHttpClientBuilder WithUseCookies(CookieContainer container);
        IHttpClientBuilder WithCredentials(ICredentials credentials);
        IHttpClientBuilder WithMaxBufferSize(long bufferSize);
        IHttpClientBuilder WithProxy(IWebProxy proxy);
        IHttpClientBuilder WithCachePolicy(RequestCacheLevel cacheLevel);
        IHttpClientBuilder WithCachePolicy(RequestCachePolicy cachePolicy);
    }
}