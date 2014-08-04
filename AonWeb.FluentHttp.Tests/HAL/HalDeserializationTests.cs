using System.Net;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Mocks.WebServer;
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
        public async Task CanDeserializeResource()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceJson }.AddPrivateCacheHeader());

                var result = await HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestResource>();

                Assert.NotNull(result);
                Assert.AreEqual("Response1", result.Result);
                Assert.AreEqual("http://localhost:8889/canonical/1", result.Links.Self().ToString());
            }
        }

        [Test]
        public async Task CanDeserializeResourceWithLinks()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceWithLinksJson }.AddPrivateCacheHeader());

                var result = await HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestResourceWithLinks>();

                Assert.NotNull(result);
                Assert.AreEqual("Response1", result.Result);
                Assert.AreEqual("http://localhost:8889/link1/1", result.Links.Link1().ToString());
            }
        }
        [Test]
        public async Task CanDeserializeListWithEmbedded()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddPrivateCacheHeader());

                var result = await HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestListResource>();

                Assert.NotNull(result);
                Assert.AreEqual("http://localhost:8889/list/1", result.Links.Self().ToString());
                Assert.AreEqual(3, result.Results.Count);
                Assert.AreEqual("http://localhost:8889/canonical/1", result.Results[0].Links.Self().ToString());
            }
        }

        [Test]
        public async Task CanDeserializeListWithEmbeddedWithLinks()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddPrivateCacheHeader());

                var result = await HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestListResourceWithLinks>();

                Assert.NotNull(result);
                Assert.AreEqual("http://localhost:8889/list/1", result.Links.Self().ToString());
                Assert.AreEqual(3, result.Results.Count);
                Assert.AreEqual("http://localhost:8889/canonical/1", result.Results[0].Links.Self().ToString());
            }
        }

        [Test]
        public async Task CanDeserializeListWithEmbeddedWithLinksWithEmbeddedJsonProperty()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListEmbeddedPropertyJson }.AddPrivateCacheHeader());

                var result = await HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestListEmbeddedPropertyParentsResource>();

                Assert.NotNull(result, "Result was null");
                Assert.NotNull(result.Results, "Result.Results was null");
                Assert.AreEqual(result.Count, result.Results.Count, "Unexpected value for Result.Count");
                Assert.NotNull(result.Results[0].Children, "Result.Results[0].Children was null");
                Assert.AreEqual(result.Results[0].Children.Count, result.Results[0].Children.Results.Count, "Unexpected value for Result.Results[0].Children.Count");
                Assert.NotNull(result.Results[0].Children.Results[0], "Result.Results[0].Children.Results[0] was null");
                Assert.IsNotNullOrEmpty(result.Results[0].Children.Results[0].Result, "Unexpected value for Result.Results[0].Children.Results[0].Result");
            }
        }

        [Test]
        public async Task CanDeserializeWithEmbedAsArray()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListEmbeddedArrayJson }.AddPrivateCacheHeader());

                var result = await HalCallBuilder.Create()
                    .WithLink(uri).ResultAsync<TestListEmbeddedArrayParentResource>();

                Assert.NotNull(result, "Result was null");
                Assert.NotNull(result.Results, "Result.Results was null");
                Assert.AreEqual(2, result.Results.Count, "Unexpected value for Result.Count");
                Assert.IsNotNullOrEmpty(result.Results[0].Result, "Unexpected value for Result.Results[0].Result");
            }
        }

        [Test]
        public void CanDeserializeCamelCaseError()
        {
            var uri = LocalWebServer.DefaultListenerUri;
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceJson, StatusCode = HttpStatusCode.NotFound}.AddPrivateCacheHeader());

                var ex = Assert.Catch<HttpErrorException<TestError>>(async ()=> await HalCallBuilder.Create().WithLink(uri).WithErrorType<TestError>().SendAsync());

                Assert.NotNull(ex);
                Assert.AreEqual("Response1", ex.Error.Result);
            }
        }
    }
}
