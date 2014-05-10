﻿using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Formatting;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.HAL.Representations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.HAL
{
    public class HalCallBuilder<TResult, TContent, TError> : IAdvancedHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
        where TError : IHalResource
    {
        private readonly IHttpCallBuilder<TResult, TContent, TError> _innerBuilder;

        private HalCallBuilder()
            : this(HttpCallBuilder<TResult, TContent, TError>.Create()) { }

        private HalCallBuilder(IHttpCallBuilder<TResult, TContent, TError> builder)
        {
            _innerBuilder = builder;
        }

        public static IHalCallBuilder<TResult, TContent, TError> Create()
        {
            return new HalCallBuilder<TResult, TContent, TError>();
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(string link)
        {
            _innerBuilder.WithUri(link);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(Uri link)
        {
            _innerBuilder.WithUri(link);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(Func<string> linkFactory)
        {
            if (linkFactory == null)
                throw new ArgumentNullException("linkFactory");

            return WithLink(linkFactory());
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(Func<Uri> linkFactory)
        {
            if (linkFactory == null)
                throw new ArgumentNullException("linkFactory");

            return WithLink(linkFactory());
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return WithLink(resource.GetLink(key, tokenKey, tokenValue));
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            return WithLink(resource.GetLink(key, tokens));
        }

        public IHalCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value)
        {
            _innerBuilder.WithQueryString(name, value);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithQueryString(NameValueCollection values)
        {
            _innerBuilder.WithQueryString(values);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> AsGet()
        {
            return WithMethod(HttpMethod.Get);
        }

        public IHalCallBuilder<TResult, TContent, TError> AsPut()
        {
            return WithMethod(HttpMethod.Put);
        }

        public IHalCallBuilder<TResult, TContent, TError> AsPost()
        {
            return WithMethod(HttpMethod.Post);
        }

        public IHalCallBuilder<TResult, TContent, TError> AsDelete()
        {
            return WithMethod(HttpMethod.Delete);
        }

        public IHalCallBuilder<TResult, TContent, TError> AsPatch()
        {
            return WithMethod(new HttpMethod("PATCH"));
        }

        public IHalCallBuilder<TResult, TContent, TError> AsHead()
        {
            return WithMethod(HttpMethod.Head);
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithEncoding(Encoding encoding)
        {
            _innerBuilder.Advanced.WithEncoding(encoding);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithMediaType(string mediaType)
        {
            _innerBuilder.Advanced.WithMediaType(mediaType);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithMethod(string method)
        {
            _innerBuilder.Advanced.WithMethod(method);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method)
        {
            _innerBuilder.Advanced.WithMethod(method);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithAcceptHeader(string mediaType)
        {
            _innerBuilder.Advanced.WithAcceptHeader(mediaType);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithAcceptCharSet(Encoding encoding)
        {
            _innerBuilder.Advanced.WithAcceptCharSet(encoding);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithAcceptCharSet(string charSet)
        {
            _innerBuilder.Advanced.WithAcceptCharSet(charSet);

            return this;
        }


        public IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content)
        {
            _innerBuilder.WithContent(content);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding)
        {
            _innerBuilder.WithContent(content, encoding);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType)
        {
            _innerBuilder.WithContent(content, encoding, mediaType);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory)
        {
            _innerBuilder.WithContent(contentFactory);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding)
        {
            _innerBuilder.WithContent(contentFactory, encoding);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding, string mediaType)
        {
            _innerBuilder.WithContent(contentFactory, encoding, mediaType);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result)
        {
            _innerBuilder.WithDefaultResult(result);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFactory)
        {
            _innerBuilder.WithDefaultResult(resultFactory);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.Advanced.ConfigureClient(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithMediaTypeFormatter(MediaTypeFormatter formatter)
        {
            _innerBuilder.Advanced.WithMediaTypeFormatter(formatter);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureMediaTypeFormatter<TFormatter>(Action<TFormatter> configure) where TFormatter : MediaTypeFormatter
        {

            _innerBuilder.Advanced.ConfigureMediaTypeFormatter(configure);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithHandler(IHttpCallHandler<TResult, TContent, TError> handler)
        {
            _innerBuilder.Advanced.WithHandler(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureHandler<THandler>(Action<THandler> configure) where THandler : class, IHttpCallHandler<TResult, TContent, TError>
        {
            _innerBuilder.Advanced.ConfigureHandler(configure);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> TryConfigureHandler<THandler>(Action<THandler> configure) where THandler : class, IHttpCallHandler<TResult, TContent, TError>
        {
            _innerBuilder.Advanced.TryConfigureHandler(configure);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator)
        {
            _innerBuilder.Advanced.WithSuccessfulResponseValidator(validator);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithExceptionFactory(Func<HttpErrorContext<TResult, TContent, TError>, Exception> factory)
        {
            _innerBuilder.Advanced.WithExceptionFactory(factory);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithCaching(bool enabled = true)
        {

            _innerBuilder.Advanced.WithCaching(enabled);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithNoCache(bool nocache = true)
        {
            _innerBuilder.Advanced.WithNoCache(nocache);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithDependentResources(params IHalResource[] resources)
        {
            var uris = resources.Select(r => r.Links.Self);

            _innerBuilder.Advanced.WithDependentUris(uris);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithDependentLink(string link)
        {
            _innerBuilder.Advanced.WithDependentUri(link);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithDependentLink(Func<string> linkFactory)
        {
            if (linkFactory == null)
                throw new ArgumentNullException("linkFactory");

            _innerBuilder.Advanced.WithDependentUri(linkFactory());

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnSending(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnSending(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnSending(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnSending(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnSent(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnSent(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnSent(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnSent(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnResult(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Action<HttpResultContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnResult(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnResult(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnResult(HttpCallHandlerPriority priority, Func<HttpResultContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnResult(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnError(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Action<HttpErrorContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnError(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnError(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnError(HttpCallHandlerPriority priority, Func<HttpErrorContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnError(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnException(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext<TResult, TContent, TError>> handler)
        {
            _innerBuilder.Advanced.OnException(priority, handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnException(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext<TResult, TContent, TError>, Task> handler)
        {
            _innerBuilder.Advanced.OnException(priority, handler);

            return this;
        }
        
        public IHalCallBuilder<TResult, TContent, TError> CancelRequest()
        {
            _innerBuilder.CancelRequest();

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> Advanced { get { return this; } }

        public TResult Result()
        {
            return _innerBuilder.Result();
        }

        public async Task<TResult> ResultAsync()
        {
            return await _innerBuilder.ResultAsync();
        }
    }
}
