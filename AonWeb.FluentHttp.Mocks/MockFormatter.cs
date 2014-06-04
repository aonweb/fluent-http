using System;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockFormatter<TResult, TContent, TError>
        : IHttpCallFormatter<TResult, TContent, TError>,
        IHttpTypedMocker<MockFormatter<TResult, TContent, TError>, TResult, TContent, TError>
    {
        private readonly IHttpCallFormatter<TResult, TContent, TError> _innerFormatter;

        private Func<HttpResponseMessage,HttpCallContext<TResult, TContent, TError>, TResult> _resultFactory;
        private Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> _errorFactory;

        public MockFormatter()
        {
            _innerFormatter = new HttpCallFormatter<TResult, TContent, TError>();
        }

        public Task<HttpContent> CreateContent<T>(T value, HttpCallContext<TResult, TContent, TError> context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<TResult> DeserializeResult(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            if (_resultFactory != null) 
                return Task.FromResult(_resultFactory(response, context));

            return _innerFormatter.DeserializeResult(response, context);
        }

        public Task<TError> DeserializeError(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            if (_errorFactory != null)
                return Task.FromResult(_errorFactory(response, context));

            return _innerFormatter.DeserializeError(response, context);
        }

        public MockFormatter<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult((r, c) => result);
        }

        public MockFormatter<TResult, TContent, TError> WithResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _resultFactory = resultFactory;

            return this;
        }

        public MockFormatter<TResult, TContent, TError> WithError(TError error)
        {
            return WithError((r, c) => error);
        }

        public MockFormatter<TResult, TContent, TError> WithError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _errorFactory = errorFactory;

            return this;
        }
    }
}