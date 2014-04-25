using System;
using AonWeb.Fluent.Http.Client;
using AonWeb.Fluent.Http.Handlers;

namespace AonWeb.Fluent.Http
{
    public interface IAdvancedHttpCallBuilder : IHttpCallBuilder
    {
        IHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder ConfigureRedirect(Action<IRedirectHandler> configuration);
        IHttpCallBuilder WithRedirectHandler(Action<HttpRedirectContext> handler);
        IHttpCallBuilder WithNoCache();
    }

    public interface IAdvancedHttpCallBuilder<TResult, in TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureRedirect(Action<IRedirectHandler> configuration);
        IHttpCallBuilder<TResult, TContent, TError> WithRedirectHandler(Action<HttpRedirectContext> handler);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureErrorHandling(Action<IErrorHandler<TError>> configuration);
        IHttpCallBuilder<TResult, TContent, TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler);
        IHttpCallBuilder<TResult, TContent, TError> WithExceptionHandler(Action<HttpExceptionContext> handler);
        IHttpCallBuilder<TResult, TContent, TError> WithNoCache();
    }
}