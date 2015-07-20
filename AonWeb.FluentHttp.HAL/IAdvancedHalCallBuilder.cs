using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.HAL
{
    public interface IAdvancedHalCallBuilder : IHalCallBuilder
    {
        IAdvancedHalCallBuilder WithContentEncoding(Encoding encoding);
        IAdvancedHalCallBuilder WithMediaType(string mediaType);
        IAdvancedHalCallBuilder WithMethod(string method);
        IAdvancedHalCallBuilder WithMethod(HttpMethod method);
        IAdvancedHalCallBuilder WithAcceptHeader(string mediaType);
        IAdvancedHalCallBuilder WithAcceptCharSet(Encoding encoding);
        IAdvancedHalCallBuilder WithAcceptCharSet(string charSet);

        IAdvancedHalCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IAdvancedHalCallBuilder WithMediaTypeFormatter(MediaTypeFormatter formatter);
        IAdvancedHalCallBuilder ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure)
            where TFormatter : MediaTypeFormatter;

        IAdvancedHalCallBuilder WithHandler<TResult, TContent, TError>(ITypedHttpCallHandler handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder WithHandler(ITypedHttpCallHandler handler);
        IAdvancedHalCallBuilder ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ITypedHttpCallHandler;
        IAdvancedHalCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ITypedHttpCallHandler;

        IAdvancedHalCallBuilder ConfigureHttpHandler<THandler>(Action<THandler> configure)
                   where THandler : class, IHttpCallHandler;
        IAdvancedHalCallBuilder TryConfigureHttpHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;
        IAdvancedHalCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IAdvancedHalCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);

        IAdvancedHalCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedHalCallBuilder WithExceptionFactory(Func<HttpErrorContext, Exception> factory);

        IAdvancedHalCallBuilder WithCaching(bool enabled = true);
        IAdvancedHalCallBuilder WithNoCache(bool nocache = true);
        IAdvancedHalCallBuilder WithDependentResources(params IHalResource[] resources);
        IAdvancedHalCallBuilder WithDependentLink(Uri link);
        IAdvancedHalCallBuilder WithDependentLink(Func<Uri> linkFactory);

        IAdvancedHalCallBuilder OnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSending<TResult, TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSendingAsync<TResult, TContent>(Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSendingAsync<TResult, TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;

        IAdvancedHalCallBuilder OnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler)
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSendingWithContent<TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<object, TContent>> handler)
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSendingWithContentAsync<TContent>(Func<TypedHttpSendingContext<object, TContent>, Task> handler)
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSendingWithContentAsync<TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<object, TContent>, Task> handler)
            where TContent : IHalRequest;

        IAdvancedHalCallBuilder OnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSendingWithResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, object>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSendingWithResultAsync<TResult>(Func<TypedHttpSendingContext<TResult, object>, Task> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSendingWithResultAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, object>, Task> handler)
            where TResult : IHalResource;

        IAdvancedHalCallBuilder OnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSentAsync<TResult>(Func<TypedHttpSentContext<TResult>, Task> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSentAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSentContext<TResult>, Task> handler)
            where TResult : IHalResource;

        IAdvancedHalCallBuilder OnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnResultAsync<TResult>(Func<TypedHttpResultContext<TResult>, Task> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnResultAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpResultContext<TResult>, Task> handler)
            where TResult : IHalResource;

        IAdvancedHalCallBuilder OnError<TError>(Action<TypedHttpErrorContext<TError>> handler);
        IAdvancedHalCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Action<TypedHttpErrorContext<TError>> handler);
        IAdvancedHalCallBuilder OnErrorAsync<TError>(Func<TypedHttpErrorContext<TError>, Task> handler);
        IAdvancedHalCallBuilder OnErrorAsync<TError>(HttpCallHandlerPriority priority, Func<TypedHttpErrorContext<TError>, Task> handler);

        IAdvancedHalCallBuilder OnException(Action<TypedHttpExceptionContext> handler);
        IAdvancedHalCallBuilder OnException(HttpCallHandlerPriority priority, Action<TypedHttpExceptionContext> handler);
        IAdvancedHalCallBuilder OnExceptionAsync(Func<TypedHttpExceptionContext, Task> handler);
        IAdvancedHalCallBuilder OnExceptionAsync(HttpCallHandlerPriority priority, Func<TypedHttpExceptionContext, Task> handler);

        IAdvancedHalCallBuilder WithAutoDecompression(bool enabled = true);
        IAdvancedHalCallBuilder WithSuppressCancellationExceptions(bool suppress = true);
        IAdvancedHalCallBuilder WithSuppressTypeMismatchExceptions(bool suppress = true);
        IAdvancedHalCallBuilder WithTimeout(TimeSpan? timeout);
    }
}