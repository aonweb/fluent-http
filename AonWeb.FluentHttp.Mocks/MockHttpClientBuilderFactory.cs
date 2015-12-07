using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpClientBuilderFactory : HttpClientBuilderFactory
    {
        public override IHttpClientBuilder Create()
        {
            return new MockHttpClientBuilder(new HttpClientSettings());
        }
    }
}