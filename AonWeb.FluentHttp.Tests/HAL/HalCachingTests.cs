using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Mocks.WebServer;
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
        [Ignore]
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

                var result1 = HalCallBuilder.Create()
                    .WithLink(nonCanonicalUri).ResultAsync<TestResource>().Result;

                HalCallBuilder.Create()
                    .WithLink(canonicalUri).AsPut().SendAsync().Wait();

                var result2 = HalCallBuilder.Create()
                    .WithLink(nonCanonicalUri).ResultAsync<TestResource>().Result;

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.AreNotEqual(result1.Result, result2.Result);
            }
        }

        //get to list with embeds, put to canonical, expires list
        [Test]
        [Ignore]
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

                var result1 = HalCallBuilder.Create().WithLink(listUri).ResultAsync<TestListResource>().Result;

                HalCallBuilder.Create().WithLink(canonicalUri).AsPut().SendAsync().Wait();

                var result2 = HalCallBuilder.Create().WithLink(listUri).ResultAsync<TestListResource>().Result;


                Assert.AreNotEqual(result1.Results[0].Result, result2.Results[0].Result);
            }
        }

        [Test]
        public async Task WhenCacheHandler_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo {Body = HalMother.TestListJson}.AddCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnMiss<TestListResource>(ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnHit<TestListResource>(ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                Assert.IsTrue(miss, "Miss Handler was not called");
                Assert.IsTrue(hit, "Hit Handler was not called");
                Assert.AreSame(result1, result2);
            }
        }

        [Test]
        public async Task WhenCacheHandler_WithObjectHandler_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnMiss<object>(ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnHit<object>(ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                Assert.IsTrue(miss, "Miss Handler was not called");
                Assert.IsTrue(hit, "Hit Handler was not called");
                Assert.AreSame(result1, result2);
            }
        }

        [Test]
        public async Task WhenCacheHandler_WithPriority_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnMiss<TestListResource>(HttpCallHandlerPriority.First, ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnHit<TestListResource>(HttpCallHandlerPriority.First, ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                Assert.IsTrue(miss, "Miss Handler was not called");
                Assert.IsTrue(hit, "Hit Handler was not called");
                Assert.AreSame(result1, result2);
            }
        }

        [Test]
        public async Task WhenCacheHandler_WithObjectHandlerAndPriority_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnMiss<TestListResource>(HttpCallHandlerPriority.First, ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c.OnHit<TestListResource>(ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                Assert.IsTrue(miss, "Miss Handler was not called");
                Assert.IsTrue(hit, "Hit Handler was not called");
                Assert.AreSame(result1, result2);
            }
        }

        [Test]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectHandlerCalled()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss1 = false;
                var hit1 = false;
                var miss2 = false;
                var hit2 = false;

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c
                        .OnMiss<object>(HttpCallHandlerPriority.First, ctx =>
                        {
                            miss1 = true;
                        })
                        .OnMiss<TestListResource>(HttpCallHandlerPriority.Last, ctx =>
                        {
                            miss2 = true;
                        }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c
                        .OnHit<object>(HttpCallHandlerPriority.First, ctx =>
                        {
                            hit1 = true;
                        })
                        .OnHit<TestListResource>(HttpCallHandlerPriority.Last, ctx =>
                        {
                            hit2 = true;
                        }))
                    .ResultAsync<TestListResource>();


                Assert.IsTrue(miss1, "Miss Handler 1 was not called");
                Assert.IsTrue(miss2, "Miss Handler 2 was not called");
                Assert.IsTrue(hit1, "Hit Handler 1 was not called");
                Assert.IsTrue(hit2, "Hit Handler 2 was not called");
                Assert.AreSame(result1, result2);
            }
            
        }

        [Test]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectContextItemsMaintained()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddCacheHeader(expires: DateTime.Now.AddHours(1)));

                var expected = "Initial";
                var key = "MyCacheItemsKey";
                string miss1 = null;
                string miss2 = null;
                string hit1 = null;
                string hit2 = null;

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c
                        .OnMiss<object>(HttpCallHandlerPriority.First, ctx =>
                        {
                            miss1 = ctx.Items[key] as string;
                            ctx.Items[key] = expected;
                        })
                        .OnMiss<TestListResource>(HttpCallHandlerPriority.Last, ctx =>
                        {
                            miss2 = ctx.Items[key] as string;
                        }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c
                        .OnHit<object>(HttpCallHandlerPriority.First, ctx =>
                        {
                            hit1 = ctx.Items[key] as string;
                            ctx.Items[key] = expected;
                        })
                        .OnHit<TestListResource>(HttpCallHandlerPriority.Last, ctx =>
                        {
                            hit2 = ctx.Items[key] as string;
                        }))
                    .ResultAsync<TestListResource>();


                Assert.IsNull(miss1, "Miss Handler 1 has incorrect value");
                Assert.AreEqual(expected, miss2, "Miss Handler 2 has incorrect value");
                Assert.IsNull(hit1, "Hit Handler 1 has incorrect value");
                Assert.AreEqual(expected, hit2, "Hit Handler 2 has incorrect value");
                Assert.AreSame(result1, result2);
            }

        }

        [Test]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectHandlerCalledInOrder()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                var listUri = LocalWebServer.DefaultListenerUri + "/list/1";

                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = HalMother.TestListJson }.AddCacheHeader(expires: DateTime.Now.AddHours(1)));
                var current = 0;
                var miss1 = 0;
                var miss2 = 0;
                var miss3 = 0;
                var hit1 = 0;
                var hit2 = 0;
                

                var result1 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c
                        .OnMiss<object>(HttpCallHandlerPriority.Last, ctx =>
                        {
                            current++;
                            miss3 = current;
                        })
                        .OnMiss<TestListResource>(HttpCallHandlerPriority.First, ctx =>
                        {
                            current++;
                            miss1 = current;
                        })
                        .OnMiss<TestListResource>(HttpCallHandlerPriority.Default ,ctx =>
                        {
                            current++;
                            miss2 = current;
                        }))
                    .ResultAsync<TestListResource>();

                var result2 = await HalCallBuilder.Create().WithLink(listUri)
                    .Advanced.ConfigureHandler<TypedHttpCallCacheHandler>(c => c
                        .OnHit<object>(HttpCallHandlerPriority.Low, ctx =>
                        {
                            current++;
                            hit2 = current;
                        })
                        .OnHit<TestListResource>(HttpCallHandlerPriority.High, ctx =>
                        {
                            current++;
                            hit1 = current;
                        }))
                    .ResultAsync<TestListResource>();


                Assert.AreEqual(1,miss1, "Miss Handler 1 has incorrect value");
                Assert.AreEqual(2, miss2, "Miss Handler 2 has incorrect value");
                Assert.AreEqual(3, miss3, "Miss Handler 2 has incorrect value");
                Assert.AreEqual(4, hit1, "Hit Handler 1 has incorrect value");
                Assert.AreEqual(5, hit2, "Hit Handler 2 has incorrect value");
                Assert.AreSame(result1, result2);
            }

        }

        public class StubCacheHandler : CacheHandler
        {
            public bool Miss { get; set; }
            public bool Hit { get; set; }
            public bool Store { get; set; }
            public bool Expired { get; set; }


            public override Task OnMiss<TResult>(CacheMissContext<TResult> context)
            {
                Miss = true;

                return Task.Delay(0);
            }

            public override Task OnHit<TResult>(CacheHitContext<TResult> context)
            {

                Hit = true;
                return Task.Delay(0);
            }

            public override Task OnStore<TResult>(CacheStoreContext<TResult> context)
            {

                Store = true;
                return Task.Delay(0);
            }

            public override Task OnExpired(CacheExpiredContext context)
            {

                Expired = true;
                return Task.Delay(0);
            }
        }
    }
}
