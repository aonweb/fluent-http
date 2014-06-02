using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHalCallBuilder<TResult, TContent, TError> : HalCallBuilder<TResult, TContent, TError>, IMockHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
    {
        private readonly MockHttpCallBuilder<TResult, TContent, TError> _innerBuilder;

        protected MockHalCallBuilder()
            : this(MockHttpCallBuilder<TResult, TContent, TError>.CreateMock())
        { }

        private MockHalCallBuilder(MockHttpCallBuilder<TResult, TContent, TError> builder)
            : base(builder)
        {
            _innerBuilder = builder;
        }

        public static MockHalCallBuilder<TResult, TContent, TError> CreateMock()
        {
            return new MockHalCallBuilder<TResult, TContent, TError>();
        }

        public IMockHalCallBuilder<TResult, TContent, TError> ConfigureResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _innerBuilder.ConfigureResult(resultFactory);

            return this;
        }

        public IMockHalCallBuilder<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public IMockHalCallBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode)
        {
            _innerBuilder.ConfigureResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockHalCallBuilder<TResult, TContent, TError> ConfigureError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _innerBuilder.ConfigureError(errorFactory);

            return this;
        }

        public IMockHalCallBuilder<TResult, TContent, TError> WithError(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public IMockHalCallBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode)
        {
            _innerBuilder.ConfigureError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockHalCallBuilder<TResult, TContent, TError> ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.ConfigureResponse(responseFactory);

            return this;
        }

        public IMockHalCallBuilder<TResult, TContent, TError> WithResponse(HttpResponseMessage response)
        {
            return ConfigureResponse(r => response);
        }

        public IMockHalCallBuilder<TResult, TContent, TError> WithResponse(ResponseInfo response)
        {
            return ConfigureResponse(r => response.ToHttpResponseMessage());
        }
    }
}
