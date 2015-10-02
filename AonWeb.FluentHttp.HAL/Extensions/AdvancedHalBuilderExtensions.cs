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
using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public static class AdvancedHalBuilderExtensions
    {
        public static IAdvancedHalBuilder WithContentEncoding(this IAdvancedHalBuilder builder, Encoding encoding)

        {
            return builder.Advanced.WithConfiguration(b => b.WithContentEncoding(encoding));
        }

        public static IAdvancedHalBuilder WithHeadersConfiguration(this IAdvancedHalBuilder builder, Action<HttpRequestHeaders> configuration)
        {
            return builder.Advanced.WithConfiguration(b => b.WithHeadersConfiguration(configuration));
        }

        public static IAdvancedHalBuilder WithHeader(this IAdvancedHalBuilder builder, string name, string value)

        {
            return builder.Advanced.WithConfiguration(b => b.WithHeader(name, value));
        }

        public static IAdvancedHalBuilder WithHeader(this IAdvancedHalBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.Advanced.WithConfiguration(b => b.WithHeader(name, values));
        }

        public static IAdvancedHalBuilder WithAppendHeader(this IAdvancedHalBuilder builder, string name, string value)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendHeader(name, value));
        }

        public static IAdvancedHalBuilder WithAppendHeader(this IAdvancedHalBuilder builder, string name, IEnumerable<string> values)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAppendHeader(name, values));
        }

        public static IAdvancedHalBuilder WithAcceptHeaderValue(this IAdvancedHalBuilder builder, string mediaType)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAcceptHeaderValue(mediaType));
        }

        public static IAdvancedHalBuilder WithAcceptCharSet(this IAdvancedHalBuilder builder, Encoding encoding)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAcceptCharSet(encoding));
        }

        public static IAdvancedHalBuilder WithAcceptCharSet(this IAdvancedHalBuilder builder, string charSet)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAcceptCharSet(charSet));
        }

        public static IAdvancedHalBuilder WithAutoDecompression(this IAdvancedHalBuilder builder, bool enabled = true)

        {
            return builder.Advanced.WithConfiguration(b => b.WithAutoDecompression(enabled));
        }

        public static IAdvancedHalBuilder WithSuppressCancellationExceptions(this IAdvancedHalBuilder builder, bool suppress = true)

        {
            return builder.Advanced.WithConfiguration(b => b.WithSuppressCancellationExceptions(suppress));
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

        public static IAdvancedHalBuilder WithHandler<TResult, TContent, TError>(this IAdvancedHalBuilder builder, ITypedHandler handler)
        {
            builder.WithConfiguration(b => b.WithHandler<TResult, TContent, TError>(handler));

            return builder;
        }

        public static IAdvancedHalBuilder WithHandler(this IAdvancedHalBuilder builder, ITypedHandler handler)
        {
            return builder.WithHandler<object, object, object>(handler);
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


        public static IAdvancedHalBuilder WithExceptionFactory(this IAdvancedHalBuilder builder, Func<ErrorContext, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedHalBuilder WithCaching(this IAdvancedHalBuilder builder, bool enabled = true)
        {
            return builder.WithConfiguration(b => b.WithCaching(enabled));
        }

        public static IAdvancedHalBuilder WithNoCache(this IAdvancedHalBuilder builder, bool nocache = true)
        {

            builder.Advanced.WithConfiguration(b => b.WithNoCache(nocache));

            return builder;
        }

        public static IAdvancedHalBuilder WithDependentResources(this IAdvancedHalBuilder builder, params IHalResource[] resources)
        {
            if (resources == null)
                return builder;

            var uris = resources.Select(r => r.Links.Self());

            return builder.WithDependentLinks(uris);
        }

        public static IAdvancedHalBuilder WithDependentLink(this IAdvancedHalBuilder builder, Uri uri)
        {
            return builder.WithOptionalHandlerConfiguration<TypedCacheConfigurationHandler>(h => h.WithDependentUri(uri));
        }

        public static IAdvancedHalBuilder WithDependentLinks(this IAdvancedHalBuilder builder, IEnumerable<Uri> uris)
        {
            return builder.WithOptionalHandlerConfiguration<TypedCacheConfigurationHandler>(h => h.WithDependentUris(uris));
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
    }
}