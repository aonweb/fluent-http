using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHalCallBuilder : 
        HalCallBuilder,
        IMockTypedBuilder<MockHalCallBuilder>
    {
        private readonly MockTypedHttpCallBuilder _innerBuilder;

        protected MockHalCallBuilder()
            : this(MockTypedHttpCallBuilder.CreateMock())
        { }

        private MockHalCallBuilder(MockTypedHttpCallBuilder builder)
            : base(builder)
        {
            _innerBuilder = builder;
        }

        public static MockHalCallBuilder CreateMock()
        {
            return new MockHalCallBuilder();
        }

        public MockHalCallBuilder WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory)
        {
            _innerBuilder.WithResult(resultFactory);

            return this;
        }

        public MockHalCallBuilder WithResult<TResult>(TResult result) 
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public MockHalCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _innerBuilder.WithResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public MockHalCallBuilder WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory)
        {
            _innerBuilder.WithError(errorFactory);

            return this;
        }

        public MockHalCallBuilder WithError<TError>(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public MockHalCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _innerBuilder.WithError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public MockHalCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public MockHalCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public MockHalCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}
