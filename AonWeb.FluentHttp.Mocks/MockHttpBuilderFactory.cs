using System.Collections.Generic;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilderFactory : HttpBuilderFactory, IMockHttpBuilderFactory
    {
        public MockHttpBuilderFactory()
            :base(new MockHttpClientBuilderFactory(), null) { }

        protected override IChildHttpBuilder GetBuilder( IHttpBuilderSettings settings, IHttpClientBuilder clientBuilder)
        {
            return new MockHttpBuilder(settings, (IMockHttpClientBuilder)clientBuilder);
        }

        protected override IHttpBuilderSettings GetSettings( IList<IHttpHandler> handlers, ICacheSettings cacheSettings)
        {
            return new MockHttpBuilderSettings(cacheSettings, handlers, null);
        }

        public new IMockHttpBuilder Create()
        {
            return (IMockHttpBuilder)base.Create();
        }

        public new IMockHttpBuilder CreateAsChild()
        {
            return (IMockHttpBuilder)base.CreateAsChild();
        }
    }
}