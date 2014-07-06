using System;
using System.Net.Http;

using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClientBuilder : HttpClientBuilder, IMockHttpClientBuilder
    {
        private Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        protected override IHttpClient GetClientInstance(HttpMessageHandler handler)
        {
            var client = new MockHttpClient();

            if (_responseFactory != null)
                client.WithResponse(_responseFactory);

            return client;
        }

        public IMockHttpClientBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;

            return this;
        }
    }
}