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

namespace AonWeb.FluentHttp.GraphQL
{
    public static class AdvancedGraphQLBuilderExtensions
    {
        //        public static IAdvancedGraphQLBuilder WithContentEncoding(this IAdvancedGraphQLBuilder builder, Encoding encoding)

        //        {
        //            return builder.WithConfiguration(b => b.WithContentEncoding(encoding));
        //        }

        public static IAdvancedGraphQLBuilder WithHeadersConfiguration(this IAdvancedGraphQLBuilder builder, Action<HttpRequestHeaders> configuration)
        {
            return builder.WithConfiguration(b => b.WithHeadersConfiguration(configuration));
        }

        public static IAdvancedGraphQLBuilder WithHeader(this IAdvancedGraphQLBuilder builder, string name, string value)

        {
            return builder.WithConfiguration(b => b.WithHeader(name, value));
        }

        public static IAdvancedGraphQLBuilder WithHeader(this IAdvancedGraphQLBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.WithConfiguration(b => b.WithHeader(name, values));
        }

        public static IAdvancedGraphQLBuilder WithAppendHeader(this IAdvancedGraphQLBuilder builder, string name, string value)

        {
            return builder.WithConfiguration(b => b.WithAppendHeader(name, value));
        }

        public static IAdvancedGraphQLBuilder WithAppendHeader(this IAdvancedGraphQLBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.WithConfiguration(b => b.WithAppendHeader(name, values));
        }

        //        public static IAdvancedGraphQLBuilder WithAcceptHeaderValue(this IAdvancedGraphQLBuilder builder, string mediaType)

        //        {
        //            return builder.WithConfiguration(b => b.WithAcceptHeaderValue(mediaType));
        //        }

        //        public static IAdvancedGraphQLBuilder WithAcceptCharSet(this IAdvancedGraphQLBuilder builder, Encoding encoding)

        //        {
        //            return builder.WithConfiguration(b => b.WithAcceptCharSet(encoding));
        //        }

        //        public static IAdvancedGraphQLBuilder WithAcceptCharSet(this IAdvancedGraphQLBuilder builder, string charSet)

        //        {
        //            return builder.WithConfiguration(b => b.WithAcceptCharSet(charSet));
        //        }

        public static IAdvancedGraphQLBuilder WithAutoDecompression(this IAdvancedGraphQLBuilder builder, bool enabled = true)

        {
            return builder.WithConfiguration(b => b.WithAutoDecompression(enabled));
        }

        public static IAdvancedGraphQLBuilder WithSuppressCancellationExceptions(this IAdvancedGraphQLBuilder builder, bool suppress = true)

        {
            return builder.WithConfiguration(b => b.WithSuppressCancellationExceptions(suppress));
        }

        //        public static IAdvancedGraphQLBuilder WithMethod(this IAdvancedGraphQLBuilder builder, string method)
        //        {
        //            builder.WithConfiguration(b => b.WithMethod(method));

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithMethod(this IAdvancedGraphQLBuilder builder, HttpMethod method)
        //        {
        //            builder.WithConfiguration(b => b.WithMethod(method));

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithMediaType(this IAdvancedGraphQLBuilder builder, string mediaType)
        //        {
        //            builder.WithConfiguration(b => b.WithMediaType(mediaType));

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithMediaTypeFormatter(this IAdvancedGraphQLBuilder builder, MediaTypeFormatter formatter)
        //        {
        //            builder.WithConfiguration(b => b.WithMediaTypeFormatter(formatter));

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithMediaTypeFormatterConfiguration<TFormatter>(this IAdvancedGraphQLBuilder builder, Action<TFormatter> configure) where TFormatter : MediaTypeFormatter
        //        {
        //            builder.WithConfiguration(b => b.WithMediaTypeFormatterConfiguration(configure));

        //            return builder;
        //        }

        public static IAdvancedGraphQLBuilder WithHandler(this IAdvancedGraphQLBuilder builder, ITypedHandler handler)
        {
            builder.WithConfiguration(b => b.WithHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithHandlerConfiguration<THandler>(this IAdvancedGraphQLBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithOptionalHandlerConfiguration<THandler>(this IAdvancedGraphQLBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithOptionalHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithHttpHandler(this IAdvancedGraphQLBuilder builder, IHttpHandler httpHandler)
        {
            builder.WithConfiguration(b => b.WithHttpHandler(httpHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithHttpHandlerConfiguration<THandler>(this IAdvancedGraphQLBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithHttpHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithOptionalHttpHandlerConfiguration<THandler>(this IAdvancedGraphQLBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(b => b.WithOptionalHttpHandlerConfiguration(configure));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithRetryConfiguration(this IAdvancedGraphQLBuilder builder, Action<RetryHandler> configuration)

        {
            builder.WithConfiguration(b => b.WithRetryConfiguration(configuration));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithRedirectConfiguration(this IAdvancedGraphQLBuilder builder, Action<RedirectHandler> configuration)

        {
            builder.WithConfiguration(b => b.WithRedirectConfiguration(configuration));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithAutoFollowConfiguration(this IAdvancedGraphQLBuilder builder, Action<FollowLocationHandler> configuration)

        {
            builder.WithConfiguration(b => b.WithAutoFollowConfiguration(configuration));

            return builder;
        }

        //        public static IAdvancedGraphQLBuilder WithHttpContentFactory<TContent>(this IAdvancedGraphQLBuilder builder, Func<ITypedBuilderContext, object, Task<HttpContent>> contentFactory)
        //            where TContent : IGraphQLRequest
        //        {
        //            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)).HttpContentFactory = async (ctx, content) => await contentFactory(ctx, content));

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithHttpContentFactory(this IAdvancedGraphQLBuilder builder, Func<ITypedBuilderContext, object, Task<HttpContent>> contentFactory)
        //        {
        //            builder.WithConfiguration(s => s.HttpContentFactory = contentFactory);

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithResultFactory<TResult>(this IAdvancedGraphQLBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<TResult>> resultFactory)
        //            where TResult : IGraphQLResource
        //        {
        //            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)).ResultFactory = async (ctx, request, response) => await resultFactory(ctx, request, response));

        //            return builder;
        //        }

        //        public static IAdvancedGraphQLBuilder WithResultFactory(this IAdvancedGraphQLBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, Task<object>> resultFactory)
        //        {
        //            builder.WithConfiguration(s => s.ResultFactory = resultFactory);

        //            return builder;
        //        }

        public static IAdvancedGraphQLBuilder WithErrorFactory<TError>(this IAdvancedGraphQLBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, ExceptionDispatchInfo, Task<TError>> errorFactory)

        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TError)).ErrorFactory = async (ctx, request, response, ex) => await errorFactory(ctx, request, response, ex));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithErrorFactory(this IAdvancedGraphQLBuilder builder, Func<ITypedBuilderContext, HttpRequestMessage, HttpResponseMessage, ExceptionDispatchInfo, Task<object>> errorFactory)
        {
            builder.WithConfiguration(s => s.ErrorFactory = errorFactory);

            return builder;
        }


        public static IAdvancedGraphQLBuilder WithExceptionFactory(this IAdvancedGraphQLBuilder builder, Func<ExceptionCreationContext, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithContextItem(this IAdvancedGraphQLBuilder builder, string key, object value)
        {
            builder.WithConfiguration(b => b.WithContextItem(key, value));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithNoCache(this IAdvancedGraphQLBuilder builder, bool nocache = true)
        {

            builder.WithConfiguration(b => b.WithNoCache(nocache));

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithCaching(this IAdvancedGraphQLBuilder builder, bool enabled = true)
        {
            return builder.WithConfiguration(b => b.WithCaching(enabled));
        }

        //        public static IAdvancedGraphQLBuilder WithDependentResources(this IAdvancedGraphQLBuilder builder, params IGraphQLResource[] resources)
        //        {
        //            if (resources == null)
        //                return builder;

        //            var uris = resources.Select(r => r.Links.Self());

        //            return builder.WithDependentUris(uris);
        //        }

        public static IAdvancedGraphQLBuilder WithSuppressTypeMismatchExceptions(this IAdvancedGraphQLBuilder builder, bool suppress = true)
        {
            builder.WithConfiguration(s => s.SuppressTypeMismatchExceptions = suppress);

            return builder;
        }

        public static IAdvancedGraphQLBuilder WithSuccessfulResponseValidator(this IAdvancedGraphQLBuilder builder, Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            return builder.WithConfiguration(b => b.WithSuccessfulResponseValidator(validator));
        }

        public static IAdvancedGraphQLBuilder WithTimeout(this IAdvancedGraphQLBuilder builder, TimeSpan? timeout)
        {
            return builder.WithConfiguration(b => b.WithTimeout(timeout));
        }

        #region Typed Handler Events

        #region OnSending

        public static IAdvancedGraphQLBuilder OnSending(this IAdvancedGraphQLBuilder builder, Action<TypedSendingContext<object, object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSending(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedSendingContext<object, object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingAsync(this IAdvancedGraphQLBuilder builder, Func<TypedSendingContext<object, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedSendingContext<object, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSending<TResult, TContent>(this IAdvancedGraphQLBuilder builder, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSending<TResult, TContent>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingAsync<TResult, TContent>(this IAdvancedGraphQLBuilder builder, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingAsync<TResult, TContent>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithContent<TContent>(this IAdvancedGraphQLBuilder builder, Action<TypedSendingContext<object, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithContent<TContent>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedSendingContext<object, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithContentAsync<TContent>(this IAdvancedGraphQLBuilder builder, Func<TypedSendingContext<object, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithContentAsync<TContent>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedSendingContext<object, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithResult<TResult>(this IAdvancedGraphQLBuilder builder, Action<TypedSendingContext<TResult, object>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithResult<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, object>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithResultAsync<TResult>(this IAdvancedGraphQLBuilder builder, Func<TypedSendingContext<TResult, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSendingWithResultAsync<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending
        public static IAdvancedGraphQLBuilder OnSent(this IAdvancedGraphQLBuilder builder, Action<TypedSentContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSent(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedSentContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSentAsync(this IAdvancedGraphQLBuilder builder, Func<TypedSentContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSentAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedSentContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSent<TResult>(this IAdvancedGraphQLBuilder builder, Action<TypedSentContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSent<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedSentContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSentAsync<TResult>(this IAdvancedGraphQLBuilder builder, Func<TypedSentContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnSentAsync<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedSentContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending

        public static IAdvancedGraphQLBuilder OnResult(this IAdvancedGraphQLBuilder builder, Action<TypedResultContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResult(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedResultContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResultAsync(this IAdvancedGraphQLBuilder builder, Func<TypedResultContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResultAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedResultContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResult<TResult>(this IAdvancedGraphQLBuilder builder, Action<TypedResultContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResult<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedResultContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResultAsync<TResult>(this IAdvancedGraphQLBuilder builder, Func<TypedResultContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnResultAsync<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedResultContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending

        public static IAdvancedGraphQLBuilder OnError(this IAdvancedGraphQLBuilder builder, Action<TypedErrorContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnError(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedErrorContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnErrorAsync(this IAdvancedGraphQLBuilder builder, Func<TypedErrorContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnErrorAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedErrorContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnError<TError>(this IAdvancedGraphQLBuilder builder, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnError<TError>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnErrorAsync<TError>(this IAdvancedGraphQLBuilder builder, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnErrorAsync<TError>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending

        public static IAdvancedGraphQLBuilder OnException(this IAdvancedGraphQLBuilder builder, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnException(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExceptionHandler(priority, handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnExceptionAsync(this IAdvancedGraphQLBuilder builder, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnExceptionAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExceptionHandler(priority, handler));

            return builder;
        }


        #endregion

        #endregion

        #region Caching Events

        #region Hit

        public static IAdvancedGraphQLBuilder OnCacheHit(this IAdvancedGraphQLBuilder builder, Action<CacheHitContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHit(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheHitContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHitAsync(this IAdvancedGraphQLBuilder builder, Func<CacheHitContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHitAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheHitContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHit<TResult>(this IAdvancedGraphQLBuilder builder, Action<CacheHitContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHit<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheHitContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHitAsync<TResult>(this IAdvancedGraphQLBuilder builder, Func<CacheHitContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheHitAsync<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheHitContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Miss
        public static IAdvancedGraphQLBuilder OnCacheMiss(this IAdvancedGraphQLBuilder builder, Action<CacheMissContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMiss(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheMissContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMissAsync(this IAdvancedGraphQLBuilder builder, Func<CacheMissContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMissAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheMissContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMiss<TResult>(this IAdvancedGraphQLBuilder builder, Action<CacheMissContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMiss<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheMissContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMissAsync<TResult>(this IAdvancedGraphQLBuilder builder, Func<CacheMissContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheMissAsync<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheMissContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Store

        public static IAdvancedGraphQLBuilder OnCacheStore(this IAdvancedGraphQLBuilder builder, Action<CacheStoreContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStore(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheStoreContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStoreAsync(this IAdvancedGraphQLBuilder builder, Func<CacheStoreContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStoreAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheStoreContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStore<TResult>(this IAdvancedGraphQLBuilder builder, Action<CacheStoreContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStore<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheStoreContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStoreAsync<TResult>(this IAdvancedGraphQLBuilder builder, Func<CacheStoreContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheStoreAsync<TResult>(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheStoreContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expiring

        public static IAdvancedGraphQLBuilder OnCacheExpiring(this IAdvancedGraphQLBuilder builder, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheExpiring(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheExpiringAsync(this IAdvancedGraphQLBuilder builder, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheExpiringAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expired

        public static IAdvancedGraphQLBuilder OnCacheExpired(this IAdvancedGraphQLBuilder builder, Action<CacheExpiredContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheExpired(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Action<CacheExpiredContext> cacheHandler)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheExpiredAsync(this IAdvancedGraphQLBuilder builder, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedGraphQLBuilder OnCacheExpiredAsync(this IAdvancedGraphQLBuilder builder, HandlerPriority priority, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #endregion
    }
}