using System;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHalCallBuilder :
        HalCallBuilder,
        IMockTypedBuilder<QueuedMockHalCallBuilder>
    {
        private readonly QueuedMockTypedHttpCallBuilder _innerBuilder;

        protected QueuedMockHalCallBuilder()
        {
            _innerBuilder = new QueuedMockTypedHttpCallBuilder();
        }

        public static QueuedMockHalCallBuilder CreateMock()
        {
            return new QueuedMockHalCallBuilder();
        }

        public QueuedMockHalCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _innerBuilder.WithResult(resultFactory);

            return this;
        }

        public QueuedMockHalCallBuilder WithResult<TResult>(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public QueuedMockHalCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _innerBuilder.WithResult((r, c) => result);

            return this;
        }

        public QueuedMockHalCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _innerBuilder.WithError(errorFactory);

            return this;
        }

        public QueuedMockHalCallBuilder WithError<TError>(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public QueuedMockHalCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _innerBuilder.WithError((r, c) => error);

            return this;
        }

        public QueuedMockHalCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public QueuedMockHalCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public QueuedMockHalCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}