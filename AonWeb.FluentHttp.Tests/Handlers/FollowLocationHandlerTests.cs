using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Handlers
{
    [Collection("LocalWebServer Tests")]
    public class FollowLocationHandlerTests
    {
        private readonly ITestOutputHelper _logger;

        public FollowLocationHandlerTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Current.GetCachingDefaults().Enabled = false;
        }

        [Fact]
        public async Task WithResponse_WhenStatusIsCreated_ExpectLocationFollowed()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Created).WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actualUrl = null;
                HttpMethod actualMethod = null;
                var hasBody = true;
                server.WithRequestInspector(
                    async r =>
                        {
                            actualUrl = r.RequestUri;
                            actualMethod = r.Method;

                            if (r.Content == null)
                                hasBody = false;
                            else
                            {
                                var content = await r.Content.ReadAsStringAsync();
                                hasBody = !string.IsNullOrWhiteSpace(content);
                            }
                        });

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).AsPost().WithContent("POST CONTENT").ResultAsync().ReadContentsAsync();

                actualUrl.ShouldBe(expected);
                actualMethod.ShouldBe(HttpMethod.Get);
                hasBody.ShouldBeFalse();
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithResult_WhenStatusIsCreated_ExpectLocationFollowed()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Created).WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actualUrl = null;
                HttpMethod actualMethod = null;
                var hasBody = true;
                server.WithRequestInspector(
                    async r =>
                    {
                        actualUrl = r.RequestUri;
                        actualMethod = r.Method;

                        if (r.Content == null)
                            hasBody = false;
                        else {
                            var content = await r.Content.ReadAsStringAsync();
                            hasBody = !string.IsNullOrWhiteSpace(content);
                        }
                    });

                //act
                var result = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).AsPost().WithContent("POST CONTENT").ResultAsync<string>();

                actualUrl.ShouldBe(expected);
                actualMethod.ShouldBe(HttpMethod.Get);
                hasBody.ShouldBeFalse();
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithResult_WhenNotEnabledAndStatusIsCreated_ExpectNotFollowedAndNoException()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var redirectUrl = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server.WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Created).WithHeader("Location", redirectUrl.ToString()));

                var calledBack = false;

                //act
                await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).Advanced
                    .WithAutoFollowConfiguration(h => h.WithAutoFollow(false).WithCallback(ctx => calledBack = true))
                    .SendAsync();

                calledBack.ShouldBeFalse();
            }
        }
    }
}