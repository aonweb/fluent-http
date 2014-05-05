using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IAdvancedHttpCallBuilder : IHttpCallBuilder
    {
        IHttpCallBuilder WithScheme(string scheme);
        IHttpCallBuilder WithHost(string host);
        IHttpCallBuilder WithPort(int port);
        IHttpCallBuilder WithPath(string absolutePathAndQuery);
        IHttpCallBuilder WithEncoding(Encoding encoding);
        IHttpCallBuilder WithMediaType(string mediaType);
        IHttpCallBuilder WithMethod(string method);
        IHttpCallBuilder WithMethod(HttpMethod method);
        IHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);

        IHttpCallBuilder WithHandler(IHttpCallHandler handler);
        IHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure) 
            where THandler : class, IHttpCallHandler;
        IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IHttpCallBuilder WithNoCache();

        IHttpCallBuilder OnSending(Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler);

        IHttpCallBuilder OnSent(Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler);

        IHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler);
    }

    public interface IAdvancedHttpCallBuilder<TResult, TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> WithScheme(string scheme);
        IHttpCallBuilder<TResult, TContent, TError> WithHost(string host);
        IHttpCallBuilder<TResult, TContent, TError> WithPort(int port);
        IHttpCallBuilder<TResult, TContent, TError> WithPath(string absolutePathAndQuery);
        IHttpCallBuilder<TResult, TContent, TError> WithEncoding(Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithMediaType(string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithMethod(string method);
        IHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        
        IHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;
        IHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory);
        IHttpCallBuilder<TResult, TContent, TError> WithNoCache();

        IHttpCallBuilder<TResult, TContent, TError> OnSending(Action<HttpSendingContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSending(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler);

        IHttpCallBuilder<TResult, TContent, TError> OnSent(Action<HttpSentContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSent(Func<HttpSentContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler);

        IHttpCallBuilder<TResult, TContent, TError> OnResult(Action<HttpResultContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnResult(Func<HttpResultContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler);

        IHttpCallBuilder<TResult, TContent, TError> OnError(Action<HttpErrorContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnError(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler);

        IHttpCallBuilder<TResult, TContent, TError> OnException(Action<HttpExceptionContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnException(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler);
        
    }
}