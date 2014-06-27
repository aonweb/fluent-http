using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IChildHttpCallBuilder : IAdvancedHttpCallBuilder
    {
        HttpRequestMessage CreateRequest();
        Task<HttpResponseMessage> ResultFromRequestAsync(HttpRequestMessage request);

        void ApplySettings(TypedHttpCallBuilderSettings settings);
    }

    public interface IRecursiveHttpCallBuilder : IAdvancedHttpCallBuilder
    {
        Task<HttpResponseMessage> RecursiveResultAsync();
    }

    public interface IAdvancedHttpCallBuilder : IHttpCallBuilder
    {
        IAdvancedHttpCallBuilder WithScheme(string scheme);
        IAdvancedHttpCallBuilder WithHost(string host);
        IAdvancedHttpCallBuilder WithPort(int port);
        IAdvancedHttpCallBuilder WithPath(string absolutePathAndQuery);
        IAdvancedHttpCallBuilder WithContentEncoding(Encoding encoding);
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
        IAdvancedHttpCallBuilder WithDependentUri(Uri uri);
        IAdvancedHttpCallBuilder WithDependentUris(IEnumerable<Uri> uris);

        IAdvancedHttpCallBuilder OnSending(Action<HttpSendingContext> handler);
        IAdvancedHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler);
        IAdvancedHttpCallBuilder OnSendingAsync(Func<HttpSendingContext, Task> handler);
        IAdvancedHttpCallBuilder OnSendingAsync(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler);

        IAdvancedHttpCallBuilder OnSent(Action<HttpSentContext> handler);
        IAdvancedHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler);
        IAdvancedHttpCallBuilder OnSentAsync(Func<HttpSentContext, Task> handler);
        IAdvancedHttpCallBuilder OnSentAsync(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler);

        IAdvancedHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IAdvancedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler);
        IAdvancedHttpCallBuilder OnExceptionAsync(Func<HttpExceptionContext, Task> handler);
        IAdvancedHttpCallBuilder OnExceptionAsync(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler);

        IAdvancedHttpCallBuilder WithAutoDecompression(bool enabled = true);
        IAdvancedHttpCallBuilder WithSuppressCancellationExceptions(bool suppress = true);
        IAdvancedHttpCallBuilder WithTimeout(TimeSpan? timeout);
    }
}