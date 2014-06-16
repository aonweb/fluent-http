using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedHttpCallBuilder : 
        TypedHttpCallBuilder,
        IMockBuilder<MockTypedHttpCallBuilder>
    {
        private readonly MockFormatter _formatter;
        private readonly MockHttpCallBuilder _innerBuilder;

        protected MockTypedHttpCallBuilder()
            : this(new MockHttpCallBuilder(),new MockFormatter())
        { }

        private MockTypedHttpCallBuilder(MockHttpCallBuilder builder, MockFormatter formatter)
            : base(builder, formatter)
        {
            _innerBuilder = builder;
            _formatter = formatter;
        }

        public static MockTypedHttpCallBuilder CreateMock()
        {
            return new MockTypedHttpCallBuilder();
        }

        public static MockTypedHttpCallBuilder CreateMock(string baseUri)
        {
            return (MockTypedHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public static MockTypedHttpCallBuilder CreateMock(Uri baseUri)
        {
            return (MockTypedHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public MockTypedHttpCallBuilder WithResult<TResult>(Func<HttpResponseMessage, HttpCallContext, TResult> resultFactory)
        {
            _formatter.WithResult(resultFactory);

            return this;
        }

        public MockTypedHttpCallBuilder WithResult<TResult>(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public MockTypedHttpCallBuilder WithResult<TResult>(TResult result, HttpStatusCode statusCode)
        {
            _formatter.WithResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public MockTypedHttpCallBuilder WithError<TError>(Func<HttpResponseMessage, HttpCallContext, TError> errorFactory)
        {
            _formatter.WithError(errorFactory);

            return this;
        }

        public MockTypedHttpCallBuilder WithError<TError>(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public MockTypedHttpCallBuilder WithError<TError>(TError error, HttpStatusCode statusCode)
        {
            _formatter.WithError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public MockTypedHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.WithResponse(responseFactory);

            return this;
        }

        public MockTypedHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public MockTypedHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}