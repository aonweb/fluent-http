using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public static class HttpClientBuilderExtensions
    {
        internal static IHttpClientBuilder ApplyRequestHeaders(this IHttpClientBuilder builder, HttpRequestMessage request)
        {
            return builder.WithConfiguration(s => s.RequestHeaderConfiguration?.Invoke(request.Headers));
        }

        public static IHttpClientBuilder WithHeadersConfiguration(this IHttpClientBuilder builder, Action<HttpRequestHeaders> configuration)
        {
            return builder.WithConfiguration(s => s.RequestHeaderConfiguration = (Action<HttpRequestHeaders>)Delegate.Combine(s.RequestHeaderConfiguration, configuration));
        }

        public static IHttpClientBuilder WithHeader(this IHttpClientBuilder builder, string name, string value)
        {
            return builder.WithHeadersConfiguration(h => {
                if (h.Contains(name))
                    h.Remove(name);

                h.Add(name, value);
            });
        }

        public static IHttpClientBuilder WithHeader(this IHttpClientBuilder builder, string name, IEnumerable<string> values)
        {
            return builder.WithHeadersConfiguration(h =>
            {
                if (h.Contains(name))
                    h.Remove(name);

                h.Add(name, values);
            });
        }

        public static IHttpClientBuilder WithAppendHeader(this IHttpClientBuilder builder, string name, string value)
        {
            return builder.WithHeadersConfiguration(h => h.Add(name, value));
        }

        public static IHttpClientBuilder WithAppendHeader(this IHttpClientBuilder builder, string name, IEnumerable<string> values)
        {
            return builder.WithHeadersConfiguration(h => h.Add(name, values));
        }

        public static IHttpClientBuilder WithTimeout(this IHttpClientBuilder builder, TimeSpan? timeout)
        {
            return builder.WithConfiguration(s => s.Timeout = timeout);
        }

        public static IHttpClientBuilder WithDecompressionMethods(this IHttpClientBuilder builder, DecompressionMethods options)
        {
            return builder.WithConfiguration(s => s.DecompressionMethods = options);
        }

        public static IHttpClientBuilder WithClientCertificateOptions(this IHttpClientBuilder builder, ClientCertificateOption options)
        {
            return builder.WithConfiguration(s => s.ClientCertificateOptions = options);
        }

        public static IHttpClientBuilder WithUseCookies(this IHttpClientBuilder builder)
        {
            return builder.WithUseCookies(new CookieContainer());
        }

        public static IHttpClientBuilder WithUseCookies(this IHttpClientBuilder builder,CookieContainer container)
        {
            return builder.WithConfiguration(s => s.CookieContainer = container);
        }

        public static IHttpClientBuilder WithCredentials(this IHttpClientBuilder builder,ICredentials credentials)
        {
            return builder.WithConfiguration(s => s.Credentials = credentials);
        }

        public static IHttpClientBuilder WithMaxRequestBufferSize(this IHttpClientBuilder builder,long bufferSize)
        {
            return builder.WithConfiguration(s => s.MaxRequestContentBufferSize = bufferSize);
        }

        public static IHttpClientBuilder WithMaxResponseBufferSize(this IHttpClientBuilder builder, long bufferSize)
        {
            return builder.WithConfiguration(s => s.MaxResponseContentBufferSize = bufferSize);
        }

        public static IHttpClientBuilder WithProxy(this IHttpClientBuilder builder,IWebProxy proxy)
        {
            return builder.WithConfiguration(s => s.Proxy = proxy);
        }

        public static IHttpClientBuilder WithNoCache(this IHttpClientBuilder builder, bool nocache = true)
        {
            return builder.WithHeadersConfiguration(
                h =>
                {
                    if (h.CacheControl == null)
                        h.CacheControl = new CacheControlHeaderValue();

                    h.CacheControl.NoCache = nocache;
                    h.CacheControl.NoStore = nocache;
                });
        }  
    }
}
