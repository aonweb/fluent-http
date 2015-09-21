using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp
{
    public static class AdvancedHttpBuilderExtensions
    {

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

        public static IAdvancedHttpBuilder WithHandler(this IAdvancedHttpBuilder builder, IHandler handler)
        {
            builder.WithConfiguration(s => s.Handler.WithHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder WithHandlerConfiguration<THandler>(this IAdvancedHttpBuilder builder, Action<THandler> configure)
            where THandler : class, IHandler
        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configure));

            return builder;
        }

        public static IAdvancedHttpBuilder WithOptionalHandlerConfiguration<THandler>(this IAdvancedHttpBuilder builder, Action<THandler> configure)
            where THandler : class, IHandler
        {
            builder.WithConfiguration(s => s.Handler.WithConfiguration(configure, false));

            return builder;
        }

        public static IAdvancedHttpBuilder WithSuccessfulResponseValidator(this IAdvancedHttpBuilder builder, Func<HttpResponseMessage, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            builder.WithConfiguration(s => s.SuccessfulResponseValidators.Add(validator));

            return builder;
        }

        public static IAdvancedHttpBuilder WithExceptionFactory(this IAdvancedHttpBuilder builder, Func<HttpResponseMessage, Exception> factory)
        {
            builder.WithConfiguration(s => s.ExceptionFactory = factory);

            return builder;
        }

        public static IAdvancedHttpBuilder WithCaching(this IAdvancedHttpBuilder builder, bool enabled = true)

        {
            return builder.WithHandlerConfiguration<HttpCacheHandler>(handler => handler.WithCaching(enabled));
        }

        public static IAdvancedHttpBuilder WithDependentUri(this IAdvancedHttpBuilder builder, Uri uri)

        {
            return builder.WithOptionalHandlerConfiguration<HttpCacheHandler>(h => h.WithDependentUri(uri));
        }

        public static IAdvancedHttpBuilder WithDependentUris(this IAdvancedHttpBuilder builder, IEnumerable<Uri> uris)

        {
            return builder.WithOptionalHandlerConfiguration<HttpCacheHandler>(h => h.WithDependentUris(uris));
        }

        public static IAdvancedHttpBuilder OnSending(this IAdvancedHttpBuilder builder, Action<SendingContext> handler)

        {
            builder.WithConfiguration(s => s.Handler.WithSendingHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSending(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<SendingContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSendingAsync(this IAdvancedHttpBuilder builder, Func<SendingContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSendingAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<SendingContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncSendingHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSent(this IAdvancedHttpBuilder builder, Action<SentContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithSentHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSent(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<SentContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSentAsync(this IAdvancedHttpBuilder builder, Func<SentContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncSentHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnSentAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<SentContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncSentHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnException(this IAdvancedHttpBuilder builder, Action<ExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnException(this IAdvancedHttpBuilder builder, HandlerPriority priority, Action<ExceptionContext> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithExceptionHandler(priority, handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnExceptionAsync(this IAdvancedHttpBuilder builder, Func<ExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncExceptionHandler(handler));

            return builder;
        }

        public static IAdvancedHttpBuilder OnExceptionAsync(this IAdvancedHttpBuilder builder, HandlerPriority priority, Func<ExceptionContext, Task> handler)
        {
            builder.WithConfiguration(s => s.Handler.WithAsyncExceptionHandler(priority, handler));

            return builder;
        }
    }
}