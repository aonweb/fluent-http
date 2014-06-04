using System;
using System.Net.Http;

using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClientBuilder : HttpClientBuilder, IHttpMocker<MockHttpClientBuilder>
    {
        private Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        protected override IHttpClient GetClientInstance(HttpMessageHandler handler)
        {
            var client = new MockHttpClient();

            if (_responseFactory != null)
                client.WithResponse(_responseFactory);

            return client;
        }

        public MockHttpClientBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;

            return this;
        }

        public MockHttpClientBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public MockHttpClientBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}