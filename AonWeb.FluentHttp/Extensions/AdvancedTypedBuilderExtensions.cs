using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;

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
            builder.WithConfiguration(s => s.MediaTypeFormatters.Add(formatter));

            return builder;
        }

        public static IAdvancedTypedBuilder WithMediaTypeFormatterConfiguration<TFormatter>(this IAdvancedTypedBuilder builder, Action<TFormatter> configure) where TFormatter : MediaTypeFormatter
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            builder.WithConfiguration(s =>
            {
                var formatter = s.MediaTypeFormatters.OfType<TFormatter>().FirstOrDefault();

                if (formatter != null)
                    configure(formatter);
            });

            return builder;
        }

        public static IAdvancedTypedBuilder WithHandler<TResult, TContent, TError>(this IAdvancedTypedBuilder builder, ITypedHandler handler)
        {
            builder.WithConfiguration(s => s.Handler.WithHandler<TResult, TContent, TError>(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder WithHandler(this IAdvancedTypedBuilder builder, ITypedHandler handler)
        {
            return builder.WithHandler<object, object, object>(handler);
        }

        public static IAdvancedTypedBuilder WithHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configure));

            return builder;
        }

        public static IAdvancedTypedBuilder WithOptionalHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configure, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithHttpHandler(this IAdvancedTypedBuilder builder, IHandler handler)
        {
            builder.WithConfiguration(s => s.Handler.WithHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder WithHttpHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configure));

            return builder;
        }

        public static IAdvancedTypedBuilder WithOptionalHttpHandlerConfiguration<THandler>(this IAdvancedTypedBuilder builder, Action<THandler> configure)
            where THandler : class, ITypedHandler
        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configure, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithRetryConfiguration(this IAdvancedTypedBuilder builder, Action<RetryHandler> configuration)

        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configuration, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithRedirectConfiguration(this IAdvancedTypedBuilder builder, Action<RedirectHandler> configuration)

        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configuration, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithAutoFollowConfiguration(this IAdvancedTypedBuilder builder, Action<FollowLocationHandler> configuration)

        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configuration, false));

            return builder;
        }

        public static IAdvancedTypedBuilder WithExceptionFactory(this IAdvancedTypedBuilder builder, Func<ErrorContext, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedTypedBuilder WithCaching(this IAdvancedTypedBuilder builder, bool enabled = true)
        {

            builder.WithHandlerConfiguration<TypedCacheHandler>(handler => handler.WithCaching(enabled));

            return builder;
        }

        public static IAdvancedTypedBuilder WithDependentUri(this IAdvancedTypedBuilder builder, Uri uri)
        {
            return builder.WithOptionalHandlerConfiguration<TypedCacheHandler>(h => h.WithDependentUri(uri));
        }

        public static IAdvancedTypedBuilder WithDependentUris(this IAdvancedTypedBuilder builder, IEnumerable<Uri> uris)
        {
            return builder.WithOptionalHandlerConfiguration<TypedCacheHandler>(h => h.WithDependentUris(uris));
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

            builder.WithConfiguration((ITypedBuilderSettings s) => s.SuccessfulResponseValidators.Add(validator));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSending<TResult, TContent>(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSending<TResult, TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingAsync<TResult, TContent>(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingAsync<TResult, TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContent<TContent>(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<object, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.Handler.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContent<TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<object, TContent>> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.Handler.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContentAsync<TContent>(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<object, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithContentAsync<TContent>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<object, TContent>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithContentType(typeof(TContent)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResult<TResult>(this IAdvancedTypedBuilder builder, Action<TypedSendingContext<TResult, object>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResult<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSendingContext<TResult, object>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResultAsync<TResult>(this IAdvancedTypedBuilder builder, Func<TypedSendingContext<TResult, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSendingWithResultAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSendingContext<TResult, object>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSent<TResult>(this IAdvancedTypedBuilder builder, Action<TypedSentContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSent<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedSentContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSentAsync<TResult>(this IAdvancedTypedBuilder builder, Func<TypedSentContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnSentAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedSentContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResult<TResult>(this IAdvancedTypedBuilder builder, Action<TypedResultContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithResultHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResult<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedResultContext<TResult>> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResultAsync<TResult>(this IAdvancedTypedBuilder builder, Func<TypedResultContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncResultHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnResultAsync<TResult>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedResultContext<TResult>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithResultType(typeof(TResult)));

            builder.WithConfiguration(s => s.Handler.WithAsyncResultHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnError<TError>(this IAdvancedTypedBuilder builder, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.Handler.WithErrorHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnError<TError>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedErrorContext<TError>> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.Handler.WithErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnErrorAsync<TError>(this IAdvancedTypedBuilder builder, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.Handler.WithAsyncErrorHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnErrorAsync<TError>(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedErrorContext<TError>, Task> handler)
        {
            builder.WithConfiguration(s => s.WithErrorType(typeof(TError)));

            builder.WithConfiguration(s => s.Handler.WithAsyncErrorHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnException(this IAdvancedTypedBuilder builder, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnException(this IAdvancedTypedBuilder builder, HandlerPriority priority, Action<TypedExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithExceptionHandler(priority, handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnExceptionAsync(this IAdvancedTypedBuilder builder, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedTypedBuilder OnExceptionAsync(this IAdvancedTypedBuilder builder, HandlerPriority priority, Func<TypedExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncExceptionHandler(priority, handler));

            return builder;
        }
    }
}