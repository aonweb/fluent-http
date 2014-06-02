using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpCallBuilder<TResult, TContent, TError> : HttpCallBuilder<TResult, TContent, TError>, IMockHttpCallBuilder<TResult, TContent, TError>
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

        public IMockHttpCallBuilder<TResult, TContent, TError> ConfigureResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory)
        {
            _formatter.ConfigureResult(resultFactory);

            return this;
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> WithResult(TResult result)
        {
            return WithResult(result, HttpStatusCode.OK);
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode)
        {
            _formatter.ConfigureResult((r, c) => result);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> ConfigureError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory)
        {
            _formatter.ConfigureError(errorFactory);

            return this;
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> WithError(TError error)
        {
            return WithError(error, HttpStatusCode.InternalServerError);
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode)
        {
            _formatter.ConfigureError((r, c) => error);

            return WithResponse(new ResponseInfo(statusCode));
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _innerBuilder.ConfigureResponse(responseFactory);

            return this;
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> WithResponse(HttpResponseMessage response)
        {
            return ConfigureResponse(r => response);
        }

        public IMockHttpCallBuilder<TResult, TContent, TError> WithResponse(ResponseInfo response)
        {
            return ConfigureResponse(r => response.ToHttpResponseMessage());
        }
    }

    public class MockHttpCallBuilder : HttpCallBuilder, IMockHttpCallBuilder
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

        public IMockHttpCallBuilder ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureClient(b => ((MockHttpClientBuilder)b).ConfigureResponse(responseFactory));

            return this;
        }

        public IMockHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return ConfigureResponse(r => response);
        }

        public IMockHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return ConfigureResponse(r => response.ToHttpResponseMessage());
        }
    }
}