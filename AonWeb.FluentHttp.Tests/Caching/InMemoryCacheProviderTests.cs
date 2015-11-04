using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Settings;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests.Caching
{
    public class InMemoryCacheProviderTests
    {
        private ICacheProvider CreateProvider()
        {
            return new InMemoryCacheProvider(new InMemoryVaryByProvider());
        }

        private HttpResponseMessage CreateResponse(string result)
        {
            return new MockHttpResponseMessage(HttpStatusCode.OK).WithContent(result);
        }

        private ICacheContext CreateContextForTypedResult<T>(Uri uri, T result, HttpResponseMessage response)
        {
            var request = new MockHttpRequestMessage()
            {
                RequestUri = uri,
            };
            
            response.RequestMessage = request;
            var cacheSettings = new CacheSettings();
            var settings = new TypedBuilderSettings(new Formatter(), cacheSettings, null, null);
            settings.WithDefiniteResultType(typeof (T));
            var httpBuilderContext = new TypedBuilderContext(settings);
            var handlerContext = new TypedResultContext<T>(httpBuilderContext, request, response, result);
            return new CacheContext(cacheSettings, handlerContext);
        }

        private ICacheContext CreateContextForHttpResponse(Uri uri, HttpResponseMessage response, string accept = "application/json")
        {
            var request = new MockHttpRequestMessage()
            {
                RequestUri = uri,
            };

            request.Headers.Accept.ParseAdd(accept);
            response.RequestMessage = request;
            var cacheSettings = new CacheSettings();
            var settings = new HttpBuilderSettings(cacheSettings, null, null);
            settings.UriBuilder.Uri = uri;
            var httpBuilderContext = new HttpBuilderContext(settings);
            var handlerContext = new HttpSentContext(httpBuilderContext, request, response);
            return new CacheContext(cacheSettings, handlerContext);
        }

        [Fact]
        public async Task WhenTypedItemIsCached_CorrectValueCanBeRetrieved()
        {
            var provider = CreateProvider();
            var uri = new Uri("http://somedomain.com/resource1");
            var result = "result";
            var response = CreateResponse(result);
            var context = CreateContextForTypedResult(uri, result, response);
            var cacheEntry = new CacheEntry(result, response.RequestMessage, response, context);
            await provider.Put(context, cacheEntry);

            var entry = await provider.Get(context);
            var actual = entry.Value as string;

            actual.ShouldBe(result);
        }

        [Fact]
        public async Task WhenHttpItemIsCached_CorrectValueCanBeRetrieved()
        {
            var provider = CreateProvider();
            var uri = new Uri("http://somedomain.com/resource1");
            var result = "result";
            var response = CreateResponse(result);
            var context = CreateContextForHttpResponse(uri, response);
            var cacheEntry = new CacheEntry(response, response.RequestMessage, response, context);
            await provider.Put(context, cacheEntry);

            var entry = await provider.Get(context);
            var actualResponse = entry.Value as HttpResponseMessage;
            var actualResult = await actualResponse.ReadContentsAsync();

            actualResult.ShouldBe(result);
            
        }

        [Fact]
        public async Task WhenMultipleTypedItemsAreCached_CorrectValuesCanBeRetrieved()
        {
            var provider = CreateProvider();
            var uri1 = new Uri("http://somedomain.com/resource1");
            var result1 = "result1";
            var response1 = CreateResponse(result1);
            var context1 = CreateContextForTypedResult(uri1, result1, response1);
            var cacheEntry1 = new CacheEntry(result1, response1.RequestMessage, response1, context1);
            var uri2 = new Uri("http://somedomain.com/resource2");
            var result2 = "result2";
            var response2 = CreateResponse(result2);
            var context2 = CreateContextForTypedResult(uri2, result2, response2);
            var cacheEntry2 = new CacheEntry(result2, response2.RequestMessage, response2, context2);

            await provider.Put(context1, cacheEntry1);
            await provider.Put(context2, cacheEntry2);

            var entry1 = await provider.Get(context1);
            var actual1 = entry1.Value as string;
            var entry2 = await provider.Get(context2);
            var actual2 = entry2.Value as string;

            actual1.ShouldNotBe(actual2);
            actual1.ShouldBe(result1);
            actual2.ShouldBe(result2);
        }

        [Fact]
        public async Task WhenMultipleHttpItemsAreCached_CorrectValuesCanBeRetrieved()
        {
            var provider = CreateProvider();
            var uri1 = new Uri("http://somedomain.com/resource1");
            var result1 = "result1";
            var response1 = CreateResponse(result1);
            var context1 = CreateContextForHttpResponse(uri1, response1);
            var cacheEntry1 = new CacheEntry(response1, response1.RequestMessage, response1, context1);
            var uri2 = new Uri("http://somedomain.com/resource2");
            var result2 = "result2";
            var response2 = CreateResponse(result2);
            var context2 = CreateContextForHttpResponse(uri2, response2);
            var cacheEntry2 = new CacheEntry(response2, response2.RequestMessage, response2, context2);

            await provider.Put(context1, cacheEntry1);
            await provider.Put(context2, cacheEntry2);

            var entry1 = await provider.Get(context1);
            var actualResponse1 = entry1.Value as HttpResponseMessage;
            var actualResult1 = await actualResponse1.ReadContentsAsync();
            var entry2 = await provider.Get(context2);
            var actualResponse2 = entry2.Value as HttpResponseMessage;
            var actualResult2 = await actualResponse2.ReadContentsAsync();

            actualResult1.ShouldNotBe(actualResult2);
            actualResult1.ShouldBe(result1);
            actualResult2.ShouldBe(result2);
        }

        [Fact]
        public async Task WhenMultipleTypedItemsAreCached_WithDifferentTypes_CorrectValuesCanBeRetrieved()
        {
            var provider = CreateProvider();
            var uri = new Uri("http://somedomain.com/resource1");
            long result1 = 3;
            short result2 = 5;
            var response1 = CreateResponse(result1.ToString());
            var context1 = CreateContextForTypedResult(uri, result1, response1);
            var cacheEntry1 = new CacheEntry(result1, response1.RequestMessage, response1, context1);
            var response2 = CreateResponse(result2.ToString());
            var context2 = CreateContextForTypedResult(uri, result2, response2);
            var cacheEntry2 = new CacheEntry(result2, response2.RequestMessage, response2, context2);

            await provider.Put(context1, cacheEntry1);
            await provider.Put(context2, cacheEntry2);

            var entry1 = await provider.Get(context1);
            var entry2 = await provider.Get(context2);

            entry1.Value.ShouldBeOfType<long>();
            entry2.Value.ShouldBeOfType<short>();
            entry1.Value.ShouldBe(result1);
            entry2.Value.ShouldBe(result2);
        }

        [Fact]
        public async Task WhenMultipleHttpItemsAreCached_WithVaryBy_CorrectValuesCanBeRetrieved()
        {
            var provider = CreateProvider();
            var uri = new Uri("http://somedomain.com/resource");
            var result1 = "result1";
            var response1 = CreateResponse(result1);
            var context1 = CreateContextForHttpResponse(uri, response1, "application/json");
            var cacheEntry1 = new CacheEntry(response1, response1.RequestMessage, response1, context1);
            var result2 = "result2";
            var response2 = CreateResponse(result2);
            var context2 = CreateContextForHttpResponse(uri, response2, "text/plain");
            var cacheEntry2 = new CacheEntry(response2, response2.RequestMessage, response2, context2);

            await provider.Put(context1, cacheEntry1);
            await provider.Put(context2, cacheEntry2);

            var entry1 = await provider.Get(context1);
            var actualResponse1 = entry1.Value as HttpResponseMessage;
            var actualResult1 = await actualResponse1.ReadContentsAsync();
            var entry2 = await provider.Get(context2);
            var actualResponse2 = entry2.Value as HttpResponseMessage;
            var actualResult2 = await actualResponse2.ReadContentsAsync();

            actualResult1.ShouldNotBe(actualResult2);
            actualResult1.ShouldBe(result1);
            actualResult2.ShouldBe(result2);
        }
    }
}