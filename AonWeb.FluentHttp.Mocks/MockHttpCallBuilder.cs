using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpCallBuilder<TResult, TContent, TError> : 
        HttpCallBuilder<TResult, TContent, TError>,
        IMockBuilder<TResult, TContent, TError>
    {
        private readonly MockFormatter<TResult, TContent, TError> _formatter;
        private readonly MockHttpCallBuilder _innerBuilder;

        protected MockHttpCallBuilder()
            : this(new MockHttpCallBuilder(),new MockFormatter<TResult, TContent, TError>())
        { }

        private MockHttpCallBuilder(MockHttpCallBuilder builder, MockFormatter<TResult, TContent, TError> formatter)
            : base(builder, formatter)
        {
            _innerBuilder = builder;
            _formatter = formatter;
        }

        public static MockHttpCallBuilder<TResult, TContent, TError> CreateMock()
        {
            return new MockHttpCallBuilder<TResult, TContent, TError>();
        }

        public static MockHttpCallBuilder<TResult, TContent, TError> CreateMock(string baseUri)
        {
            return (MockHttpCallBuilder<TResult, TContent, TError>)(CreateMock().WithBaseUri(baseUri));
        }

        public static MockHttpCallBuilder<TResult, TContent, TError> CreateMock(Uri baseUri)
        {
            return (MockHttpCallBuilder<TResult, TContent, TError>)(CreateMock().WithBaseUri(baseUri));
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _formatter.WithResult(resultFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public IMockBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode)
        {
            _formatter.WithResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockBuilder<TResult, TContent, TError> WithError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _formatter.WithError(errorFactory);

            return this;
        }

        public IMockBuilder<TResult, TContent, TError> WithError(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public IMockBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode)
        {
            _formatter.WithError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
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

    public class MockHttpCallBuilder : HttpCallBuilder, IMockBuilder
    {
        public MockHttpCallBuilder()
            : base(new MockHttpClientBuilder()) { }

        public static MockHttpCallBuilder CreateMock()
        {
            return new MockHttpCallBuilder();
        }

        public static MockHttpCallBuilder CreateMock(string baseUri)
        {
            return (MockHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public static MockHttpCallBuilder CreateMock(Uri baseUri)
        {
            return (MockHttpCallBuilder)(CreateMock().WithBaseUri(baseUri));
        }

        public IMockBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(responseFactory));

            return this;
        }

        public IMockBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public IMockBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}