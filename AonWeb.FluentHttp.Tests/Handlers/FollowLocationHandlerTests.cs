using System.Net;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests.Handlers
{
    public class FollowLocationHandlerTests
    {
        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        public FollowLocationHandlerTests()
        {
            Defaults.Caching.Enabled = false;
        }

        [Fact]
        public async Task WithResponse_WhenStatusIsCreated_ExpectLocationFollowed()
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");

            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Created).WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                string actualUrl = null;
                string actualMethod = null;
                var hasBody = true;
                server.WithRequestInspector(
                    r =>
                        {
                            actualUrl = r.Url.ToString();
                            actualMethod = r.HttpMethod;
                            hasBody = r.HasEntityBody;
                        });

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).AsPost().WithContent("POST CONTENT").ResultAsync().ReadContentsAsync();

                actualUrl.ShouldBe(expected);
                actualMethod.ShouldBe("GET");
                hasBody.ShouldBeFalse();
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithResult_WhenStatusIsCreated_ExpectLocationFollowed()
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");

            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Created).WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("\"Success\""));

                string actualUrl = null;
                string actualMethod = null;
                var hasBody = true;
                server.WithRequestInspector(
                    r =>
                    {
                        actualUrl = r.Url.ToString();
                        actualMethod = r.HttpMethod;
                        hasBody = r.HasEntityBody;
                    });

                //act
                var result = await new TypedBuilderFactory().Create().WithUri(TestUriString).AsPost().WithContent("POST CONTENT").ResultAsync<string>();

                actualUrl.ShouldBe(expected);
                actualMethod.ShouldBe("GET");
                hasBody.ShouldBeFalse();
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithResult_WhenNotEnabledAndStatusIsCreated_ExpectNotFollowedAndNoException()
        {
            var redirectUrl = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.Created).WithHeader("Location", redirectUrl));

                var calledBack = false;

                //act
                await new TypedBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithAutoFollowConfiguration(h => h.WithAutoFollow(false).WithCallback(ctx => calledBack = true))
                    .SendAsync();

                calledBack.ShouldBeFalse();
            }
        }
    }
}