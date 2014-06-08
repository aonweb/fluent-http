using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class QueuedMockHalCallBuilder<TResult, TContent, TError> :
        HalCallBuilder<TResult, TContent, TError>,
        IMockBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
    {
        private readonly QueuedMockTypedHttpCallBuilder<TResult, TContent, TError> _innerBuilder;

        protected QueuedMockHalCallBuilder()
        {
            _innerBuilder = new QueuedMockTypedHttpCallBuilder<TResult, TContent, TError>();
        }

        public static QueuedMockHalCallBuilder<TResult, TContent, TError> CreateMock()
        {
            return new QueuedMockHalCallBuilder<TResult, TContent, TError>();
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _innerBuilder.WithResult(resultFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode)
        {
            _innerBuilder.WithResult((r, c) => result);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _innerBuilder.WithError(errorFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithError(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public IMockBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode)
        {
            _innerBuilder.WithError((r, c) => error);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public IMockBuilder<TResult, TContent, TError> WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}