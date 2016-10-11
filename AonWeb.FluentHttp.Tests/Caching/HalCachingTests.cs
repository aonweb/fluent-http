using System;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
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
            Cache.DeleteAll();
        }

        private static IHalBuilder CreateBuilder()
        {
            return new HalBuilderFactory().Create().Advanced.WithCaching(true);
        }

        [Fact(Skip="Not yet implemented")]
        public async Task WhenGetResourceByNonCanonicalUri_ThenModifyResourceByCanonicalUri_ExpectBothExpiredInCache()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var nonCanonicalUri = server.ListeningUri.AppendPath("/noncanonical/1");
                var canonicalUri = server.ListeningUri.AppendPath("/canonical/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = CreateBuilder()
                    .WithLink(nonCanonicalUri).ResultAsync<TestResource>();

                await CreateBuilder()
                    .WithLink(canonicalUri).AsPut().SendAsync();

                var result2 = CreateBuilder()
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
                var listUri = server.ListeningUri.AppendPath("/list/1");
                var canonicalUri = server.ListeningUri.AppendPath("/canonical/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder().WithLink(listUri).ResultAsync<TestListResource>();

                await CreateBuilder().WithLink(canonicalUri).AsPut().SendAsync();

                var result2 = await CreateBuilder().WithLink(listUri).ResultAsync<TestListResource>();


                result1.Results[0].ShouldNotBe(result2.Results[0]);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheMiss(ctx =>
                    {
                        miss = true;
                    })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheHit(ctx =>
                    {
                        hit = true;
                    })
                    .ResultAsync<TestListResource>();

                miss.ShouldBeTrue("Miss HandlerRegister was not called");
                hit.ShouldBeTrue("Hit HandlerRegister was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithObjectHandler_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheMiss(ctx =>
                    {
                        miss = true;
                    })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheHit(ctx =>
                    {
                        hit = true;
                    })
                    .ResultAsync<TestListResource>();


                miss.ShouldBeTrue("Miss HandlerRegister was not called");
                hit.ShouldBeTrue("Hit HandlerRegister was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithPriority_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheMiss(HandlerPriority.First, ctx =>
                    {
                        miss = true;
                    })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheHit(HandlerPriority.First, ctx =>
                    {
                        hit = true;
                    })
                    .ResultAsync<TestListResource>();


                miss.ShouldBeTrue("Miss HandlerRegister was not called");
                hit.ShouldBeTrue("Hit HandlerRegister was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithObjectHandlerAndPriority_ExpectHandlerCalled()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss = false;
                var hit = false;

                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheMiss(HandlerPriority.First, ctx =>
                    {
                        miss = true;
                    })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheHit(ctx =>
                    {
                        hit = true;
                    })
                    .ResultAsync<TestListResource>();


                miss.ShouldBeTrue("Miss HandlerRegister was not called");
                hit.ShouldBeTrue("Hit HandlerRegister was not called");
                result1.ShouldBeSameAs(result2);
            }
        }

        [Fact]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectHandlerCalled()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var miss1 = false;
                var hit1 = false;
                var miss2 = false;
                var hit2 = false;

                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced.OnCacheMiss(HandlerPriority.First, ctx =>
                        {
                            miss1 = true;
                        })
                        .OnCacheMiss<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            miss2 = true;
                        })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced
                        .OnCacheHit(HandlerPriority.First, ctx =>
                        {
                            hit1 = true;
                        })
                        .OnCacheHit<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            hit2 = true;
                        })
                    .ResultAsync<TestListResource>();


                miss1.ShouldBeTrue("Miss HandlerRegister 1 was not called");
                miss2.ShouldBeTrue("Miss HandlerRegister 2 was not called");
                hit1.ShouldBeTrue("Hit HandlerRegister 1 was not called");
                hit2.ShouldBeTrue("Hit HandlerRegister 2 was not called");
                result1.ShouldBeSameAs(result2);
            }

        }

        [Fact]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectContextItemsMaintained()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithCacheHeader(expires: DateTime.Now.AddHours(1)));

                var expected = "Initial";
                var key = "MyCacheItemsKey";
                string miss1 = null;
                string miss2 = null;
                string hit1 = null;
                string hit2 = null;

                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced
                        .OnCacheMiss(HandlerPriority.First, ctx =>
                        {
                            miss1 = ctx.Items[key] as string;
                            ctx.Items[key] = expected;
                        })
                        .OnCacheMiss<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            miss2 = ctx.Items[key] as string;
                        })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced
                        .OnCacheHit(HandlerPriority.First, ctx =>
                        {
                            hit1 = ctx.Items[key] as string;
                            ctx.Items[key] = expected;
                        })
                        .OnCacheHit<TestListResource>(HandlerPriority.Last, ctx =>
                        {
                            hit2 = ctx.Items[key] as string;
                        })
                    .ResultAsync<TestListResource>();


                miss1.ShouldBeNull("Miss HandlerRegister 1 has incorrect value");
                expected.ShouldBe(miss2);
                hit1.ShouldBeNull("Hit HandlerRegister 1 has incorrect value");
                expected.ShouldBe(hit2);
                result1.ShouldBeSameAs(result2);
            }

        }

        [Fact]
        public async Task WhenCacheHandler_WithMultipleHandlers_ExpectHandlerCalledInOrder()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var listUri = server.ListeningUri.AppendPath("/list/1");

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


                var result1 = await CreateBuilder().WithLink(listUri)
                    .Advanced
                        .OnCacheMiss(HandlerPriority.Last, ctx =>
                        {
                            current++;
                            miss3 = current;
                        })
                        .OnCacheMiss<TestListResource>(HandlerPriority.First, ctx =>
                        {
                            current++;
                            miss1 = current;
                        })
                        .OnCacheMiss<TestListResource>(HandlerPriority.Default, ctx =>
                        {
                            current++;
                            miss2 = current;
                        })
                    .ResultAsync<TestListResource>();

                var result2 = await CreateBuilder().WithLink(listUri)
                    .Advanced
                        .OnCacheHit(HandlerPriority.Low, ctx =>
                        {
                            current++;
                            hit2 = current;
                        })
                        .OnCacheHit<TestListResource>(HandlerPriority.High, ctx =>
                        {
                            current++;
                            hit1 = current;
                        })
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