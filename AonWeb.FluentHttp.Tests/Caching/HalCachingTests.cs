using System;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Caching
{
    [Collection("LocalWebServer Tests")]
    public class HalCachingTests
    {
        private readonly ITestOutputHelper _logger;

        public HalCachingTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Current.GetCachingDefaults().Enabled = true;
            Cache.Clear();
        }

        [Fact(Skip="Not yet implemented")]
        public async Task WhenGetResourceByNonCanonicalUri_ThenModifyResourceByCanonicalUri_ExpectBothExpiredInCache()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var nonCanonicalUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/noncanonical/1");
                var canonicalUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/canonical/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = new HalBuilderFactory().Create()
                    .WithLink(nonCanonicalUri).ResultAsync<TestResource>();

                await new HalBuilderFactory().Create()
                    .WithLink(canonicalUri).AsPut().SendAsync();

                var result2 = new HalBuilderFactory().Create()
                    .WithLink(nonCanonicalUri).ResultAsync<TestResource>();

                result1.ShouldNotBeNull();
                result2.ShouldNotBeNull();
                result1.Result.ShouldNotBe(result2.Result);
            }
        }

        //get to list with embeds, put to canonical, expires list
        [Fact(Skip = "Not yet implemented")]
        public async Task WhenGetListWithEmbeddedResources_ThenModifyOneEmbeddedResource_ExpectListExpired()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");
                var canonicalUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/canonical/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri).ResultAsync<TestListResource>();

                await new HalBuilderFactory().Create().WithLink(canonicalUri).AsPut().SendAsync();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri).ResultAsync<TestListResource>();


                result1.Results[0].ShouldNotBe(result2.Results[0]);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnMiss<TestListResource>(ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnHit<TestListResource>(ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();

                miss.ShouldBeTrue("Miss Handler was not called");
                hit.ShouldBeTrue("Hit Handler was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithObjectHandler_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnMiss<object>(ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnHit<object>(ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                miss.ShouldBeTrue("Miss Handler was not called");
                hit.ShouldBeTrue("Hit Handler was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithPriority_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnMiss<TestListResource>(HandlerPriority.First, ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnHit<TestListResource>(HandlerPriority.First, ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                miss.ShouldBeTrue("Miss Handler was not called");
                hit.ShouldBeTrue("Hit Handler was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithObjectHandlerAndPriority_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnMiss<TestListResource>(HandlerPriority.First, ctx =>
                    {
                        miss = true;
                    }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c.OnHit<TestListResource>(ctx =>
                    {
                        hit = true;
                    }))
                    .ResultAsync<TestListResource>();


                miss.ShouldBeTrue("Miss Handler was not called");
                hit.ShouldBeTrue("Hit Handler was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectHandlerCalled()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss1 = false;
                var hit1 = false;
                var miss2 = false;
                var hit2 = false;

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c
                        .OnMiss<object>(HandlerPriority.First, ctx =>
                        {
                            miss1 = true;
                        })
                        .OnMiss<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            miss2 = true;
                        }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c
                        .OnHit<object>(HandlerPriority.First, ctx =>
                        {
                            hit1 = true;
                        })
                        .OnHit<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            hit2 = true;
                        }))
                    .ResultAsync<TestListResource>();


                miss1.ShouldBeTrue("Miss Handler 1 was not called");
                miss2.ShouldBeTrue("Miss Handler 2 was not called");
                hit1.ShouldBeTrue("Hit Handler 1 was not called");
                hit2.ShouldBeTrue("Hit Handler 2 was not called");
                result1.ShouldBeSameAs(result2);
            }

        }

        [Fact]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectContextItemsMaintained()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var expected = "Initial";
                var key = "MyCacheItemsKey";
                string miss1 = null;
                string miss2 = null;
                string hit1 = null;
                string hit2 = null;

                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c
                        .OnMiss<object>(HandlerPriority.First, ctx =>
                        {
                            miss1 = ctx.Items[key] as string;
                            ctx.Items[key] = expected;
                        })
                        .OnMiss<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            miss2 = ctx.Items[key] as string;
                        }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c
                        .OnHit<object>(HandlerPriority.First, ctx =>
                        {
                            hit1 = ctx.Items[key] as string;
                            ctx.Items[key] = expected;
                        })
                        .OnHit<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            hit2 = ctx.Items[key] as string;
                        }))
                    .ResultAsync<TestListResource>();


                miss1.ShouldBeNull("Miss Handler 1 has incorrect value");
                expected.ShouldBe(miss2);
                hit1.ShouldBeNull("Hit Handler 1 has incorrect value");
                expected.ShouldBe(hit2);
                result1.ShouldBeSameAs(result2);
            }

        }

        [Fact]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectHandlerCalledInOrder()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = UriHelpers.CombineVirtualPaths(server.ListeningUri, "/list/1");

                server
                    .WithNextResponse(
                        new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(
                            expires: DateTime.Now.AddHours(1)));
                var current = 0;
                var miss1 = 0;
                var miss2 = 0;
                var miss3 = 0;
                var hit1 = 0;
                var hit2 = 0;


                var result1 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c
                        .OnMiss<object>(HandlerPriority.Last, ctx =>
                        {
                            current++;
                            miss3 = current;
                        })
                        .OnMiss<TestListResource>(HandlerPriority.First, ctx =>
                        {
                            current++;
                            miss1 = current;
                        })
                        .OnMiss<TestListResource>(HandlerPriority.Default, ctx =>
                        {
                            current++;
                            miss2 = current;
                        }))
                    .ResultAsync<TestListResource>();

                var result2 = await new HalBuilderFactory().Create().WithLink(listUri)
                    .Advanced.WithHandlerConfiguration<TypedCacheConfigurationHandler>(c => c
                        .OnHit<object>(HandlerPriority.Low, ctx =>
                        {
                            current++;
                            hit2 = current;
                        })
                        .OnHit<TestListResource>(HandlerPriority.High, ctx =>
                        {
                            current++;
                            hit1 = current;
                        }))
                    .ResultAsync<TestListResource>();


                miss1.ShouldBe(1);
                miss2.ShouldBe(2);
                miss3.ShouldBe(3);
                hit1.ShouldBe(4);
                hit2.ShouldBe(5);
                result1.ShouldBeSameAs(result2);
            }
        }
    }
}