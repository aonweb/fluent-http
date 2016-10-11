using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Serialization;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Hal
{
    [Collection("LocalWebServer Tests")]
    public class HalDeserializationTests
    {
        private readonly ITestOutputHelper _logger;

        public HalDeserializationTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Cache.DeleteAll();
        }

        private static IHalBuilder CreateBuilder()
        {
            return new HalBuilderFactory().Create().Advanced.WithCaching(false);
        }

        [Fact]
        public async Task CanDeserializeResource()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result = await CreateBuilder()
                    .WithLink(uri).ResultAsync<TestResource>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestResource.Default1());
            }
        }

        [Fact]
        public async Task CanDeserializeResourceWithLinks()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResourceWithLinks.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result = await CreateBuilder()
                    .WithLink(uri).ResultAsync<TestResourceWithLinks>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestResourceWithLinks.Default1());
            }
        }
        [Fact]
        public async Task CanDeserializeListWithEmbedded()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result = await CreateBuilder()
                    .WithLink(uri).ResultAsync<TestListResource>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestListResource.Default1());
            }
        }

        [Fact]
        public async Task CanDeserializeListWithEmbeddedWithLinks()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResourceWithLinks.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result = await CreateBuilder()
                    .WithLink(uri).ResultAsync<TestListResourceWithLinks>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestListResourceWithLinks.Default1());
            }
        }

        [Fact]
        public async Task CanDeserializeListWithEmbeddedWithLinksWithEmbeddedJsonProperty()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListEmbeddedPropertyParentsResource.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result = await CreateBuilder()
                    .WithLink(uri).ResultAsync<TestListEmbeddedPropertyParentsResource>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestListEmbeddedPropertyParentsResource.Default1());
            }
        }

        [Fact]
        public async Task CanDeserializeWithEmbedAsArray()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListEmbeddedArrayParentResource.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var result = await CreateBuilder()
                    .WithLink(uri).ResultAsync<TestListEmbeddedArrayParentResource>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestListEmbeddedArrayParentResource.Default1());
            }
        }

        [Fact]
        public async Task CanDeserializeCamelCaseError()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.NotFound).WithContent(TestError.SerializedDefault1).WithPrivateCacheHeader().WithDefaultExpiration());

                var ex = await Should.ThrowAsync<HttpErrorException<TestError>>(
                    CreateBuilder()
                        .WithLink(uri)
                        .WithErrorType<TestError>()
                        .SendAsync());

                ex.ShouldNotBeNull();
                ex.Error.ShouldBe(TestError.Default1());
            }
        }
    }
}