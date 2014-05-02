using System;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IAdvancedHttpCallBuilder : IHttpCallBuilder
    {
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

        IHttpCallBuilder OnSending(Action<HttpCallContext> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpCallContext> handler);
        IHttpCallBuilder OnSending(Func<HttpCallContext, Task> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler);

        IHttpCallBuilder OnSent(Action<HttpCallContext> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpCallContext> handler);
        IHttpCallBuilder OnSent(Func<HttpCallContext, Task> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler);

        IHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler);
    }

    public interface IAdvancedHttpCallBuilder<TResult, TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);

        
        IHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;
        IHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory);
        IHttpCallBuilder<TResult, TContent, TError> WithNoCache();

        IHttpCallBuilder<TResult, TContent, TError> OnSending(Action<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSending(Func<HttpCallContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler);

        IHttpCallBuilder<TResult, TContent, TError> OnSent(Action<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSent(Func<HttpCallContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler);

        IHttpCallBuilder<TResult, TContent, TError> OnResult(Action<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnResult(Func<HttpCallContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler);

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