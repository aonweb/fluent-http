using System;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpCallBuilder : HttpCallBuilder, IMockBuilder<MockHttpCallBuilder>
    {
        public MockHttpCallBuilder()
            : base(new MockHttpClientBuilder()) { }

        public static MockHttpCallBuilder CreateMock()
        {
            return new MockHttpCallBuilder().ConfigureMock();
        }

        public static MockHttpCallBuilder CreateMock(string baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);

            return (MockHttpCallBuilder)builder;
        }

        public static MockHttpCallBuilder CreateMock(Uri baseUri)
        {
            var builder = CreateMock().WithBaseUri(baseUri);
                
            return (MockHttpCallBuilder)builder;
        }

        public MockHttpCallBuilder WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            ConfigureClient(b => ((MockHttpClientBuilder)b).WithResponse(responseFactory));

            return this;
        }

        public MockHttpCallBuilder WithResponse(HttpResponseMessage response)
        {
            return WithResponse(r => response);
        }

        public MockHttpCallBuilder WithResponse(ResponseInfo response)
        {
            return WithResponse(r => response.ToHttpResponseMessage());
        }
    }
}