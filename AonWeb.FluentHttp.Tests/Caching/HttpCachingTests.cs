using System;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Caching
{
    public class HttpCachingTests
    {
        private readonly ITestOutputHelper _logger;

        public HttpCachingTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Cache.Clear();
        }


        [Fact]
        public async Task WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuilders()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                var result2 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuildersOnDifferentThreads()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var response1 = await Task.Factory.StartNew(() =>
                    new HttpBuilderFactory().Create()
                   .WithUri(server.ListeningUri)
                   .ResultAsync());

                var result1 = await response1.ReadContentsAsync();

                var response2 = await Task.Factory.StartNew(() =>
                    new HttpBuilderFactory().Create()
                   .WithUri(server.ListeningUri)
                   .ResultAsync());

                var result2 = await response2.ReadContentsAsync();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOff_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await new HttpBuilderFactory().Create()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithNoCache()
                        .ResultAsync().ReadContentsAsync();

                var result2 = await new HttpBuilderFactory().Create()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithNoCache()
                        .ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsAreNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithNoCacheHeader());

                var result1 = await new HttpBuilderFactory().Create()
                       .WithUri(server.ListeningUri)
                       .Advanced.WithNoCache()
                       .ResultAsync().ReadContentsAsync();

                var result2 = await new HttpBuilderFactory().Create()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithNoCache()
                        .ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithPost_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync().ReadContentsAsync();

                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).AsPost()
                       .ResultAsync();

                var result2 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }


        [Fact]
        public async Task WithDependendUrls_ExpectPostInvalidatesDependents()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var parentUri = server.ListeningUri;
                var childUri = UriHelpers.CombineVirtualPaths(parentUri, "child");
                server
                    .WithResponse(
                        ctx => ctx.RequestUri == parentUri && ctx.Method == HttpMethod.Get,
                        ctx =>
                        {
                            return new MockHttpResponseMessage().WithContent("Parent Response" + ctx.RequestCountForThisUrl).WithPrivateCacheHeader();
                        })
                    .WithResponse(
                        ctx => ctx.RequestUri == childUri && ctx.Method == HttpMethod.Get,
                        ctx =>
                        {
                            return new MockHttpResponseMessage().WithContent("Child Response" + ctx.RequestCountForThisUrl).WithPrivateCacheHeader();
                        })
                    .WithResponse(ctx => ctx.Method == HttpMethod.Post,
                        new MockHttpResponseMessage().WithNoCacheHeader());

                var parent1 = await new HttpBuilderFactory().Create().WithUri(parentUri).Advanced.WithDependentUri(childUri).ResultAsync().ReadContentsAsync();
                var child1 = await new HttpBuilderFactory().Create().WithUri(childUri).ResultAsync().ReadContentsAsync();
                var parent2 = await new HttpBuilderFactory().Create().WithUri(parentUri).ResultAsync().ReadContentsAsync();
                var child2 = await new HttpBuilderFactory().Create().WithUri(childUri).Advanced.ResultAsync().ReadContentsAsync();

                parent1.ShouldBe(parent2);
                child1.ShouldBe(child2);

                await new HttpBuilderFactory().Create().WithUri(parentUri).AsPost().ResultAsync();

                var parent3 = await new HttpBuilderFactory().Create().WithUri(parentUri).ResultAsync().ReadContentsAsync();
                parent3.ShouldBe("Parent Response3"); // because of the post

                var child3 = await new HttpBuilderFactory().Create().WithUri(childUri).ResultAsync().ReadContentsAsync();
                child3.ShouldBe("Child Response2");
            }
        }

        [Fact]
        public async Task WithDependendUrlsThatAreSelfReferential_ExpectPostNoException()
        {


            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var parentUri = server.ListeningUri;
                var childUri = UriHelpers.CombineVirtualPaths(parentUri, "child");
                var grandchildUri = UriHelpers.CombineVirtualPaths(parentUri, "grandchild");
                server.WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response2").WithPrivateCacheHeader());

                var parent1 = await new HttpBuilderFactory().Create().WithUri(parentUri).Advanced.WithDependentUris(new[] { parentUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var child1 = await new HttpBuilderFactory().Create().WithUri(childUri).Advanced.WithDependentUris(new[] { childUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var grandchild1 = await new HttpBuilderFactory().Create().WithUri(grandchildUri).Advanced.WithDependentUris(new[] { parentUri, childUri }).ResultAsync().ReadContentsAsync();

                await new HttpBuilderFactory().Create().WithUri(parentUri).AsPost().ResultAsync();

                var parent2 = await new HttpBuilderFactory().Create().WithUri(parentUri).Advanced.WithDependentUris(new[] { parentUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var child2 = await new HttpBuilderFactory().Create().WithUri(childUri).Advanced.WithDependentUris(new[] { childUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var grandchild2 = await new HttpBuilderFactory().Create().WithUri(grandchildUri).Advanced.WithDependentUris(new[] { parentUri, childUri }).ResultAsync().ReadContentsAsync();

                parent1.ShouldNotBe(parent2);
                child1.ShouldNotBe(child2);
                grandchild1.ShouldNotBe(grandchild2);
            }

        }

        [Fact]
        public async Task WithDependendUrls2LevelsDeep_ExpectPostInvalidatesDependents()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var parentUri = server.ListeningUri;
                var childUri = new Uri(UriHelpers.CombineVirtualPaths(parentUri.ToString(), "child"));
                var grandchildUri = new Uri(UriHelpers.CombineVirtualPaths(parentUri.ToString(), "grandchild"));
                server.WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response2").WithPrivateCacheHeader());

                var parent1 = await new HttpBuilderFactory().Create().WithUri(parentUri).Advanced.WithDependentUri(childUri).ResultAsync().ReadContentsAsync();
                var child1 = await new HttpBuilderFactory().Create().WithUri(childUri).Advanced.WithDependentUri(grandchildUri).ResultAsync().ReadContentsAsync();
                var grandchild1 = await new HttpBuilderFactory().Create().WithUri(grandchildUri).ResultAsync().ReadContentsAsync();

                await new HttpBuilderFactory().Create().WithUri(parentUri).AsPost().ResultAsync();

                var parent2 = await new HttpBuilderFactory().Create().WithUri(parentUri).Advanced.WithDependentUri(childUri).ResultAsync().ReadContentsAsync();
                var child2 = await new HttpBuilderFactory().Create().WithUri(childUri).ResultAsync().ReadContentsAsync();
                var grandchild2 = await new HttpBuilderFactory().Create().WithUri(childUri).ResultAsync().ReadContentsAsync();

                parent1.ShouldNotBe(parent2);
                child1.ShouldNotBe(child2);
                grandchild1.ShouldNotBe(grandchild2);
            }
        }


        [Fact]
        public async Task WhenHttpCachingIsOn_WithPut_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync();

                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).AsPut()
                       .ResultAsync();

                var result2 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithPatch_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync();

                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).AsPatch()
                       .ResultAsync();

                var result2 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithDelete_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).AsDelete().ResultAsync();

                var result2 = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }
    }
}