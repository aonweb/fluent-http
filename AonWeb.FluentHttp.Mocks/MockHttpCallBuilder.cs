using System;
using System.Net.Http;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpCallBuilder : HttpCallBuilder, IMockBuilder<MockHttpCallBuilder>
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