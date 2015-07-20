using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IChildTypedHttpCallBuilder : IRecursiveTypedHttpCallBuilder 
    {
        void ApplySettings(TypedHttpCallBuilderSettings settings);
    }

    public interface IRecursiveTypedHttpCallBuilder : IAdvancedTypedHttpCallBuilder {
        Task<TResult> RecursiveResultAsync<TResult>();
    }

    public interface IAdvancedTypedHttpCallBuilder : ITypedHttpCallBuilder
    {
        IAdvancedTypedHttpCallBuilder WithScheme(string scheme);
        IAdvancedTypedHttpCallBuilder WithHost(string host);
        IAdvancedTypedHttpCallBuilder WithPort(int port);
        IAdvancedTypedHttpCallBuilder WithPath(string absolutePathAndQuery);
        IAdvancedTypedHttpCallBuilder WithContentEncoding(Encoding encoding);
        IAdvancedTypedHttpCallBuilder WithMediaType(string mediaType);
        IAdvancedTypedHttpCallBuilder WithMethod(string method);
        IAdvancedTypedHttpCallBuilder WithMethod(HttpMethod method);

        IAdvancedTypedHttpCallBuilder WithAcceptHeader(string mediaType);
        IAdvancedTypedHttpCallBuilder WithAcceptCharSet(Encoding encoding);
        IAdvancedTypedHttpCallBuilder WithAcceptCharSet(string charSet);
        IAdvancedTypedHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IAdvancedTypedHttpCallBuilder WithMediaTypeFormatter(MediaTypeFormatter formatter);
        IAdvancedTypedHttpCallBuilder ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure)
            where TFormatter : MediaTypeFormatter;

        IAdvancedTypedHttpCallBuilder WithHandler<TResult, TContent, TError>(ITypedHttpCallHandler handler);
        IAdvancedTypedHttpCallBuilder WithHandler(ITypedHttpCallHandler handler);
        IAdvancedTypedHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure)
                    where THandler : class, ITypedHttpCallHandler;
        IAdvancedTypedHttpCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ITypedHttpCallHandler;

        IAdvancedTypedHttpCallBuilder ConfigureHttpHandler<THandler>(Action<THandler> configure)
                    where THandler : class, IHttpCallHandler;
        IAdvancedTypedHttpCallBuilder TryConfigureHttpHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;
        IAdvancedTypedHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IAdvancedTypedHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);

        IAdvancedTypedHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedTypedHttpCallBuilder WithExceptionFactory(Func<HttpErrorContext, Exception> factory);

        IAdvancedTypedHttpCallBuilder WithCaching(bool enabled = true);
        IAdvancedTypedHttpCallBuilder WithNoCache(bool nocache = true);
        IAdvancedTypedHttpCallBuilder WithDependentUri(Uri uri);
        IAdvancedTypedHttpCallBuilder WithDependentUris(IEnumerable<Uri> uris);

        IAdvancedTypedHttpCallBuilder OnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler);
        IAdvancedTypedHttpCallBuilder OnSending<TResult, TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, TContent>> handler);
        IAdvancedTypedHttpCallBuilder OnSendingAsync<TResult, TContent>(Func<TypedHttpSendingContext<TResult, TContent>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnSendingAsync<TResult, TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, TContent>, Task> handler);

        IAdvancedTypedHttpCallBuilder OnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler);
        IAdvancedTypedHttpCallBuilder OnSendingWithContent<TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<object, TContent>> handler);
        IAdvancedTypedHttpCallBuilder OnSendingWithContentAsync<TContent>(Func<TypedHttpSendingContext<object, TContent>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnSendingWithContentAsync<TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<object, TContent>, Task> handler);

        IAdvancedTypedHttpCallBuilder OnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler);
        IAdvancedTypedHttpCallBuilder OnSendingWithResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, object>> handler);
        IAdvancedTypedHttpCallBuilder OnSendingWithResultAsync<TResult>(Func<TypedHttpSendingContext<TResult, object>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnSendingWithResultAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, object>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSentContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnSentAsync<TResult>(Func<TypedHttpSentContext<TResult>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnSentAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSentContext<TResult>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpResultContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnResultAsync<TResult>(Func<TypedHttpResultContext<TResult>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnResultAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpResultContext<TResult>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnError<TError>(Action<TypedHttpErrorContext<TError>> handler);
        IAdvancedTypedHttpCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Action<TypedHttpErrorContext<TError>> handler);
        IAdvancedTypedHttpCallBuilder OnErrorAsync<TError>(Func<TypedHttpErrorContext<TError>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnErrorAsync<TError>(HttpCallHandlerPriority priority, Func<TypedHttpErrorContext<TError>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnException(Action<TypedHttpExceptionContext> handler);
        IAdvancedTypedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<TypedHttpExceptionContext> handler);
        IAdvancedTypedHttpCallBuilder OnExceptionAsync(Func<TypedHttpExceptionContext, Task> handler);
        IAdvancedTypedHttpCallBuilder OnExceptionAsync(HttpCallHandlerPriority priority, Func<TypedHttpExceptionContext, Task> handler);
        
        IAdvancedTypedHttpCallBuilder WithAutoDecompression(bool enabled = true);
        IAdvancedTypedHttpCallBuilder WithSuppressCancellationExceptions(bool suppress = true);
        IAdvancedTypedHttpCallBuilder WithSuppressTypeMismatchExceptions(bool suppress = true);
        IAdvancedTypedHttpCallBuilder WithTimeout(TimeSpan? timeout);
    }
}