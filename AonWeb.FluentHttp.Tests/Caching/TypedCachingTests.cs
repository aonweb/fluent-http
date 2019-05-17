using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
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
    public class TypedCachingTests
    {
        private readonly ITestOutputHelper _logger;

        public TypedCachingTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Cache.DeleteAll();
        }

        private static ITypedBuilder CreateBuilder(TimeSpan? duration = null)
        {
            return new TypedBuilderFactory().Create().Advanced.WithCaching(true).WithDefaultDurationForCacheableResults(duration);
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerSendsNotModified_ExpectContentsCached()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                  .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1)
                    .WithMustRevalidateHeader()
                    .WithMaxAge(TimeSpan.FromSeconds(10)))
                  .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.NotModified).WithDefaultExpiration())
                  .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.NotModified).WithDefaultExpiration());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync<TestResult>();

                await Task.Delay(TimeSpan.FromSeconds(2));

                var result2 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync<TestResult>();

                var result3 = await CreateBuilder().WithUri(server.ListeningUri).ResultAsync<TestResult>();

                result1.ShouldBe(TestResult.Default1());
                result2.ShouldBe(TestResult.Default1());
                result3.ShouldBe(TestResult.Default1());
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var builder = CreateBuilder()
                    .WithUri(server.ListeningUri);

                var result1 = await builder.ResultAsync<TestResult>();

                var result2 = await builder.ResultAsync<TestResult>();

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
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder().WithUri(firstUri).ResultAsync<TestResult>();

                var result2 = await CreateBuilder().WithUri(secondUri).ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
                result1.ShouldBe(TestResult.Default1());
                result2.ShouldBe(TestResult.Default2());
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_ExpectContentsCachedAccrossCallBuilders()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();


                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_ExpectContentsCachedAccrossCallBuildersOnDifferentThreads()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());
                var uri = server.ListeningUri;
                var result1 = await Task.Factory.StartNew(() =>
                    CreateBuilder()
                   .WithUri(uri)
                   .ResultAsync<TestResult>().Result);

                var result2 = await Task.Factory.StartNew(() =>
                    CreateBuilder()
                   .WithUri(uri)
                   .ResultAsync<TestResult>().Result);

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerDoesNotSendCacheHeaders_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerDoesNotSendCacheHeadersButTypeIsCacheable_ExpectContentsCached()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerDoesNotSendNoCacheHeadersAndTypeIsCacheableButDurationZeroOrLess_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResultWithDurationZero>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResultWithDurationZero>();

                result1.ShouldNotBe(result2);
                result1.ShouldBe(TestResult.Default1());
                result2.ShouldBe(TestResult.Default2());
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerDoesNotSendNoCacheHeadersAndTypeIsCacheableButDurationNullAndDefaultIsNotNull_ExpectContentsCached()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await CreateBuilder(TimeSpan.FromMinutes(5))
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResultWithDurationNull>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResultWithDurationNull>();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerDoesNotSendNoCacheHeadersAndTypeIsCacheableButDurationNullAndDefaultIsNull_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResultWithDurationNull>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResultWithDurationNull>();

                result1.ShouldNotBe(result2);
                result1.ShouldBe(TestResult.Default1());
                result2.ShouldBe(TestResult.Default2());
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerSendsNoCacheHeadersAndTypeIsCacheableButDurationZeroOrLess_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithNoCacheNoStoreHeader());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                result1.ShouldNotBe(result2);
                result1.ShouldBe(TestResult.Default1());
                result2.ShouldBe(TestResult.Default2());
            }
        }

        [Fact]
        public async Task WhenCachingIsOff_ExpectContentsNotCached()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .Advanced.WithCaching(false)
                    .ResultAsync<TestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .Advanced.WithCaching(false)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndNoCacheCalled_ExpectContentsNotCachedAndCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .Advanced.WithNoCache()
                    .ResultAsync<TestResult>();

                var result3 = await CreateBuilder()
                   .WithUri(server.ListeningUri)
                   .Advanced.WithNoCache()
                   .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
                result2.ShouldNotBe(result3);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsAreNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithNoCacheNoStoreHeader());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerSendsNoCacheHeaderAndTypeIsCacheable_ExpectContentsAreNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithNoCacheNoStoreHeader());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithPost_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                await CreateBuilder().WithUri(server.ListeningUri).AsPost()
                        .SendAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithPut_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                await CreateBuilder().WithUri(server.ListeningUri).AsPut()
                        .SendAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithPatch_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                await CreateBuilder().WithUri(server.ListeningUri).AsPatch()
                        .SendAsync();

                var result2 = await CreateBuilder().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOn_WithDelete_ExpectCacheInvalidated()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheNoStoreHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .AsDelete()
                    .SendAsync();

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenExceptionInCall_ExpectContentsExpired()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                try
                {
                    await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .AsPut()
                    .SendAsync();
                }
                catch (HttpRequestException)
                {
                    // expected
                }

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenExceptionInHandler_ExpectContentsExpired()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.OK).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                try
                {
                    await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .AsPut()
                    .Advanced
                    .OnSending(context => { throw new Exception("Boom!"); })
                    .SendAsync();
                }
                catch (Exception)
                {
                    // expected
                }

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenExceptionBeforeCall_ExpectExceptionAndContentsUntouched()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.OK).WithPrivateCacheHeader().WithDefaultExpiration())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader().WithDefaultExpiration());

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                try
                {
                    await CreateBuilder()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithMethod((HttpMethod)null)
                        .SendAsync();
                }
                catch (ArgumentNullException) { }

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenCachingIsOnAndServerSendsEtag_ExpectNextRequestContainsIfNoneMatch()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var etag = "12345";

                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1)
                        .WithEtag(etag).WithMaxAge(TimeSpan.FromSeconds(1)).WithMustRevalidateHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1)
                        .WithEtag("54321").WithMaxAge(TimeSpan.Zero));

                string ifNoneMatch = null;

                var result1 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                await Task.Delay(TimeSpan.FromMilliseconds(1002));

                var result2 = await CreateBuilder()
                    .WithUri(server.ListeningUri)
                    .Advanced.OnSending(ctx =>
                    {
                        ifNoneMatch = ctx.Request.Headers?.IfNoneMatch?.FirstOrDefault()?.Tag;
                    })
                    .ResultAsync<TestResult>();

                ifNoneMatch.ShouldNotBeNull();
                ifNoneMatch.ShouldBe("\"" + etag + "\"");
            }
        }

    }
}