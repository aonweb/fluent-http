using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp
{
    public static class AdvancedHttpBuilderExtensions
    {
        public static IAdvancedHttpBuilder WithContentEncoding(this IAdvancedHttpBuilder builder, Encoding encoding)

        {
            if (encoding == null)
                return builder;

            builder.WithConfiguration(s => s.ContentEncoding = encoding);
            builder.WithAcceptCharSet(encoding);

            return builder;
        }

        public static IAdvancedHttpBuilder WithHeadersConfiguration(this IAdvancedHttpBuilder builder, Action<HttpRequestHeaders> configuration)
        {
            return builder.WithClientConfiguration(c => c.WithHeadersConfiguration(configuration));
        }

        public static IAdvancedHttpBuilder WithHeader(this IAdvancedHttpBuilder builder, string name, string value)

        {
            return builder.WithClientConfiguration(c => c.WithHeader(name, value));
        }

        public static IAdvancedHttpBuilder WithHeader(this IAdvancedHttpBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.WithClientConfiguration(c => c.WithHeader(name, values));
        }

        public static IAdvancedHttpBuilder WithAppendHeader(this IAdvancedHttpBuilder builder, string name, string value)

        {
            return builder.WithClientConfiguration(c => c.WithAppendHeader(name, value));
        }

        public static IAdvancedHttpBuilder WithAppendHeader(this IAdvancedHttpBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.WithClientConfiguration(c => c.WithAppendHeader(name, values));
        }

        public static IAdvancedHttpBuilder WithMediaType(this IAdvancedHttpBuilder builder, string mediaType)

        {
            if (mediaType == null)
                return builder;

            builder.WithConfiguration(s => s.MediaType = mediaType);
            builder.WithAcceptHeaderValue(mediaType);

            return builder;
        }

        public static IAdvancedHttpBuilder WithAcceptHeaderValue(this IAdvancedHttpBuilder builder, string mediaType)

        {
            return builder.WithHeadersConfiguration(h => h.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType)));
        }

        public static IAdvancedHttpBuilder WithAcceptCharSet(this IAdvancedHttpBuilder builder, Encoding encoding)

        {
            return builder.WithAcceptCharSet(encoding.WebName);
        }

        public static IAdvancedHttpBuilder WithAcceptCharSet(this IAdvancedHttpBuilder builder, string charSet)

        {
            return builder.WithHeadersConfiguration(h => h.AcceptCharset.Add(new StringWithQualityHeaderValue(charSet)));
        }

        public static IAdvancedHttpBuilder WithAutoDecompression(this IAdvancedHttpBuilder builder, bool enabled = true)

        {
            builder.WithConfiguration(s => s.AutoDecompression = enabled);

            return builder.WithDecompressionMethods(enabled
                    ? DecompressionMethods.GZip | DecompressionMethods.Deflate
                    : DecompressionMethods.None);
        }

        public static IAdvancedHttpBuilder WithDecompressionMethods(this IAdvancedHttpBuilder builder, DecompressionMethods options)
        {
            return builder.WithClientConfiguration(c => c.WithDecompressionMethods(options));
        }

        public static IAdvancedHttpBuilder WithClientCertificateOptions(this IAdvancedHttpBuilder builder, ClientCertificateOption options)
        {
            return builder.WithClientConfiguration(c => c.WithClientCertificateOptions(options));
        }

        public static IAdvancedHttpBuilder WithCheckCertificateRevocationList(this IAdvancedHttpBuilder builder, bool check)
        {
            return builder.WithClientConfiguration(c => c.WithCheckCertificateRevocationList(check));
        }

        public static IAdvancedHttpBuilder WithClientCertificates(this IAdvancedHttpBuilder builder, IEnumerable<X509Certificate> certificates)
        {
            return builder.WithClientConfiguration(c => c.WithClientCertificates(certificates));
        }

        public static IAdvancedHttpBuilder WithClientCertificates(this IAdvancedHttpBuilder builder, X509Certificate certificate)
        {
            return builder.WithClientConfiguration(c => c.WithClientCertificates(certificate));
        }

        public static IAdvancedHttpBuilder WithUseCookies(this IAdvancedHttpBuilder builder)
        {
            return builder.WithClientConfiguration(c => c.WithUseCookies());
        }

        public static IAdvancedHttpBuilder WithUseCookies(this IAdvancedHttpBuilder builder, CookieContainer container)
        {
            return builder.WithClientConfiguration(c => c.WithUseCookies(container));
        }

        public static IAdvancedHttpBuilder WithCredentials(this IAdvancedHttpBuilder builder, ICredentials credentials)
        {
            return builder.WithClientConfiguration(c => c.WithCredentials(credentials));
        }

        public static IAdvancedHttpBuilder WithTimeout(this IAdvancedHttpBuilder builder, TimeSpan? timeout)

        {
            return builder.WithClientConfiguration(c => c.WithTimeout(timeout));
        }

        public static IAdvancedHttpBuilder WithNoCache(this IAdvancedHttpBuilder builder, bool nocache = true)

        {
            return builder.WithClientConfiguration(c => c.WithNoCache(nocache));
        }

        public static IAdvancedHttpBuilder WithCaching(this IAdvancedHttpBuilder builder, bool enabled = true)
        {
            return builder.WithOptionalHandlerConfiguration<HttpCacheConfigurationHandler>(c => c.Enabled = enabled);
        }

        public static IAdvancedHttpBuilder WithSuppressCancellationExceptions(this IAdvancedHttpBuilder builder, bool suppress = true)

        {
            builder.WithConfiguration(s => s.SuppressCancellationErrors = suppress);

            return builder;
        }

        public static IAdvancedHttpBuilder WithRetryConfiguration(this IAdvancedHttpBuilder builder, Action<RetryHandler> configuration)

        {
            return builder.WithHandlerConfiguration(configuration);
        }

        public static IAdvancedHttpBuilder WithRedirectConfiguration(this IAdvancedHttpBuilder builder, Action<RedirectHandler> configuration)

        {
            return builder.WithHandlerConfiguration(configuration);
        }

        public static IAdvancedHttpBuilder WithAutoFollowConfiguration(this IAdvancedHttpBuilder builder, Action<FollowLocationHandler> configuration)

        {
            return builder.WithHandlerConfiguration(configuration);
        }

        public static IAdvancedHttpBuilder WithHandler(this IAdvancedHttpBuilder builder, IHttpHandler httpHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHandler(httpHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder WithHandlerConfiguration<THandler>(this IAdvancedHttpBuilder builder, Action<THandler> configure)
            where THandler : class, IHttpHandler
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configure));

            return builder;
        }

        public static IAdvancedHttpBuilder WithOptionalHandlerConfiguration<THandler>(this IAdvancedHttpBuilder builder, Action<THandler> configure)
            where THandler : class, IHttpHandler
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configure, false));

            return builder;
        }

        public static IAdvancedHttpBuilder WithSuccessfulResponseValidator(this IAdvancedHttpBuilder builder, Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            builder.WithConfiguration(s => s.ResponseValidator.Add(new ResponseValidatorFunc(validator)));

            return builder;
        }

        public static IAdvancedHttpBuilder WithExceptionFactory(this IAdvancedHttpBuilder builder, Func<HttpResponseMessage, HttpRequestMessage, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedHttpBuilder WithContextItem(this IAdvancedHttpBuilder builder, string key, object value)
        {
            builder.WithConfiguration(s => s.Items[key] = value);

            return builder;
        }

        public static IAdvancedHttpBuilder OnSending(this IAdvancedHttpBuilder builder, Action<HttpSendingContext> handler)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSending(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<HttpSendingContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSendingAsync(this IAdvancedHttpBuilder builder, Func<HttpSendingContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSendingAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<HttpSendingContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSent(this IAdvancedHttpBuilder builder, Action<HttpSentContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSent(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<HttpSentContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSentAsync(this IAdvancedHttpBuilder builder, Func<HttpSentContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSentAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<HttpSentContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnException(this IAdvancedHttpBuilder builder, Action<HttpExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnException(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<HttpExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExceptionHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnExceptionAsync(this IAdvancedHttpBuilder builder, Func<HttpExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnExceptionAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<HttpExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExceptionHandler(priority, handler));

            return builder;
        }

        #region Caching Events

        #region Hit

        public static IAdvancedHttpBuilder OnCacheHit(this IAdvancedHttpBuilder builder, Action<CacheHitContext<HttpResponseMessage>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheHit(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<CacheHitContext<HttpResponseMessage>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheHitAsync(this IAdvancedHttpBuilder builder, Func<CacheHitContext<HttpResponseMessage>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheHitAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<CacheHitContext<HttpResponseMessage>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Miss


        public static IAdvancedHttpBuilder OnCacheMiss(this IAdvancedHttpBuilder builder, Action<CacheMissContext<HttpResponseMessage>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheMiss(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<CacheMissContext<HttpResponseMessage>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheMissAsync(this IAdvancedHttpBuilder builder, Func<CacheMissContext<HttpResponseMessage>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheMissAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<CacheMissContext<HttpResponseMessage>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Store


        public static IAdvancedHttpBuilder OnCacheStore(this IAdvancedHttpBuilder builder, Action<CacheStoreContext<HttpResponseMessage>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheStore(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<CacheStoreContext<HttpResponseMessage>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheStoreAsync(this IAdvancedHttpBuilder builder, Func<CacheStoreContext<HttpResponseMessage>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheStoreAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<CacheStoreContext<HttpResponseMessage>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expiring

        public static IAdvancedHttpBuilder OnCacheExpiring(this IAdvancedHttpBuilder builder, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheExpiring(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheExpiringAsync(this IAdvancedHttpBuilder builder, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheExpiringAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expired

        public static IAdvancedHttpBuilder OnCacheExpired(this IAdvancedHttpBuilder builder, Action<CacheExpiredContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheExpired(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<CacheExpiredContext> cacheHandler)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheExpiredAsync(this IAdvancedHttpBuilder builder, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnCacheExpiredAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #endregion
    }
}