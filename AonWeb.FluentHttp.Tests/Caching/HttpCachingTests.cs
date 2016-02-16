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
    [Collection("LocalWebServer Tests")]
    public class HttpCachingTests
    {
        private readonly ITestOutputHelper _logger;

        public HttpCachingTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Cache.DeleteAll();
        }

        private static IHttpBuilder CreateBuilder(TimeSpan? duration = null)
        {
            return new HttpBuilderFactory().Create().Advanced.WithCaching(true).WithDefaultDurationForCacheableResults(duration);
        }


        [Fact]
        public async Task WhenCachingIsOn_ExpectContentsCachedAccrossCallBuilders()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_ExpectUniqueUrisAreDistinct()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var baseUri = server.ListeningUri;
                var firstUri = baseUri.AppendPath("first");
                var secondUri = baseUri.AppendPath("second");

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var response1 = await CreateBuilder().WithUri(firstUri).ResultAsync();
                var result1 = await response1.ReadContentsAsync();

                var response2 = await CreateBuilder().WithUri(secondUri).ResultAsync();
                var result2 = await response2.ReadContentsAsync();

                result1.ShouldNotBe(result2);
                result1.ShouldBe(TestResult.SerializedDefault1);
                result2.ShouldBe(TestResult.SerializedDefault2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_ExpectContentsCachedAccrossCallBuildersOnDifferentThreads()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var response1 = await Task.Factory.StartNew(() =>
                    CreateBuilder()
                   .WithUri(server.ListeningUri)
                   .ResultAsync());

                var result1 = await response1.ReadContentsAsync();

                var response2 = await Task.Factory.StartNew(() =>
                    CreateBuilder()
                   .WithUri(server.ListeningUri)
                   .ResultAsync());

                var result2 = await response2.ReadContentsAsync();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOff_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await CreateBuilder()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithNoCache()
                        .ResultAsync().ReadContentsAsync();

                var result2 = await CreateBuilder()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithNoCache()
                        .ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsAreNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithNoCacheHeader());

                var result1 = await CreateBuilder()
                       .WithUri(server.ListeningUri)
                       .Advanced.WithNoCache()
                       .ResultAsync().ReadContentsAsync();

                var result2 = await CreateBuilder()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithNoCache()
                        .ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithPost_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync().ReadContentsAsync();

                await CreateBuilder().WithUri(server.ListeningUri).AsPost()
                       .ResultAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri)
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
                var childUri = parentUri.AppendPath("child");
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

                var parent1 = await CreateBuilder().WithUri(parentUri).Advanced.WithDependentUri(childUri).ResultAsync().ReadContentsAsync();
                var child1 = await CreateBuilder().WithUri(childUri).ResultAsync().ReadContentsAsync();
                var parent2 = await CreateBuilder().WithUri(parentUri).ResultAsync().ReadContentsAsync();
                var child2 = await CreateBuilder().WithUri(childUri).Advanced.ResultAsync().ReadContentsAsync();

                parent1.ShouldBe(parent2);
                child1.ShouldBe(child2);

                await CreateBuilder().WithUri(parentUri).AsPost().ResultAsync();

                var parent3 = await CreateBuilder().WithUri(parentUri).ResultAsync().ReadContentsAsync();
                parent3.ShouldBe("Parent Response3"); // because of the post

                var child3 = await CreateBuilder().WithUri(childUri).ResultAsync().ReadContentsAsync();
                child3.ShouldBe("Child Response2");
            }
        }

        [Fact]
        public async Task WithDependendUrlsThatAreSelfReferential_ExpectPostNoException()
        {


            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var parentUri = server.ListeningUri;
                var childUri = parentUri.AppendPath("child");
                var grandchildUri = parentUri.AppendPath("grandchild");
                server.WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response2").WithPrivateCacheHeader());

                var parent1 = await CreateBuilder().WithUri(parentUri).Advanced.WithDependentUris(new[] { parentUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var child1 = await CreateBuilder().WithUri(childUri).Advanced.WithDependentUris(new[] { childUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var grandchild1 = await CreateBuilder().WithUri(grandchildUri).Advanced.WithDependentUris(new[] { parentUri, childUri }).ResultAsync().ReadContentsAsync();

                await CreateBuilder().WithUri(parentUri).AsPost().ResultAsync();

                var parent2 = await CreateBuilder().WithUri(parentUri).Advanced.WithDependentUris(new[] { parentUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var child2 = await CreateBuilder().WithUri(childUri).Advanced.WithDependentUris(new[] { childUri, grandchildUri }).ResultAsync().ReadContentsAsync();
                var grandchild2 = await CreateBuilder().WithUri(grandchildUri).Advanced.WithDependentUris(new[] { parentUri, childUri }).ResultAsync().ReadContentsAsync();

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
                var childUri = parentUri.AppendPath("child");
                var grandchildUri = parentUri.AppendPath("grandchild");
                server.WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Parent Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Child Response2").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Grandchild Response2").WithPrivateCacheHeader());

                var parent1 = await CreateBuilder().WithUri(parentUri).Advanced.WithDependentUri(childUri).ResultAsync().ReadContentsAsync();
                var child1 = await CreateBuilder().WithUri(childUri).Advanced.WithDependentUri(grandchildUri).ResultAsync().ReadContentsAsync();
                var grandchild1 = await CreateBuilder().WithUri(grandchildUri).ResultAsync().ReadContentsAsync();

                await CreateBuilder().WithUri(parentUri).AsPost().ResultAsync();

                var parent2 = await CreateBuilder().WithUri(parentUri).Advanced.WithDependentUri(childUri).ResultAsync().ReadContentsAsync();
                var child2 = await CreateBuilder().WithUri(childUri).ResultAsync().ReadContentsAsync();
                var grandchild2 = await CreateBuilder().WithUri(childUri).ResultAsync().ReadContentsAsync();

                parent1.ShouldNotBe(parent2);
                child1.ShouldNotBe(child2);
                grandchild1.ShouldNotBe(grandchild2);
            }
        }


        [Fact]
        public async Task WhenCachingIsOn_WithPut_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync();

                await CreateBuilder().WithUri(server.ListeningUri).AsPut()
                       .ResultAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithPatch_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync();

                await CreateBuilder().WithUri(server.ListeningUri).AsPatch()
                       .ResultAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithDelete_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response1").WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Response2").WithPrivateCacheHeader());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                await CreateBuilder().WithUri(server.ListeningUri).AsDelete().ResultAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync().ReadContentsAsync();

                result1.ShouldNotBe(result2);
            }
        }
    }
}