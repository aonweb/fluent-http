using System;
using System.Net.Http;

using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClientBuilder : HttpClientBuilder
    {
        private Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        protected override IHttpClient GetClientInstance(HttpMessageHandler handler)
        {
            var client = new MockHttpClient();

            if (_responseFactory != null)
                client.ConfigureResponse(_responseFactory);

            return client;
        }

        public MockHttpClientBuilder ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;

            return this;
        }
    }
}