using System;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockFormatter : IMockFormatter
    {
        private readonly IHttpCallFormatter _innerFormatter;

        private Func<HttpResponseMessage, TypedHttpCallContext, object> _resultFactory;
        private Func<HttpResponseMessage, TypedHttpCallContext, object> _errorFactory;

        public MockFormatter()
        {
            _innerFormatter = new HttpCallFormatter();
        }

        public Task<HttpContent> CreateContent(object value, TypedHttpCallContext context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<object> DeserializeResult(HttpResponseMessage response, TypedHttpCallContext context)
        {
            if (_resultFactory != null)
                return Task.FromResult(_resultFactory(response, context));

            return _innerFormatter.DeserializeResult(response, context);
        }

        public Task<object> DeserializeError(HttpResponseMessage response, TypedHttpCallContext context)
        {
            if (_errorFactory != null)
                return Task.FromResult(_errorFactory(response, context));

            return _innerFormatter.DeserializeError(response, context);
        }

        public IMockFormatter WithResult<TResult>(TResult result)
        {
            return WithResult((r, c) => result);
        }

        public IMockFormatter WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _resultFactory = (r, c) => resultFactory(r,c);

            return this;
        }

        public IMockFormatter WithError<TError>(TError error)
        {
            return WithError((r, c) => error);
        }

        public IMockFormatter WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _errorFactory = (r, c) => errorFactory(r,c);

            return this;
        }
    }
}