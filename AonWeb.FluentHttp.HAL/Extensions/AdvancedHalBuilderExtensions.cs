using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.HAL.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public static class AdvancedHalBuilderExtensions
    {
        public static IAdvancedHalBuilder WithContentEncoding(this IAdvancedHalBuilder builder, Encoding encoding)

        {
            return builder.WithConfiguration(b => b.WithContentEncoding(encoding));
        }

        public static IAdvancedHalBuilder WithHeadersConfiguration(this IAdvancedHalBuilder builder, Action<HttpRequestHeaders> configuration)
        {
            return builder.WithConfiguration(b => b.WithHeadersConfiguration(configuration));
        }

        public static IAdvancedHalBuilder WithHeader(this IAdvancedHalBuilder builder, string name, string value)

        {
            return builder.WithConfiguration(b => b.WithHeader(name, value));
        }

        public static IAdvancedHalBuilder WithHeader(this IAdvancedHalBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.WithConfiguration(b => b.WithHeader(name, values));
        }

        public static IAdvancedHalBuilder WithAppendHeader(this IAdvancedHalBuilder builder, string name, string value)

        {
            return builder.WithConfiguration(b => b.WithAppendHeader(name, value));
        }

        public static IAdvancedHalBuilder WithAppendHeader(this IAdvancedHalBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.WithConfiguration(b => b.WithAppendHeader(name, values));
        }

        public static IAdvancedHalBuilder WithAcceptHeaderValue(this IAdvancedHalBuilder builder, string mediaType)

        {
            return builder.WithConfiguration(b => b.WithAcceptHeaderValue(mediaType));
        }

        public static IAdvancedHalBuilder WithAcceptCharSet(this IAdvancedHalBuilder builder, Encoding encoding)

        {
            return builder.WithConfiguration(b => b.WithAcceptCharSet(encoding));
        }

        public static IAdvancedHalBuilder WithAcceptCharSet(this IAdvancedHalBuilder builder, string charSet)

        {
            return builder.WithConfiguration(b => b.WithAcceptCharSet(charSet));
        }

        public static IAdvancedHalBuilder WithAutoDecompression(this IAdvancedHalBuilder builder, bool enabled = true)

        {
            return builder.WithConfiguration(b => b.WithAutoDecompression(enabled));
        }

        public static IAdvancedHalBuilder WithSuppressCancellationExceptions(this IAdvancedHalBuilder builder, bool suppress = true)

        {
            return builder.WithConfiguration(b => b.WithSuppressCancellationExceptions(suppress));
        }

        public static IAdvancedHalBuilder WithMethod(this IAdvancedHalBuilder builder, string method)
        {
            builder.WithConfiguration(b => b.WithMethod(method));

            return builder;
        }

        public static IAdvancedHalBuilder WithMethod(this IAdvancedHalBuilder builder, HttpMethod method)
        {
            builder.WithConfiguration(b => b.WithMethod(method));

            return builder;
        }

        public static IAdvancedHalBuilder WithMediaType(this IAdvancedHalBuilder builder, string mediaType)
        {
            builder.WithConfiguration(b => b.WithMediaType(mediaType));

            return builder;
        }

        public static IAdvancedHalBuilder WithMediaTypeFormatter(this IAdvancedHalBuilder builder, MediaTypeFormatter formatter)
        {
            builder.WithConfiguration(b => b.WithMediaTypeFormatter(formatter));

            return builder;
        }

        public static IAdvancedHalBuilder WithMediaTypeFormatterConfiguration<TFormatter>(this IAdvancedHalBuilder builder, Action<TFormatter> configure) where TFormatter : MediaTypeFormatter
        {
            builder.WithConfiguration(b => b.WithMediaTypeFormatterConfiguration(configure));

            return builder;
        }

        public static IAdvancedHalBuilder WithHandler(this IAdvancedHalBuilder builder, ITypedHandler handler)
        {
            builder.WithConfiguration(b => b.WithHandler(handler));

            return builder;
        }

        public static IAdvancedHalBuilder WithHandlerConfiguration<THandler>(this IAdvancedHalBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedHalBuilder WithOptionalHandlerConfiguration<THandler>(this IAdvancedHalBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithOptionalHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedHalBuilder WithHttpHandler(this IAdvancedHalBuilder builder, IHttpHandler httpHandler)
        {
            builder.WithConfiguration(b => b.WithHttpHandler(httpHandler));

            return builder;
        }

        public static IAdvancedHalBuilder WithHttpHandlerConfiguration<THandler>(this IAdvancedHalBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithHttpHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedHalBuilder WithOptionalHttpHandlerConfiguration<THandler>(this IAdvancedHalBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithOptionalHttpHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedHalBuilder WithRetryConfiguration(this IAdvancedHalBuilder builder, Action<RetryHandler> configuration)

        {
            builder.WithConfiguration(b => b.WithRetryConfiguration(configuration));

            return builder;
        }

        public static IAdvancedHalBuilder WithRedirectConfiguration(this IAdvancedHalBuilder builder, Action<RedirectHandler> configuration)

        {
            builder.WithConfiguration(b => b.WithRedirectConfiguration(configuration));

            return builder;
        }

        public static IAdvancedHalBuilder WithAutoFollowConfiguration(this IAdvancedHalBuilder builder, Action<FollowLocationHandler> configuration)

        {
            builder.WithConfiguration(b => b.WithAutoFollowConfiguration(configuration));

            return builder;
        }

        public static IAdvancedHalBuilder WithHttpContentFactory<TContent>(this IAdvancedHalBuilder builder, Func<ITypedBuilderContext, object, Task<HttpContent>> contentFactory)
            where TContent : IHalRequest
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)).HttpContentFactory = async (ctx, content) => await contentFactory(ctx, content));

            return builder;
        }

        public static IAdvancedHalBuilder WithHttpContentFactory(this IAdvancedHalBuilder builder, Func<ITypedBuilderContext, object, Task<HttpContent>> contentFactory)
        {
            builder.WithConfiguration(s => s.HttpContentFactory = contentFactory);

            return builder;
        }

        public static IAdvancedHalBuilder WithResultFactory<TResult>(this IAdvancedHalBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<TResult>> resultFactory)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)).ResultFactory = async (ctx, request, response) => await resultFactory(ctx, request, response));

            return builder;
        }

        public static IAdvancedHalBuilder WithResultFactory(this IAdvancedHalBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> resultFactory)
        {
            builder.WithConfiguration(s => s.ResultFactory = resultFactory);

            return builder;
        }

        public static IAdvancedHalBuilder WithErrorFactory<TError>(this IAdvancedHalBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Exception, Task<TError>> errorFactory)
        
{
            builder.WithConfiguration(s => s.WithResultType(typeof(TError)).ErrorFactory = async (ctx, request, response, ex) => await errorFactory(ctx, request, response, ex));

            return builder;
        }

        public static IAdvancedHalBuilder WithErrorFactory(this IAdvancedHalBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Exception, Task<object>> errorFactory)
        {
            builder.WithConfiguration(s => s.ErrorFactory = errorFactory);

            return builder;
        }


        public static IAdvancedHalBuilder WithExceptionFactory(this IAdvancedHalBuilder builder, Func<ExceptionCreationContext, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedHalBuilder WithContextItem(this IAdvancedHalBuilder builder, string key, object value)
        {
            builder.WithConfiguration(b => b.WithContextItem(key, value));

            return builder;
        }

        public static IAdvancedHalBuilder WithNoCache(this IAdvancedHalBuilder builder, bool nocache = true)
        {

            builder.WithConfiguration(b => b.WithNoCache(nocache));

            return builder;
        }

        public static IAdvancedHalBuilder WithCaching(this IAdvancedHalBuilder builder, bool enabled = true)
        {
            return builder.WithConfiguration(b => b.WithCaching(enabled));
        }

        public static IAdvancedHalBuilder WithDependentResources(this IAdvancedHalBuilder builder, params IHalResource[] resources)
        {
            if (resources == null)
                return builder;

            var uris = resources.Select(r => r.Links.Self());

            return builder.WithDependentUris(uris);
        }

        public static IAdvancedHalBuilder WithSuppressTypeMismatchExceptions(this IAdvancedHalBuilder builder, bool suppress = true)
        {
            builder.WithConfiguration(s => s.SuppressTypeMismatchExceptions = suppress);

            return builder;
        }

        public static IAdvancedHalBuilder WithSuccessfulResponseValidator(this IAdvancedHalBuilder builder, Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            return builder.WithConfiguration(b => b.WithSuccessfulResponseValidator(validator));
        }

        public static IAdvancedHalBuilder WithTimeout(this IAdvancedHalBuilder builder, TimeSpan? timeout)
        {
            return builder.WithConfiguration(b => b.WithTimeout(timeout));
        }

        #region Typed Handler Events

        #region OnSending

        public static IAdvancedHalBuilder OnSending(this IAdvancedHalBuilder builder, Action<TypedSendingContext<IHalResource, IHalRequest>> handler)
        {
            builder.WithConfiguration(b => b.OnSending(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSending(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedSendingContext<IHalResource, IHalRequest>> handler)
        {
            builder.WithConfiguration(b => b.OnSending(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingAsync(this IAdvancedHalBuilder builder, Func<TypedSendingContext<IHalResource, IHalRequest>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnSendingAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedSendingContext<IHalResource, IHalRequest>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnSendingAsync(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSending<TResult, TContent>(this IAdvancedHalBuilder builder, Action<TypedSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSending(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSending<TResult, TContent>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSending(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingAsync<TResult, TContent>(this IAdvancedHalBuilder builder, Func<TypedSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSendingAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingAsync<TResult, TContent>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSendingAsync(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithContent<TContent>(this IAdvancedHalBuilder builder, Action<TypedSendingContext<object, TContent>> handler)
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSending(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithContent<TContent>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedSendingContext<object, TContent>> handler)
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSending(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithContentAsync<TContent>(this IAdvancedHalBuilder builder, Func<TypedSendingContext<object, TContent>, Task> handler)
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSendingAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithContentAsync<TContent>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedSendingContext<object, TContent>, Task> handler)
            where TContent : IHalRequest
        {
            builder.WithConfiguration(b => b.OnSendingAsync(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithResult<TResult>(this IAdvancedHalBuilder builder, Action<TypedSendingContext<TResult, object>> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSending(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithResult<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, object>> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSending(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithResultAsync<TResult>(this IAdvancedHalBuilder builder, Func<TypedSendingContext<TResult, object>, Task> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSendingAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSendingWithResultAsync<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, object>, Task> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSendingAsync(priority, handler));

            return builder;
        }

        #endregion

        #region OnSent

        public static IAdvancedHalBuilder OnSent(this IAdvancedHalBuilder builder, Action<TypedSentContext<IHalResource>> handler)
        {
            builder.WithConfiguration(b => b.OnSent(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSent(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedSentContext<IHalResource>> handler)
        {
            builder.WithConfiguration(b => b.OnSent(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSentAsync(this IAdvancedHalBuilder builder, Func<TypedSentContext<IHalResource>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnSentAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSentAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedSentContext<IHalResource>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnSentAsync(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSent<TResult>(this IAdvancedHalBuilder builder, Action<TypedSentContext<TResult>> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSent(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSent<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedSentContext<TResult>> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSent(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSentAsync<TResult>(this IAdvancedHalBuilder builder, Func<TypedSentContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSentAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnSentAsync<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedSentContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnSentAsync(priority, handler));

            return builder;
        }

        #endregion

        #region OnResult

        public static IAdvancedHalBuilder OnResult(this IAdvancedHalBuilder builder, Action<TypedResultContext<IHalResource>> handler)
        {
            builder.WithConfiguration(b => b.OnResult(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResult(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedResultContext<IHalResource>> handler)
        {
            builder.WithConfiguration(b => b.OnResult(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResultAsync(this IAdvancedHalBuilder builder, Func<TypedResultContext<IHalResource>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnResultAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResultAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedResultContext<IHalResource>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnResultAsync(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResult<TResult>(this IAdvancedHalBuilder builder, Action<TypedResultContext<TResult>> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnResult(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResult<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedResultContext<TResult>> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnResult(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResultAsync<TResult>(this IAdvancedHalBuilder builder, Func<TypedResultContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnResultAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnResultAsync<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedResultContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(b => b.OnResultAsync(priority, handler));

            return builder;
        }

        #endregion

        #region OnError

        public static IAdvancedHalBuilder OnError(this IAdvancedHalBuilder builder, Action<TypedErrorContext<object>> handler)
        {
            builder.WithConfiguration(b => b.OnError(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnError(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedErrorContext<object>> handler)
        {
            builder.WithConfiguration(b => b.OnError(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnErrorAsync(this IAdvancedHalBuilder builder, Func<TypedErrorContext<object>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnErrorAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnErrorAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedErrorContext<object>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnErrorAsync(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnError<TError>(this IAdvancedHalBuilder builder, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(b => b.OnError(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnError<TError>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(b => b.OnError(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnErrorAsync<TError>(this IAdvancedHalBuilder builder, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnErrorAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnErrorAsync<TError>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(b => b.OnErrorAsync(priority, handler));

            return builder;
        }

        #endregion

        #region OnException

        public static IAdvancedHalBuilder OnException(this IAdvancedHalBuilder builder, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(b => b.OnException(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnException(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(b => b.OnException(priority, handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnExceptionAsync(this IAdvancedHalBuilder builder, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(b => b.OnExceptionAsync(handler));

            return builder;
        }

        public static IAdvancedHalBuilder OnExceptionAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(b => b.OnExceptionAsync(priority, handler));

            return builder;
        }

        #endregion

        #endregion

        #region Caching Events

        #region Hit
        public static IAdvancedHalBuilder OnCacheHit(this IAdvancedHalBuilder builder, Action<CacheHitContext<IHalResource>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHit(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheHitContext<IHalResource>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHitAsync(this IAdvancedHalBuilder builder, Func<CacheHitContext<IHalResource>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHitAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheHitContext<IHalResource>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHit<TResult>(this IAdvancedHalBuilder builder, Action<CacheHitContext<TResult>> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHit<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheHitContext<TResult>> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHitAsync<TResult>(this IAdvancedHalBuilder builder, Func<CacheHitContext<TResult>, Task> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheHitAsync<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheHitContext<TResult>, Task> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Miss

        public static IAdvancedHalBuilder OnCacheMiss(this IAdvancedHalBuilder builder, Action<CacheMissContext<IHalResource>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMiss(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheMissContext<IHalResource>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMissAsync(this IAdvancedHalBuilder builder, Func<CacheMissContext<IHalResource>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMissAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheMissContext<IHalResource>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMiss<TResult>(this IAdvancedHalBuilder builder, Action<CacheMissContext<TResult>> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMiss<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheMissContext<TResult>> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMissAsync<TResult>(this IAdvancedHalBuilder builder, Func<CacheMissContext<TResult>, Task> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheMissAsync<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheMissContext<TResult>, Task> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Store

        public static IAdvancedHalBuilder OnCacheStore(this IAdvancedHalBuilder builder, Action<CacheStoreContext<IHalResource>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheStore(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheStoreContext<IHalResource>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheStoreAsync(this IAdvancedHalBuilder builder, Func<CacheStoreContext<IHalResource>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheStoreAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheStoreContext<IHalResource>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }


        public static IAdvancedHalBuilder OnCacheStore<TResult>(this IAdvancedHalBuilder builder, Action<CacheStoreContext<TResult>> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheStore<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheStoreContext<TResult>> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheStoreAsync<TResult>(this IAdvancedHalBuilder builder, Func<CacheStoreContext<TResult>, Task> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheStoreAsync<TResult>(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheStoreContext<TResult>, Task> cacheHandler)
            where TResult : IHalResource
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expiring

        public static IAdvancedHalBuilder OnCacheExpiring(this IAdvancedHalBuilder builder, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheExpiring(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheExpiringAsync(this IAdvancedHalBuilder builder, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheExpiringAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expired

        public static IAdvancedHalBuilder OnCacheExpired(this IAdvancedHalBuilder builder, Action<CacheExpiredContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheExpired(this IAdvancedHalBuilder builder, HandlerPriority priority, Action<CacheExpiredContext> cacheHandler)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheExpiredAsync(this IAdvancedHalBuilder builder, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedHalBuilder OnCacheExpiredAsync(this IAdvancedHalBuilder builder, HandlerPriority priority, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #endregion
    }
}