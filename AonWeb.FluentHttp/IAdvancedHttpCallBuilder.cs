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
        IAdvancedHttpCallBuilder WithScheme(string scheme);
        IAdvancedHttpCallBuilder WithHost(string host);
        IAdvancedHttpCallBuilder WithPort(int port);
        IAdvancedHttpCallBuilder WithPath(string absolutePathAndQuery);
        IAdvancedHttpCallBuilder WithEncoding(Encoding encoding);
        IAdvancedHttpCallBuilder WithMediaType(string mediaType);
        IAdvancedHttpCallBuilder WithMethod(string method);
        IAdvancedHttpCallBuilder WithMethod(HttpMethod method);
        IAdvancedHttpCallBuilder WithAcceptHeader(string mediaType);
        IAdvancedHttpCallBuilder WithAcceptCharSet(Encoding encoding);
        IAdvancedHttpCallBuilder WithAcceptCharSet(string charSet);
        //IAdvancedHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration);
        IAdvancedHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IAdvancedHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IAdvancedHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);
        IAdvancedHttpCallBuilder WithHandler(IHttpCallHandler handler);
        IAdvancedHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;
        IAdvancedHttpCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;
        IAdvancedHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IAdvancedHttpCallBuilder WithCaching(bool enabled = true);
        IAdvancedHttpCallBuilder WithNoCache(bool nocache = true);
        IAdvancedHttpCallBuilder WithDependentUri(string uri);
        IAdvancedHttpCallBuilder WithDependentUris(IEnumerable<string> uris);

        IAdvancedHttpCallBuilder OnSending(Action<HttpSendingContext> handler);
        IAdvancedHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler);
        IAdvancedHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler);
        IAdvancedHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler);

        IAdvancedHttpCallBuilder OnSent(Action<HttpSentContext> handler);
        IAdvancedHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler);
        IAdvancedHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler);
        IAdvancedHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler);

        IAdvancedHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IAdvancedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler);
        IAdvancedHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler);
        IAdvancedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler);

        IAdvancedHttpCallBuilder WithSuppressCancellationErrors(bool suppress = true);
        IAdvancedHttpCallBuilder WithTimeout(TimeSpan? timeout);
    }

    public interface IAdvancedHttpCallBuilder<TResult, TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithScheme(string scheme);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithHost(string host);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithPort(int port);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithPath(string absolutePathAndQuery);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithEncoding(Encoding encoding);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMediaType(string mediaType);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMethod(string method);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithAcceptHeader(string mediaType);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithAcceptCharSet(Encoding encoding);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithAcceptCharSet(string charSet);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithMediaTypeFormatter(MediaTypeFormatter formatter);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure)
            where TFormatter : MediaTypeFormatter;

        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;
        IAdvancedHttpCallBuilder<TResult, TContent, TError> TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler<TResult, TContent, TError>;

        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithCaching(bool enabled = true);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithNoCache(bool nocache = true);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithDependentUri(string uri);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithDependentUris(IEnumerable<string> uris);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(Action<HttpSendingContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(Action<HttpSentContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(Func<HttpSentContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(Action<HttpResultContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(Func<HttpResultContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(Action<HttpErrorContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(Action<HttpExceptionContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler);

        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithSuppressCancellationErrors(bool suppress = true);
        IAdvancedHttpCallBuilder<TResult, TContent, TError> WithTimeout(TimeSpan? timeout);
    }
}