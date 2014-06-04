using System;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockFormatter<TResult, TContent, TError>
        : IHttpCallFormatter<TResult, TContent, TError>,
          IHttpTypedMocker<QueuedMockFormatter<TResult, TContent, TError>, TResult, TContent, TError>
    {
        private readonly IHttpCallFormatter<TResult, TContent, TError> _innerFormatter;

        private readonly ResponseQueue<Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult>> _results;
        private readonly ResponseQueue<Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError>> _errors;

        public QueuedMockFormatter()
        {
            _innerFormatter = new HttpCallFormatter<TResult, TContent, TError>();

            _results = new ResponseQueue<Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult>>((r,c) => default(TResult));
            _errors = new ResponseQueue<Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError>>((r, c) => default(TError));
        }

        public Task<HttpContent> CreateContent<T>(T value, HttpCallContext<TResult, TContent, TError> context)
        {
            return _innerFormatter.CreateContent(value, context);
        }

        public Task<TResult> DeserializeResult(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            var func = _results.GetNext();

            var result = func(response, context);
                
            return Task.FromResult(result);
        }

        public Task<TError> DeserializeError(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context)
        {
            var func = _errors.GetNext();

            var result = func(response, context);

            return Task.FromResult(result);
        }

        public QueuedMockFormatter<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult((r, c) => result);
        }

        public QueuedMockFormatter<TResult, TContent, TError> WithResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _results.Add(resultFactory);

            return this;
        }

        public QueuedMockFormatter<TResult, TContent, TError> WithError(TError error)
        {
            return WithError((r, c) => error);
        }

        public QueuedMockFormatter<TResult, TContent, TError> WithError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _errors.Add(errorFactory);

            return this;
        }
    }
}