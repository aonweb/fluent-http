using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockTypedHttpCallBuilder :
        TypedHttpCallBuilder,
        IMockBuilder<QueuedMockTypedHttpCallBuilder>
    {
        private readonly QueuedMockFormatter _formatter;
        private readonly QueuedMockHttpCallBuilder _innerBuilder;

        protected internal QueuedMockTypedHttpCallBuilder()
        {
            _innerBuilder = new QueuedMockHttpCallBuilder();
            _formatter = new QueuedMockFormatter();
        }

        public static QueuedMockTypedHttpCallBuilder CreateMock()
        {
            return new QueuedMockTypedHttpCallBuilder();
        }

        public static QueuedMockTypedHttpCallBuilder CreateMock(string baseUri)
        {
            return (QueuedMockTypedHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public static QueuedMockTypedHttpCallBuilder CreateMock(Uri baseUri)
        {
            return (QueuedMockTypedHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public QueuedMockTypedHttpCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _formatter.WithResult(resultFactory);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithResult<TResult>(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public QueuedMockTypedHttpCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _formatter.WithResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public QueuedMockTypedHttpCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _formatter.WithError(errorFactory);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithError<TError>(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public QueuedMockTypedHttpCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _formatter.WithError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public QueuedMockTypedHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public QueuedMockTypedHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public QueuedMockTypedHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}