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
            where THandler : class, IHttpCallHandler;
        IAdvancedHalCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;

        IAdvancedHalCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedHalCallBuilder WithExceptionFactory(Func<HttpCallErrorContext, Exception> factory);

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
        IAdvancedHalCallBuilder OnSending<TResult, TContent>(Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IAdvancedHalCallBuilder OnSending<TResult, TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;

        IAdvancedHalCallBuilder OnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSent<TResult>(Func<TypedHttpSentContext<TResult>, Task> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSentContext<TResult>, Task> handler)
            where TResult : IHalResource;

        IAdvancedHalCallBuilder OnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnResult<TResult>(Func<TypedHttpResultContext<TResult>, Task> handler)
            where TResult : IHalResource;
        IAdvancedHalCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpResultContext<TResult>, Task> handler)
            where TResult : IHalResource;

        IAdvancedHalCallBuilder OnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler);
        IAdvancedHalCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Action<TypedHttpCallErrorContext<TError>> handler);
        IAdvancedHalCallBuilder OnError<TError>(Func<TypedHttpCallErrorContext<TError>, Task> handler);
        IAdvancedHalCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Func<TypedHttpCallErrorContext<TError>, Task> handler);

        IAdvancedHalCallBuilder OnException(Action<TypedHttpCallExceptionContext> handler);
        IAdvancedHalCallBuilder OnException(HttpCallHandlerPriority priority, Action<TypedHttpCallExceptionContext> handler);
        IAdvancedHalCallBuilder OnException(Func<TypedHttpCallExceptionContext, Task> handler);
        IAdvancedHalCallBuilder OnException(HttpCallHandlerPriority priority, Func<TypedHttpCallExceptionContext, Task> handler);

        IAdvancedHalCallBuilder WithAutoDecompression(bool enabled = true);
        IAdvancedHalCallBuilder WithSuppressCancellationErrors(bool suppress = true);
        IAdvancedHalCallBuilder WithTimeout(TimeSpan? timeout);
    }
}