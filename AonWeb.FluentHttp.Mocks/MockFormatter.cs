using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockFormatter : IMockFormatter
    {
        private readonly MockResponses<IMockTypedRequestContext, IMockResult> _results;
        private readonly MockResponses<IMockTypedRequestContext, IMockResult> _errors;
        private readonly IFormatter _innerFormatter;

        public MockFormatter()
        {
            _results = new MockResponses<IMockTypedRequestContext, IMockResult>();
            _errors = new MockResponses<IMockTypedRequestContext, IMockResult>();
            _innerFormatter = new Formatter();
        }

        public IMockFormatter WithResult<TResult>(Predicate<IMockTypedRequestContext> predicate, Func<IMockTypedRequestContext, IMockResult<TResult>> resultFactory)
        {
            _results.Add(predicate, resultFactory);

            return this;
        }

        public IMockFormatter WithError<TError>(Predicate<IMockTypedRequestContext> predicate, Func<IMockTypedRequestContext, IMockResult<TError>> errorFactory)
        {
            _errors.Add(predicate, errorFactory);

            return this;
        }

        public Task<HttpContent> CreateContent(object value, ITypedBuilderContext context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<object> DeserializeResult(HttpResponseMessage response, ITypedBuilderContext context)
        {
            var result = _results.GetResponse(new MockTypedRequestContext(context, new MockHttpRequestMessage(response.RequestMessage)));

            if (result == null)
                return _innerFormatter.DeserializeResult(response, context);

            return Task.FromResult(result.Result);
        }

        public Task<object> DeserializeError(HttpResponseMessage response, ITypedBuilderContext context)
        {
            var error = _errors.GetResponse(new MockTypedRequestContext(context, new MockHttpRequestMessage(response.RequestMessage)));

            if (error == null)
                return _innerFormatter.DeserializeError(response, context);

            return Task.FromResult(error.Result);
        }
    }
}