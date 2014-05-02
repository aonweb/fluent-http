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
        IHttpCallBuilder WithHandler(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext, Task> handler);
        IHttpCallBuilder WithHandler(IHttpCallHandler<HttpCallContext> handler);
        IHttpCallBuilder WithHandlers(params IHttpCallHandler<HttpCallContext>[] handlers);
        IHttpCallBuilder ConfigureHandler<THandler>(HttpCallHandlerType handlerType, Action<THandler> configure) 
            where THandler : class, IHttpCallHandler<HttpCallContext>;
        IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IHttpCallBuilder WithNoCache();
    }

    public interface IAdvancedHttpCallBuilder<TResult, TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder<TResult, TContent, TError> WithHandler(HttpCallHandlerType handlerType, HttpCallHandlerPriority priority, Func<HttpCallContext<TResult, TContent, TError>, Task> handler);
        IHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<HttpCallContext<TResult, TContent, TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> WithHandlers(params IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>[] handlers);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(HttpCallHandlerType handlerType, Action<THandler> configure) 
            where THandler : class, IHttpCallHandler<HttpCallContext<TResult, TContent, TError>>;
        IHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory);
        IHttpCallBuilder<TResult, TContent, TError> WithNoCache();
    }
}