using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.Fluent.HAL.Representations;
using AonWeb.Fluent.Http;
using AonWeb.Fluent.Http.Client;
using AonWeb.Fluent.Http.Handlers;
using AonWeb.Fluent.Http.Serialization;

namespace AonWeb.Fluent.HAL
{
    public interface IHalCallBuilder<TResult, in TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
        where TError : IHalResource
    {
        IHalCallBuilder<TResult, TContent, TError> WithLink(string link);
        IHalCallBuilder<TResult, TContent, TError> WithLink(Uri link);
        IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key);
        IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key, string tokenKey, object tokenValue);
        IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key, IDictionary<string, object> tokens);
        IHalCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value);
        IHalCallBuilder<TResult, TContent, TError> WithMethod(string method);
        IHalCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method);
        IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content);
        IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding);
        IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType);
        IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc);
        IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding);
        IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding, string mediaType);
        IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result);
        IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFunc);

        TResult Result();
        Task<TResult> ResultAsync();

        IHalCallBuilder<TResult, TContent, TError> CancelRequest();

        // conversion methods
        IAdvancedHalCallBuilder<TResult, TContent, TError> Advanced { get; }

        IHalCallBuilder<T, TContent, TError> WithResultOfType<T>()
            where T : IHalResource;
        IHalCallBuilder<TResult, T, TError> WithContentOfType<T>()
            where T : IHalRequest;
        IHalCallBuilder<TResult, TContent, T> WithErrorsOfType<T>()
            where T : IHalResource;
    }

    public interface IAdvancedHalCallBuilder<TResult, in TContent, TError> : IHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
        where TError : IHalResource
    {
        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration);
        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration);
        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureRedirect(Action<IRedirectHandler> configuration);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithRedirectHandler(Action<HttpRedirectContext> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureErrorHandling(Action<IErrorHandler<TError>> configuration);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithExceptionHandler(Action<HttpExceptionContext> handler);
        IAdvancedHalCallBuilder<TResult, TContent, TError> WithNoCache();
    }


    public class HalCallBuilder<TResult, TContent, TError> : IAdvancedHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
        where TError : IHalResource
    {
        private readonly IHttpCallBuilder<TResult, TContent, TError> _innerBuilder;

        public HalCallBuilder()
            : this(new HttpCallBuilder<TResult, TContent, TError>()) { }

        internal HalCallBuilder(IHttpCallBuilder<TResult, TContent, TError> builder)
        { 
            _innerBuilder = builder;
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

        public IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key)
        {
            return WithLink(resource.GetLink(key));
        }

        public IHalCallBuilder<TResult, TContent, TError> WithLink(IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return WithLink(resource.GetLink(key,tokenKey, tokenValue));
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

        public IHalCallBuilder<TResult, TContent, TError> WithMethod(string method)
        {
            _innerBuilder. WithMethod(method);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method)
        {
            _innerBuilder.WithMethod(method);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content)
        {
            _innerBuilder.WithContent(content);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding)
        {
            _innerBuilder.WithContent( content,  encoding);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType)
        {
            _innerBuilder.WithContent( content,  encoding,  mediaType);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc)
        {
            _innerBuilder.WithContent(contentFunc);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding)
        {
            _innerBuilder. WithContent(contentFunc, encoding);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding, string mediaType)
        {
            _innerBuilder.WithContent(contentFunc,  encoding,  mediaType);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result)
        {
            _innerBuilder.WithDefaultResult( result);

            return this;
        }

        public IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFunc)
        {
            _innerBuilder.WithDefaultResult(resultFunc);

            return this;
        }

        public TResult Result()
        {
            return _innerBuilder.Result();
        }

        public async Task<TResult> ResultAsync()
        {
            return await _innerBuilder.ResultAsync();
        }

        public IHalCallBuilder<TResult, TContent, TError> CancelRequest()
        {
            _innerBuilder.CancelRequest();

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> Advanced { get { return this; } }

        public IHalCallBuilder<T, TContent, TError> WithResultOfType<T>() where T : IHalResource
        {
            return new HalCallBuilder<T, TContent, TError>(_innerBuilder.WithResultOfType<T>());
        }

        public IHalCallBuilder<TResult, T, TError> WithContentOfType<T>() where T : IHalRequest
        {
            return new HalCallBuilder<TResult, T, TError>(_innerBuilder.WithContentOfType<T>());
        }

        public IHalCallBuilder<TResult, TContent, T> WithErrorsOfType<T>() where T : IHalResource
        {
            return new HalCallBuilder<TResult, TContent, T>(_innerBuilder.WithErrorsOfType<T>());
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration)
        {
            _innerBuilder.Advanced.ConfigureClient(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            _innerBuilder.Advanced.ConfigureClient(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureRedirect(Action<IRedirectHandler> configuration)
        {
            _innerBuilder.Advanced.ConfigureRedirect(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithRedirectHandler(Action<HttpRedirectContext> handler)
        {
            _innerBuilder.Advanced.WithRedirectHandler(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> ConfigureErrorHandling(Action<IErrorHandler<TError>> configuration)
        {
            _innerBuilder.Advanced.ConfigureErrorHandling(configuration);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler)
        {
            _innerBuilder.Advanced.WithErrorHandler(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithExceptionHandler(Action<HttpExceptionContext> handler)
        {
            _innerBuilder.Advanced.WithExceptionHandler(handler);

            return this;
        }

        public IAdvancedHalCallBuilder<TResult, TContent, TError> WithNoCache()
        {
            _innerBuilder.Advanced.WithNoCache();

            return this;
        }
    }
}
