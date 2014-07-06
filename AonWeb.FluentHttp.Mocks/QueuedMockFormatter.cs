using System;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockFormatter : IMockFormatter
    {
        private readonly IHttpCallFormatter _innerFormatter;

        private readonly ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, Task<object>>> _results;
        private readonly ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, Task<object>>> _errors;

        public QueuedMockFormatter()
        {
            _innerFormatter = new HttpCallFormatter();

            _results = new ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, Task<object>>>(async (r, c) => await _innerFormatter.DeserializeResult(r, c));
            _errors = new ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, Task<object>>>(async (r, c) => await _innerFormatter.DeserializeError(r, c));
        }

        public Task<HttpContent> CreateContent(object value, TypedHttpCallContext context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<object> DeserializeResult(HttpResponseMessage response, TypedHttpCallContext context)
        {
            var func = _results.GetNext();

            var result = func(response, context);
                
            return result;
        }

        public Task<object> DeserializeError(HttpResponseMessage response, TypedHttpCallContext context)
        {
            var func = _errors.GetNext();

            var result = func(response, context);

            return result;
        }

        public IMockFormatter WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _results.Add((r, c) => Task.FromResult<object>(resultFactory(r,c)));

            return this;
        }

        public IMockFormatter WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _errors.Add((r, c) => Task.FromResult<object>(errorFactory(r, c)));

            return this;
        }
    }
}