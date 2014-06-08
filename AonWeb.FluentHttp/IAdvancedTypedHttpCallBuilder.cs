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
                    where THandler : class, IHttpCallHandler;
        IAdvancedTypedHttpCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler;

        IAdvancedTypedHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IAdvancedTypedHttpCallBuilder WithExceptionFactory(Func<HttpCallErrorContext, Exception> factory);

        IAdvancedTypedHttpCallBuilder WithCaching(bool enabled = true);
        IAdvancedTypedHttpCallBuilder WithNoCache(bool nocache = true);
        IAdvancedTypedHttpCallBuilder WithDependentUri(Uri uri);
        IAdvancedTypedHttpCallBuilder WithDependentUris(IEnumerable<Uri> uris);

        IAdvancedTypedHttpCallBuilder OnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler);
        IAdvancedTypedHttpCallBuilder OnSending<TResult, TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, TContent>> handler);
        IAdvancedTypedHttpCallBuilder OnSending<TResult, TContent>(Func<TypedHttpSendingContext<TResult, TContent>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnSending<TResult, TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, TContent>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSentContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnSent<TResult>(Func<TypedHttpSentContext<TResult>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSentContext<TResult>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpResultContext<TResult>> handler);
        IAdvancedTypedHttpCallBuilder OnResult<TResult>(Func<TypedHttpResultContext<TResult>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpResultContext<TResult>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler);
        IAdvancedTypedHttpCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Action<TypedHttpCallErrorContext<TError>> handler);
        IAdvancedTypedHttpCallBuilder OnError<TError>(Func<TypedHttpCallErrorContext<TError>, Task> handler);
        IAdvancedTypedHttpCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Func<TypedHttpCallErrorContext<TError>, Task> handler);
        
        IAdvancedTypedHttpCallBuilder OnException(Action<TypedHttpCallExceptionContext> handler);
        IAdvancedTypedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<TypedHttpCallExceptionContext> handler);
        IAdvancedTypedHttpCallBuilder OnException(Func<TypedHttpCallExceptionContext, Task> handler);
        IAdvancedTypedHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<TypedHttpCallExceptionContext, Task> handler);
        
        IAdvancedTypedHttpCallBuilder WithAutoDecompression(bool enabled = true);
        IAdvancedTypedHttpCallBuilder WithSuppressCancellationErrors(bool suppress = true);
        IAdvancedTypedHttpCallBuilder WithTimeout(TimeSpan? timeout);
    }
}