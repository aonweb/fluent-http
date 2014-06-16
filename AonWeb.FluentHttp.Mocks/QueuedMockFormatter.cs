using System;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockFormatter
        : IHttpCallFormatter,
        IHttpTypedMocker<QueuedMockFormatter>
    {
        private readonly IHttpCallFormatter _innerFormatter;

        private readonly ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, object>> _results;
        private readonly ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, object>> _errors;

        public QueuedMockFormatter()
        {
            _innerFormatter = new HttpCallFormatter();

            _results = new ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, object>>((r,c) => default(object));
            _errors = new ResponseQueue<Func<HttpResponseMessage, TypedHttpCallContext, object>>((r, c) => default(object));
        }

        public Task<HttpContent> CreateContent(object value, TypedHttpCallContext context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<object> DeserializeResult(HttpResponseMessage response, TypedHttpCallContext context)
        {
            var func = _results.GetNext();

            var result = func(response, context);
                
            return Task.FromResult(result);
        }

        public Task<object> DeserializeError(HttpResponseMessage response, TypedHttpCallContext context)
        {
            var func = _errors.GetNext();

            var result = func(response, context);

            return Task.FromResult(result);
        }

        public QueuedMockFormatter WithResult<TResult>(TResult result)
        {
            return WithResult((r, c) => result);
        }

        public QueuedMockFormatter WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _results.Add((r, c) => resultFactory(r,c));

            return this;
        }

        public QueuedMockFormatter WithError<TError>(TError error)
        {
            return WithError((r, c) => error);
        }

        public QueuedMockFormatter WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _errors.Add((r, c) => errorFactory(r,c));

            return this;
        }
    }
}