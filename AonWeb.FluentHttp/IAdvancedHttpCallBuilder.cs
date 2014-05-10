using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    internal interface IChildHttpCallBuilder : IAdvancedHttpCallBuilder
    {
        HttpRequestMessage CreateRequest();
        Task<HttpResponseMessage> ResultFromRequestAsync(HttpRequestMessage request);
    }

    public interface IRecursiveHttpCallBuilder : IAdvancedHttpCallBuilder
    {
        Task<HttpResponseMessage> RecursiveResultAsync();
    }

    public interface IRecursiveHttpCallBuilder<TResult, TContent, TError> : IAdvancedHttpCallBuilder<TResult, TContent, TError>
    {
        Task<TResult> RecursiveResultAsync();
    }

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
        IHttpCallBuilder WithAcceptHeader(string mediaType);
        IHttpCallBuilder WithAcceptCharSet(Encoding encoding);
        IHttpCallBuilder WithAcceptCharSet(string charSet);
        //IHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);
        IHttpCallBuilder WithHandler(IHttpCallHandler handler);
        IHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;
        IHttpCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;
        IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IHttpCallBuilder WithCaching(bool enabled = true);
        IHttpCallBuilder WithNoCache(bool nocache = true);
        IHttpCallBuilder WithDependentUri(string uri);
        IHttpCallBuilder WithDependentUris(IEnumerable<string> uris);

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

        IHttpCallBuilder<TResult, TContent, TError> WithAcceptHeader(string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithAcceptCharSet(Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithAcceptCharSet(string charSet);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder<TResult, TContent, TError> WithMediaTypeFormatter(MediaTypeFormatter formatter);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure)
            where TFormatter : MediaTypeFormatter;

        IHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler);
        IHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;
        IHttpCallBuilder<TResult, TContent, TError> TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;

        IHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory);

        IHttpCallBuilder<TResult, TContent, TError> WithCaching(bool enabled = true);
        IHttpCallBuilder<TResult, TContent, TError> WithNoCache(bool nocache = true);
        IHttpCallBuilder<TResult, TContent, TError> WithDependentUri(string uri);
        IHttpCallBuilder<TResult, TContent, TError> WithDependentUris(IEnumerable<string> uris);

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