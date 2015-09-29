using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests
{
    public class TypedCachingTests
    {
        private readonly ITestOutputHelper _logger;

        public TypedCachingTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Caching.Enabled = true;
            Cache.Clear();
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var builder = new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri);

                var result1 = await builder.ResultAsync<TestResult>();

                var result2 = await builder.ResultAsync<TestResult>();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuilders()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();


                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuildersOnDifferentThreads()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await Task.Factory.StartNew(() =>
                    new TypedBuilderFactory().Create()
                   .WithUri(server.ListeningUri)
                   .ResultAsync<TestResult>().Result);

                var result2 = await Task.Factory.StartNew(() =>
                    new TypedBuilderFactory().Create()
                   .WithUri(server.ListeningUri)
                   .ResultAsync<TestResult>().Result);

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOnAndServerDoesntSendCacheHeaders_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOnAndServerDoesntSendCacheHeadersButTypeIsCacheable_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<CacheableTestResult>();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOnAndServerSendsNoCacheHeadersAndTypeisCacheableButDurationZeroOrLess_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2));

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<ExpiredTestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<ExpiredTestResult>();

                result1.ShouldBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOff_ExpectContentsNotCached()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .Advanced.WithCaching(false)
                    .ResultAsync<TestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .Advanced.WithCaching(false)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOnAndNoCacheCalled_ExpectContentsNotCachedAndCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .Advanced.WithNoCache()
                    .ResultAsync<TestResult>();

                var result3 = await new TypedBuilderFactory().Create()
                   .WithUri(server.ListeningUri)
                   .Advanced.WithNoCache()
                   .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
                result2.ShouldNotBe(result3);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsAreNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithNoCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithPost_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).AsPost()
                        .SendAsync();

                var result2 = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithPut_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).AsPut()
                        .SendAsync();

                var result2 = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithPatch_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).AsPatch()
                        .SendAsync();

                var result2 = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenHttpCachingIsOn_WithDelete_ExpectCacheInvalidated()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithNoCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .AsDelete()
                    .SendAsync();

                var result2 = await new TypedBuilderFactory().Create()
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
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                try
                {
                    await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .AsPut()
                    .SendAsync();
                }
                catch (HttpCallException)
                {
                    // expected
                }

                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldNotBe(result2);
            }
        }

        [Fact]
        public async Task WhenExceptionBeforeCall_ExpectExpectedExceptionAndContentsUntouched()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault1).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError).WithPrivateCacheHeader())
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResult.SerializedDefault2).WithPrivateCacheHeader());

                var result1 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                try
                {
                    await new TypedBuilderFactory().Create()
                        .WithUri(server.ListeningUri)
                        .Advanced.WithClientConfiguration(builder =>
                        {
                            throw new TestException();
                        })
                        .AsPut()
                        .SendAsync();
                }
                catch (TestException)
                {
                    // expected
                }


                var result2 = await new TypedBuilderFactory().Create()
                    .WithUri(server.ListeningUri)
                    .ResultAsync<TestResult>();

                result1.ShouldBe(result2);
            }
        }
 
    }
}