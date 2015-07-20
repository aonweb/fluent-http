using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Formatting;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.HAL.Representations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.HAL.Serialization;
using AonWeb.FluentHttp.Handlers;

using Newtonsoft.Json.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public class HalCallBuilder : IAdvancedHalCallBuilder
    {
        private readonly IChildTypedHttpCallBuilder _innerBuilder;

        protected HalCallBuilder()
            : this(TypedHttpCallBuilder.CreateAsChild(new HalCallBuilderSettings())) { }

        protected HalCallBuilder(IChildTypedHttpCallBuilder builder)
        {
            _innerBuilder = builder;
            _innerBuilder.ConfigureMediaTypeFormatter<JsonMediaTypeFormatter>(
                f =>
                    {
                        f.SerializerSettings.Converters.Add(new HalResourceConverter());
                        f.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
        }

        public static IHalCallBuilder Create()
        {
            return new HalCallBuilder();
        }

        public IHalCallBuilder WithLink(string link)
        {
            _innerBuilder.WithUri(link);

            return this;
        }

        public IHalCallBuilder WithLink(Uri link)
        {
            _innerBuilder.WithUri(link);

            return this;
        }

        public IHalCallBuilder WithLink(Func<string> linkFactory)
        {
            if (linkFactory == null)
                throw new ArgumentNullException("linkFactory");

            return WithLink(linkFactory());
        }

        public IHalCallBuilder WithLink(Func<Uri> linkFactory)
        {
            if (linkFactory == null)
                throw new ArgumentNullException("linkFactory");

            return WithLink(linkFactory());
        }

        public IHalCallBuilder WithLink(IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return WithLink(resource.GetLink(key, tokenKey, tokenValue));
        }

        public IHalCallBuilder WithLink(IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            return WithLink(resource.GetLink(key, tokens));
        }

        public IHalCallBuilder WithQueryString(string name, string value)
        {
            _innerBuilder.WithQueryString(name, value);

            return this;
        }

        public IHalCallBuilder WithQueryString(NameValueCollection values)
        {
            _innerBuilder.WithQueryString(values);

            return this;
        }

        public IHalCallBuilder AsGet()
        {
            return WithMethod(HttpMethod.Get);
        }

        public IHalCallBuilder AsPut()
        {
            return WithMethod(HttpMethod.Put);
        }

        public IHalCallBuilder AsPost()
        {
            return WithMethod(HttpMethod.Post);
        }

        public IHalCallBuilder AsDelete()
        {
            return WithMethod(HttpMethod.Delete);
        }

        public IHalCallBuilder AsPatch()
        {
            return WithMethod(new HttpMethod("PATCH"));
        }

        public IHalCallBuilder AsHead()
        {
            return WithMethod(HttpMethod.Head);
        }

        public IAdvancedHalCallBuilder WithContentEncoding(Encoding encoding)
        {
            _innerBuilder.WithContentEncoding(encoding);

            return this;
        }

        public IAdvancedHalCallBuilder WithMediaType(string mediaType)
        {
            _innerBuilder.WithMediaType(mediaType);

            return this;
        }

        public IAdvancedHalCallBuilder WithMethod(string method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IAdvancedHalCallBuilder WithMethod(HttpMethod method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IAdvancedHalCallBuilder WithAcceptHeader(string mediaType)
        {
            _innerBuilder.WithAcceptHeader(mediaType);

            return this;
        }

        public IAdvancedHalCallBuilder WithAcceptCharSet(Encoding encoding)
        {
            _innerBuilder.WithAcceptCharSet(encoding);

            return this;
        }

        public IAdvancedHalCallBuilder WithAcceptCharSet(string charSet)
        {
            _innerBuilder.WithAcceptCharSet(charSet);

            return this;
        }

        public IHalCallBuilder WithContent<TContent>(TContent content)
            where TContent : IHalRequest
        {
            _innerBuilder.WithContent(CreateContentFactoryWrapper(content));

            return this;
        }

        public IHalCallBuilder WithContent<TContent>(TContent content, Encoding encoding)
            where TContent : IHalRequest
        {
            _innerBuilder.WithContent(CreateContentFactoryWrapper(content), encoding);

            return this;
        }

        public IHalCallBuilder WithContent<TContent>(TContent content, Encoding encoding, string mediaType)
            where TContent : IHalRequest
        {
            _innerBuilder.WithContent(CreateContentFactoryWrapper(content), encoding, mediaType);

            return this;
        }

        public IHalCallBuilder WithContent<TContent>(Func<TContent> contentFactory)
            where TContent : IHalRequest
        {
            _innerBuilder.WithContent(CreateContentFactoryWrapper(contentFactory));

            return this;
        }

        public IHalCallBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding)
            where TContent : IHalRequest
        {
            _innerBuilder.WithContent(CreateContentFactoryWrapper(contentFactory), encoding);

            return this;
        }

        public IHalCallBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding, string mediaType)
            where TContent : IHalRequest
        {
            _innerBuilder.WithContent(CreateContentFactoryWrapper(contentFactory), encoding, mediaType);

            return this;
        }

        public IHalCallBuilder WithDefaultResult<TResult>(TResult result)
            where TResult : IHalResource
        {
            _innerBuilder.WithDefaultResult(result);

            return this;
        }

        public IHalCallBuilder WithDefaultResult<TResult>(Func<TResult> resultFactory)
            where TResult : IHalResource
        {
            _innerBuilder.WithDefaultResult(resultFactory);

            return this;
        }

        public IHalCallBuilder WithErrorType<TError>()
        {
            _innerBuilder.WithErrorType<TError>();

            return this;
        }

        public IAdvancedHalCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.ConfigureClient(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder WithMediaTypeFormatter(MediaTypeFormatter formatter)
        {
            _innerBuilder.WithMediaTypeFormatter(formatter);

            return this;
        }

        public IAdvancedHalCallBuilder ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure)
            where TFormatter : MediaTypeFormatter
        {

            _innerBuilder.ConfigureMediaTypeFormatter(configure);

            return this;
        }

        public IAdvancedHalCallBuilder WithHandler<TResult, TContent, TError>(ITypedHttpCallHandler handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            _innerBuilder.WithHandler<TResult, TContent, TError>(handler);

            return this;
        }

        public IAdvancedHalCallBuilder WithHandler(ITypedHttpCallHandler handler)
        {

            return WithHandler<IHalResource, IHalRequest, object>(handler);
        }

        public IAdvancedHalCallBuilder ConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ITypedHttpCallHandler
        {
            _innerBuilder.ConfigureHandler(configure);

            return this;
        }

        public IAdvancedHalCallBuilder TryConfigureHandler<THandler>(Action<THandler> configure)
            where THandler : class, ITypedHttpCallHandler
        {
            _innerBuilder.TryConfigureHandler(configure);

            return this;
        }

        public IAdvancedHalCallBuilder ConfigureHttpHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler
        {
            _innerBuilder.ConfigureHttpHandler(configure);

            return this;
        }

        public IAdvancedHalCallBuilder TryConfigureHttpHandler<THandler>(Action<THandler> configure)
            where THandler : class, IHttpCallHandler
        {
            _innerBuilder.TryConfigureHttpHandler(configure);

            return this;
        }

        public IAdvancedHalCallBuilder ConfigureRetries(Action<RetryHandler> configuration)
        {
            _innerBuilder.ConfigureRetries(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration)
        {
            _innerBuilder.ConfigureRedirect(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            _innerBuilder.WithSuccessfulResponseValidator(validator);

            return this;
        }

        public IAdvancedHalCallBuilder WithExceptionFactory(Func<HttpErrorContext, Exception> factory)
        {
            _innerBuilder.WithExceptionFactory(factory);

            return this;
        }

        public IAdvancedHalCallBuilder WithCaching(bool enabled = true)
        {

            _innerBuilder.WithCaching(enabled);

            return this;
        }

        public IAdvancedHalCallBuilder WithNoCache(bool nocache = true)
        {
            _innerBuilder.WithNoCache(nocache);

            return this;
        }

        public IAdvancedHalCallBuilder WithDependentResources(params IHalResource[] resources)
        {
            if (resources == null) 
                return this;

            var uris = resources.Select(r => r.Links.Self());

            _innerBuilder.WithDependentUris(uris);

            return this;
        }

        public IAdvancedHalCallBuilder WithDependentLink(Uri link)
        {
            _innerBuilder.WithDependentUri(link);

            return this;
        }

        public IAdvancedHalCallBuilder WithDependentLink(Func<Uri> linkFactory)
        {
            if (linkFactory == null)
                throw new ArgumentNullException("linkFactory");

            _innerBuilder.WithDependentUri(linkFactory());

            return this;
        }

        public IAdvancedHalCallBuilder OnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            _innerBuilder.OnSending(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSending<TResult, TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            _innerBuilder.OnSending(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingAsync<TResult, TContent>(Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            _innerBuilder.OnSendingAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingAsync<TResult, TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, TContent>, Task> handler)
            where TResult : IHalResource
            where TContent : IHalRequest
        {
            _innerBuilder.OnSendingAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler) where TContent : IHalRequest
        {
            _innerBuilder.OnSendingWithContent(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithContent<TContent>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<object, TContent>> handler) where TContent : IHalRequest
        {
            _innerBuilder.OnSendingWithContent(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithContentAsync<TContent>(Func<TypedHttpSendingContext<object, TContent>, Task> handler) where TContent : IHalRequest
        {
            _innerBuilder.OnSendingWithContentAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithContentAsync<TContent>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<object, TContent>, Task> handler) where TContent : IHalRequest
        {
            _innerBuilder.OnSendingWithContentAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler) where TResult : IHalResource
        {
            _innerBuilder.OnSendingWithResult(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSendingContext<TResult, object>> handler) where TResult : IHalResource
        {
            _innerBuilder.OnSendingWithResult(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithResultAsync<TResult>(Func<TypedHttpSendingContext<TResult, object>, Task> handler) where TResult : IHalResource
        {
            _innerBuilder.OnSendingWithResultAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSendingWithResultAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSendingContext<TResult, object>, Task> handler) where TResult : IHalResource
        {
            _innerBuilder.OnSendingWithResultAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnSent(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSent<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnSent(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSentAsync<TResult>(Func<TypedHttpSentContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnSentAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnSentAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpSentContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnSentAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnResult(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnResult<TResult>(HttpCallHandlerPriority priority, Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnResult(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnResultAsync<TResult>(Func<TypedHttpResultContext<TResult>, Task> handler) where TResult : IHalResource
        {
            _innerBuilder.OnResultAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnResultAsync<TResult>(HttpCallHandlerPriority priority, Func<TypedHttpResultContext<TResult>, Task> handler)
            where TResult : IHalResource
        {
            _innerBuilder.OnResultAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnError<TError>(Action<TypedHttpErrorContext<TError>> handler)
        {
            _innerBuilder.OnError(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnError<TError>(HttpCallHandlerPriority priority, Action<TypedHttpErrorContext<TError>> handler)
        {
            _innerBuilder.OnError(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnErrorAsync<TError>(Func<TypedHttpErrorContext<TError>, Task> handler)
        {
            _innerBuilder.OnErrorAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnErrorAsync<TError>(HttpCallHandlerPriority priority, Func<TypedHttpErrorContext<TError>, Task> handler)
        {
            _innerBuilder.OnErrorAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnException(Action<TypedHttpExceptionContext> handler)
        {
            _innerBuilder.OnException(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnException(HttpCallHandlerPriority priority, Action<TypedHttpExceptionContext> handler)
        {
            _innerBuilder.OnException(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnExceptionAsync(Func<TypedHttpExceptionContext, Task> handler)
        {
            _innerBuilder.OnExceptionAsync(handler);

            return this;
        }

        public IAdvancedHalCallBuilder OnExceptionAsync(HttpCallHandlerPriority priority, Func<TypedHttpExceptionContext, Task> handler)
        {
            _innerBuilder.OnExceptionAsync(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder WithAutoDecompression(bool enabled = true)
        {
            _innerBuilder.WithAutoDecompression(enabled);

            return this;
        }

        public IAdvancedHalCallBuilder WithSuppressCancellationExceptions(bool suppress = true)
        {
            _innerBuilder.WithSuppressCancellationExceptions(suppress);

            return this;
        }

        public IAdvancedHalCallBuilder WithSuppressTypeMismatchExceptions(bool suppress = true)
        {
            _innerBuilder.WithSuppressTypeMismatchExceptions(suppress);

            return this;
        }

        public IAdvancedHalCallBuilder WithTimeout(TimeSpan? timeout)
        {
            _innerBuilder.WithTimeout(timeout);

            return this;
        }

        public IHalCallBuilder CancelRequest()
        {
            _innerBuilder.CancelRequest();

            return this;
        }

        public IAdvancedHalCallBuilder Advanced { get { return this; } }

        public async Task<TResult> ResultAsync<TResult>()
            where TResult : IHalResource
        {
            return await _innerBuilder.ResultAsync<TResult>().ConfigureAwait(false);
        }

        public async Task SendAsync()
        {
            await _innerBuilder.SendAsync().ConfigureAwait(false);
        }

        private Func<TContent> CreateContentFactoryWrapper<TContent>(TContent content)
            where TContent : IHalRequest
        {
            return () =>
            {
                if (!ReferenceEquals(content, null))
                    _innerBuilder.WithDependentUris(content.DependentUris);

                return content;
            };
        }

        private Func<TContent> CreateContentFactoryWrapper<TContent>(Func<TContent> contentFactory)
            where TContent : IHalRequest
        {
            if (contentFactory == null)
                throw new ArgumentNullException("contentFactory");

            return () =>
            {
                var content = contentFactory();

                _innerBuilder.WithDependentUris(content.DependentUris);

                return content;
            };
        }
    }
}
