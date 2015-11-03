using System;
using System.Net.Http;

using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClientBuilder : HttpClientBuilder, IMockHttpClientBuilder
    {
        private readonly MockHttpClient _client;

        public MockHttpClientBuilder(IHttpClientSettings settings) 
            : base(settings)
        {
            _client = new MockHttpClient();
        }

        protected override IHttpClient GetClientInstance(HttpMessageHandler handler, IHttpClientSettings settings)
        {
            return _client;
        }

        public IMockHttpClientBuilder WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory)
        {
            _client.WithResponse(predicate, responseFactory);

            return this;
        }
    }
}