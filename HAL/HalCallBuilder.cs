using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.Fluent.Http;

namespace AonWeb.Fluent.HAL
{
    public class HalCallBuilder<TResult, TContent, TError> : IHttpCallBuilder<TResult, TContent, TError>
    {
        public IHttpCallBuilder<TResult, TContent, TError> WithUri(string uri)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithUri(Uri uri)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithMethod(string method)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFunc, Encoding encoding, string mediaType)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClient> configuration)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureClient(Action<IHttpClientBuilder> configuration)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureRedirection(Action<IRedirectionHandler> configuration)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithRedirectionHandler(Action<HttpRedirectionContext> handler)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> ConfigureErrorHandling(Action<IErrorHandler<TError>> configuration)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithErrorHandler(Action<HttpErrorContext<TError>> handler)
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> WithNoCache()
        {
            throw new NotImplementedException();
        }

        public TResult Result()
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ResultAsync()
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, TError> CancelRequest()
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<T, TContent, TError> WithResultOfType<T>()
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, T, TError> WithContentOfType<T>()
        {
            throw new NotImplementedException();
        }

        public IHttpCallBuilder<TResult, TContent, T> WithErrorsOfType<T>()
        {
            throw new NotImplementedException();
        }
    }
}
