using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Serialization;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Hal
{
    public class HalDeserializationTests
    {
        private readonly ITestOutputHelper _logger;
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new HalResourceConverter()
            },
            ContractResolver = new CamelCasePropertyNamesContractResolver()
            {
                IgnoreSerializableInterface = true,
                IgnoreSerializableAttribute = true,
                SerializeCompilerGeneratedMembers = false
            },
            Formatting = Formatting.Indented
        };

        public HalDeserializationTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Fact]
        public async Task CanDeserializeResource()
        {

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResource.SerializedDefault1).WithPrivateCacheHeader());

                var result = await new HalBuilderFactory().Create()
                    .WithLink(uri).ResultAsync<TestResource>();

                result.ShouldNotBeNull();
                result.ShouldBe(TestResource.Default1());
            }
        }

        [Fact]
        public async Task CanDeserializeResourceWithLinks()
        {

            _logger.WriteLine(JsonConvert.SerializeObject(TestResourceWithLinks.Default1(), _settings));

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                server
                    .WithNextResponse(new MockHttpResponseMessage().WithContent(TestResourceWithLinks.SerializedDefault1).WithPrivateCacheHeader());

                var result = await new HalBuilderFactory().Create()
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
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResource.SerializedDefault1).WithPrivateCacheHeader());

                var result = await new HalBuilderFactory().Create()
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
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListResourceWithLinks.SerializedDefault1).WithPrivateCacheHeader());

                var result = await new HalBuilderFactory().Create()
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
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListEmbeddedPropertyParentsResource.SerializedDefault1).WithPrivateCacheHeader());

                var result = await new HalBuilderFactory().Create()
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
                server.WithNextResponse(new MockHttpResponseMessage().WithContent(TestListEmbeddedArrayParentResource.SerializedDefault1).WithPrivateCacheHeader());

                var result = await new HalBuilderFactory().Create()
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
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.NotFound).WithContent(TestError.SerializedDefault1).WithPrivateCacheHeader());

                var ex = await Should.ThrowAsync<HttpErrorException<TestError>>(
                    new HalBuilderFactory().Create()
                        .WithLink(uri)
                        .WithErrorType<TestError>()
                        .SendAsync());

                ex.ShouldNotBeNull();
                ex.Error.ShouldBe(TestError.Default1());
            }
        }
    }
}