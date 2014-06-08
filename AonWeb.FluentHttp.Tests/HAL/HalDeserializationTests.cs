using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Tests.Helpers;
using AonWeb.FluentHttp.Tests.Helpers.HAL;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.HAL
{
    [TestFixture]
    public class HalDeserializationTests
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = true;
        }

        [SetUp]
        public void Setup()
        {
            HttpCallBuilderDefaults.ClearCache();
        }

        [Test]
        public void CanDeserializeResource()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceJson }.AddPrivateCacheHeader());

                var result = HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestResource>().Result;

                Assert.NotNull(result);
                Assert.AreEqual("Response1", result.Result);
                Assert.AreEqual("http://localhost:8889/canonical/1", result.Links.Self().ToString());
            }
        }

        [Test]
        public void CanDeserializeResourceWithLinks()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceWithLinksJson }.AddPrivateCacheHeader());

                var result = HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestResourceWithLinks>().Result;

                Assert.NotNull(result);
                Assert.AreEqual("Response1", result.Result);
                Assert.AreEqual("http://localhost:8889/link1/1", result.Links.Link1().ToString());
            }
        }
        [Test]
        public void CanDeserializeListWithEmbedded()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddPrivateCacheHeader());

                var result = HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestListResource>().Result;

                Assert.NotNull(result);
                Assert.AreEqual("http://localhost:8889/list/1", result.Links.Self().ToString());
                Assert.AreEqual(3, result.Results.Count);
                Assert.AreEqual("http://localhost:8889/canonical/1", result.Results[0].Links.Self().ToString());
            }
        }
    }
}
