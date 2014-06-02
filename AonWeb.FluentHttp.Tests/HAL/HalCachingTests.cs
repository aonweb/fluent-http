using System.Collections.Generic;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Tests.Helpers;
using AonWeb.FluentHttp.Tests.Helpers.HAL;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.HAL
{
    [TestFixture]
    public class HalCachingTests
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

        //depended uris in request are expired

        //get by non-canonical, put to canonical, expires both
        [Test]
        public void WhenGetResourceByNonCanonicalUri_ThenModifyResourceByCanonicalUri_ExpectBothExpiredInCache()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var nonCanonicalUri = LocalWebServer.DefaultListenerUri + "/noncanonical/1";
                var canonicalUri = LocalWebServer.DefaultListenerUri + "/canonical/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceJson }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestResourceJson2 }.AddPrivateCacheHeader());

                var result1 = HalCallBuilder<TestResource, EmptyHalRequest, EmptyError>.Create()
                    .WithLink(nonCanonicalUri).ResultAsync().Result;

                HalCallBuilder<EmptyHalResult, EmptyHalRequest, EmptyError>.Create()
                    .WithLink(canonicalUri).AsPut().SendAsync().Wait();

                var result2 = HalCallBuilder<TestResource, EmptyHalRequest, EmptyError>.Create()
                    .WithLink(nonCanonicalUri).ResultAsync().Result;

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.AreNotEqual(result1.Result, result2.Result);
            }
        }

        //get to list with embeds, put to canonical, expires list
        [Test]
        public void WhenGetListWithEmbeddedResources_ThenModifyOneEmbeddedResource_ExpectListExpired()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";
                var canonicalUri = LocalWebServer.DefaultListenerUri + "/canonical/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson2 }.AddPrivateCacheHeader());

                var result1 = HalCallBuilder<TestListResource, EmptyHalRequest, EmptyError>.Create()
                    .WithLink(listUri).ResultAsync().Result;

                HalCallBuilder<EmptyHalResult, EmptyHalRequest, EmptyError>.Create()
                    .WithLink(canonicalUri).AsPut().SendAsync().Wait();

                var result2 = HalCallBuilder<TestListResource, EmptyHalRequest, EmptyError>.Create()
                    .WithLink(listUri).ResultAsync().Result;


                Assert.AreNotEqual(result1.Results[0].Result, result2.Results[0].Result);
            }
        }

        

    }
}
