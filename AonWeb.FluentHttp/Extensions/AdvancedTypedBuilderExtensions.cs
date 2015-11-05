using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public static class AdvancedTypedBuilderExtensions
    {
        public static IAdvancedTypedBuilder WithContentEncoding(this IAdvancedTypedBuilder builder, Encoding encoding)

        {
            return builder.Advanced.WithConfiguration(b => b.WithContentEncoding(encoding));
        }

        public static IAdvancedTypedBuilder WithHeadersConfiguration(this IAdvancedTypedBuilder builder, Action<HttpRequestHeaders> configuration)
        {
            return builder.Advanced.WithConfiguration(b => b.WithHeadersConfiguration(configuration));
        }

        public static IAdvancedTypedBuilder WithHeader(this IAdvancedTypedBuilder builder, string name, string value)

        {
            return builder.Advanced.WithConfiguration(b => b.WithHeader(name, value));
        }

        public static IAdvancedTypedBuilder WithHeader(this IAdvancedTypedBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.Advanced.WithConfiguration(b => b.WithHeader(name, values));
        }

        public static IAdvancedTypedBuilder WithAppendHeader(this IAdvancedTypedBuilder builder, string name, string value)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendHeader(name, value));
        }

        public static IAdvancedTypedBuilder WithAppendHeader(this IAdvancedTypedBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendHeader(name, values));
        }

        public static IAdvancedTypedBuilder WithAcceptHeaderValue(this IAdvancedTypedBuilder builder, string mediaType)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAcceptHeaderValue(mediaType));
        }

        public static IAdvancedTypedBuilder WithAcceptCharSet(this IAdvancedTypedBuilder builder, Encoding encoding)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAcceptCharSet(encoding));
        }

        public static IAdvancedTypedBuilder WithAcceptCharSet(this IAdvancedTypedBuilder builder, string charSet)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAcceptCharSet(charSet));
        }

        public static IAdvancedTypedBuilder WithAutoDecompression(this IAdvancedTypedBuilder builder, bool enabled = true)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAutoDecompression(enabled));
        }

        public static IAdvancedTypedBuilder WithSuppressCancellationExceptions(this IAdvancedTypedBuilder builder, bool suppress = true)

        {
            ((IFluentConfigurable<ITypedBuilder, ITypedBuilderSettings>)builder).WithConfiguration(s => s.SuppressCancellationErrors = suppress);

            return builder.Advanced.WithConfiguration(b => b.WithSuppressCancellationExceptions(suppress));
        }

        public static IAdvancedTypedBuilder WithTimeout(this IAdvancedTypedBuilder builder, TimeSpan? timeout)
        {
            return builder.WithClientConfiguration(c => c.WithTimeout(timeout));
        }

        public static IAdvancedTypedBuilder WithNoCache(this IAdvancedTypedBuilder builder, bool nocache = true)
        {

            builder.Advanced.WithConfiguration(b => b.WithNoCache(nocache));

            return builder;
        }

        public static IAdvancedTypedBuilder WithCaching(this IAdvancedTypedBuilder builder, bool enabled = true)
        {
            return builder.WithOptionalHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.Enabled = enabled);
        }

        public static IAdvancedTypedBuilder WithMediaType(this IAdvancedTypedBuilder builder, string mediaType)
        {
            if (mediaType == null)
                return builder;

            // we need to set the media at the typed settings level so the media formatter can use it 
            builder.WithConfiguration((ITypedBuilderSettings s) => s.MediaType = mediaType);

            // we need to set it at the raw http settings level so that it can set headers and whatnot
            builder.WithConfiguration((IHttpBuilderSettings s) => s.MediaType = mediaType);
            builder.WithAcceptHeaderValue(mediaType);

            return builder;
        }

        public static IAdvancedTypedBuilder WithMediaTypeFormatter(this IAdvancedTypedBuilder builder, MediaTypeFormatter formatter)
        {
            builder.WithConfiguration(s => s.Formatter.MediaTypeFormatters.Add(formatter));

            return builder;
        }

        public static IAdvancedTypedBuilder WithMediaTypeFormatterConfiguration<TFormatter>(this IAdvancedTypedBuilder builder, Action<TFormatter> configure) where TFormatter : MediaTypeFormatter
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            builder.WithConfiguration(s =>
            {
                var formatter = s.Formatter.MediaTypeFormatters.OfType<TFormatter>().FirstOrDefault();

                if (formatter != null)
                    configure(formatter);
            });

            return builder;
        }

        public static IAdvancedTypedBuilder WithHandler(this IAdvancedTypedBuilder builder, ITypedHandler handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder WithHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configure));

            return builder;
        }

        public static IAdvancedTypedBuilder WithOptionalHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configure, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithHttpHandler(this IAdvancedTypedBuilder builder, IHttpHandler httpHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHandler(httpHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder WithHttpHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configure));

            return builder;
        }

        public static IAdvancedTypedBuilder WithOptionalHttpHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configure, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithRetryConfiguration(this IAdvancedTypedBuilder builder, Action<RetryHandler> configuration)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configuration, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithRedirectConfiguration(this IAdvancedTypedBuilder builder, Action<RedirectHandler> configuration)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configuration, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithAutoFollowConfiguration(this IAdvancedTypedBuilder builder, Action<FollowLocationHandler> configuration)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithConfiguration(configuration, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithExceptionFactory(this IAdvancedTypedBuilder builder, Func<ExceptionCreationContext, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedTypedBuilder WithContextItem(this IAdvancedTypedBuilder builder, string key, object value)
        {
            builder.WithConfiguration((ITypedBuilderSettings s) => s.Items[key] = value);

            return builder;
        }

        public static IAdvancedTypedBuilder WithSuppressTypeMismatchExceptions(this IAdvancedTypedBuilder builder, bool suppress = true)
        {
            builder.WithConfiguration(s => s.SuppressTypeMismatchExceptions = suppress);

            return builder;
        }

        public static IAdvancedTypedBuilder WithSuccessfulResponseValidator(this IAdvancedTypedBuilder builder, Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            builder.WithConfiguration((ITypedBuilderSettings s) => s.ResponseValidator.Add(new ResponseValidatorFunc(validator)));

            return builder;
        }

        #region Typed Handler Events

        #region OnSending

        public static IAdvancedTypedBuilder OnSending(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<object, object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSending(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<object, object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingAsync(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<object, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<object, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSending<TResult, TContent>(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSending<TResult, TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingAsync<TResult, TContent>(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingAsync<TResult, TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContent<TContent>(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<object, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContent<TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<object, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContentAsync<TContent>(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<object, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContentAsync<TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<object, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResult<TResult>(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<TResult, object>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResult<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, object>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResultAsync<TResult>(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<TResult, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResultAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending
        public static IAdvancedTypedBuilder OnSent(this IAdvancedTypedBuilder builder, Action<TypedSentContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSent(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSentContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSentAsync(this IAdvancedTypedBuilder builder, Func<TypedSentContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSentAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSentContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSent<TResult>(this IAdvancedTypedBuilder builder, Action<TypedSentContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSent<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSentContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSentAsync<TResult>(this IAdvancedTypedBuilder builder, Func<TypedSentContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSentAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSentContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending

        public static IAdvancedTypedBuilder OnResult(this IAdvancedTypedBuilder builder, Action<TypedResultContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResult(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedResultContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResultAsync(this IAdvancedTypedBuilder builder, Func<TypedResultContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResultAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedResultContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResult<TResult>(this IAdvancedTypedBuilder builder, Action<TypedResultContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResult<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedResultContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResultAsync<TResult>(this IAdvancedTypedBuilder builder, Func<TypedResultContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResultAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedResultContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncResultHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending

        public static IAdvancedTypedBuilder OnError(this IAdvancedTypedBuilder builder, Action<TypedErrorContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnError(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedErrorContext<object>> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnErrorAsync(this IAdvancedTypedBuilder builder, Func<TypedErrorContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnErrorAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedErrorContext<object>, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnError<TError>(this IAdvancedTypedBuilder builder, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnError<TError>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnErrorAsync<TError>(this IAdvancedTypedBuilder builder, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnErrorAsync<TError>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncErrorHandler(priority, handler));

            return builder;
        }

        #endregion

        #region OnSending

        public static IAdvancedTypedBuilder OnException(this IAdvancedTypedBuilder builder, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnException(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExceptionHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnExceptionAsync(this IAdvancedTypedBuilder builder, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnExceptionAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExceptionHandler(priority, handler));

            return builder;
        }


        #endregion

        #endregion

        #region Caching Events

        #region Hit

        public static IAdvancedTypedBuilder OnCacheHit(this IAdvancedTypedBuilder builder, Action<CacheHitContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHit(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheHitContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHitAsync(this IAdvancedTypedBuilder builder, Func<CacheHitContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHitAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheHitContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHit<TResult>(this IAdvancedTypedBuilder builder, Action<CacheHitContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHit<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheHitContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithHitHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHitAsync<TResult>(this IAdvancedTypedBuilder builder, Func<CacheHitContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheHitAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheHitContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncHitHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Miss
        public static IAdvancedTypedBuilder OnCacheMiss(this IAdvancedTypedBuilder builder, Action<CacheMissContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMiss(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheMissContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMissAsync(this IAdvancedTypedBuilder builder, Func<CacheMissContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMissAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheMissContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMiss<TResult>(this IAdvancedTypedBuilder builder, Action<CacheMissContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMiss<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheMissContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithMissHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMissAsync<TResult>(this IAdvancedTypedBuilder builder, Func<CacheMissContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheMissAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheMissContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncMissHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Store

        public static IAdvancedTypedBuilder OnCacheStore(this IAdvancedTypedBuilder builder, Action<CacheStoreContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStore(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheStoreContext<object>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStoreAsync(this IAdvancedTypedBuilder builder, Func<CacheStoreContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStoreAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheStoreContext<object>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStore<TResult>(this IAdvancedTypedBuilder builder, Action<CacheStoreContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStore<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheStoreContext<TResult>> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithStoreHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStoreAsync<TResult>(this IAdvancedTypedBuilder builder, Func<CacheStoreContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheStoreAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheStoreContext<TResult>, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncStoreHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expiring

        public static IAdvancedTypedBuilder OnCacheExpiring(this IAdvancedTypedBuilder builder, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheExpiring(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheExpiringContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiringHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheExpiringAsync(this IAdvancedTypedBuilder builder, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheExpiringAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheExpiringContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiringHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #region Expired

        public static IAdvancedTypedBuilder OnCacheExpired(this IAdvancedTypedBuilder builder, Action<CacheExpiredContext> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheExpired(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<CacheExpiredContext> cacheHandler)

        {
            builder.WithConfiguration(s => s.HandlerRegister.WithExpiredHandler(priority, cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheExpiredAsync(this IAdvancedTypedBuilder builder, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(cacheHandler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnCacheExpiredAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<CacheExpiredContext, Task> cacheHandler)
        {
            builder.WithConfiguration(s => s.HandlerRegister.WithAsyncExpiredHandler(priority, cacheHandler));

            return builder;
        }

        #endregion

        #endregion
    }
}