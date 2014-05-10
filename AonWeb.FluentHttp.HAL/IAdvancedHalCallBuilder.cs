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
    public interface IAdvancedHalCallBuilder<TResult, TContent, TError> : IHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
        where TError : IHalResource
    {
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithEncoding(Encoding encoding);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithMediaType(string mediaType);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithMethod(string method);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithAcceptHeader(string mediaType);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithAcceptCharSet(Encoding encoding);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithAcceptCharSet(string charSet);

        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithMediaTypeFormatter(MediaTypeFormatter formatter);
        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure)
            where TFormatter : MediaTypeFormatter;
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;
        IAdvancedHalCallBuilder<TResult, TContent, TError> TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory);

        IAdvancedHalCallBuilder<TResult, TContent, TError> WithCaching(bool enabled = true);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithNoCache(bool nocache = true);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithDependentResources(params IHalResource[] resources);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithDependentLink(string link);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithDependentLink(Func<string> linkFactory);

        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(Action<HttpSendingContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(Action<HttpSentContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(Func<HttpSentContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(Action<HttpResultContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(Func<HttpResultContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(Action<HttpErrorContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(Action<HttpExceptionContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler);

    }
}