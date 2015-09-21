using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests.Handlers
{
    public class RedirectionHandlerTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        public RedirectionHandlerTests()
        {
            Defaults.Caching.Enabled = false;
        }

        #endregion

        [Theory]
        [InlineData(HttpStatusCode.Found)]
        [InlineData(HttpStatusCode.Redirect)]
        [InlineData(HttpStatusCode.MovedPermanently)]
        public async Task AutoRedirect_WhenCallRedirects_ExpectRedirectOnByDefaultAndLocationFollowed(HttpStatusCode statusCode)
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse { StatusCode = statusCode }.WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                string actual = null;
                server.WithRequestInspector(r => actual = r.Url.ToString());

                //act
                var response = await new HttpBuilderFactory().Create().WithUri(TestUriString).ResultAsync();
                var result = await response.ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");

            }
        }

        [Fact]
        public async Task AutoRedirect_WhenCallRedirects_ExpectContentSent()
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                var actual = new List<string>();
                server.WithRequestInspector(r => actual.Add(r.Body));

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).AsPost().WithContent("Content").ResultAsync().Result.ReadContentsAsync();

                actual.Count.ShouldBe(2);
                actual[1].ShouldBe("Content");
            }
        }

        [Theory]
        [InlineData(TestUriString, "/redirect", TestUriString + "redirect")]
        [InlineData(TestUriString + "post", "/redirect", TestUriString + "redirect")]
        [InlineData(TestUriString + "post", "redirect", TestUriString + "post/redirect")]
        public async Task<string> AutoRedirect_WhenCallRedirectsWithRelativePath_ExpectPathHandled(string uri, string path, string expected)
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", path))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                string actual = null;
                server.WithRequestInspector(r => actual = r.Url.ToString());

                //act
                var response = await new HttpBuilderFactory().Create().WithUri(uri).AsPost().WithContent("Content").ResultAsync();
                var result = await response.Content.ReadAsStringAsync();

                result.ShouldBe("Success");

                return actual;
            }
        }

        [Fact]
        public async Task AutoRedirect_WhenNotEnabledCallRedirects_ExpectNotFollowed()
        {
            var redirectUrl = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl));

                var calledBack = false;

                //act
                await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithRedirectConfiguration(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                    .ResultAsync();

                calledBack.ShouldBeFalse();
            }
        }

        [Fact]
        public async Task AutoRedirect_WhenEnabledAndCallRedirects_ExpectRedirect()
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                string actual = null;
                server.WithRequestInspector(r => actual = r.Url.ToString());

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced.WithRedirectConfiguration(h => h.WithAutoRedirect()).ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithCallback_WhenAction_ExpectConfigurationApplied()
        {
            var redirectUrl = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl))
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl))
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl))
                    .WithNextResponse(new LocalResponse());

                int expected = 2;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RedirectHandler>(h => h.WithCallback(ctx =>
                    {
                        if (ctx.CurrentRedirectionCount >= 1)
                            ctx.ShouldRedirect = false;
                    }))
                    .ResultAsync();

                actual.ShouldBe(expected);

            }
        }

        [Fact]
        public async Task AutoRedirect_WithNoLocationHeader_ExpectNoRedirect()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.Created))
                    .WithNextResponse(new LocalResponse());

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).ResultAsync();

                result.StatusCode.ShouldBe(HttpStatusCode.Created);

            }
        }

        [Fact]
        public async Task WithRedirectStatusCode_WithAddedStatusCode_ExpectAutoRetry()
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.MultipleChoices).WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                string actual = null;
                server.WithRequestInspector(r => actual = r.Url.ToString());

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString)
                    .Advanced.WithRedirectConfiguration(h => h.WithRedirectStatusCode(HttpStatusCode.MultipleChoices)).ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithRedirectValidator_WithCustomValidator_ExpectValidatorUsed()
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.MultipleChoices).WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                string actual = null;
                server.WithRequestInspector(r => actual = r.Url.ToString());

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString)
                    .Advanced.WithRedirectConfiguration(h =>
                        h.WithRedirectValidator(r => r.Result.StatusCode == HttpStatusCode.MultipleChoices))
                    .ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public void WithRedirectValidator_WithValidatorIsNull_ExpectException()
        {
            Should.Throw<ArgumentNullException>(() => new HttpBuilderFactory().Create().WithUri(TestUriString)
                .Advanced.WithRedirectConfiguration(h =>
                    h.WithRedirectValidator(null)));
        }

        [Fact]
        public async Task WithAutoRedirect_WithMaxRedirect_ExpectException()
        {
            var redirectUrl = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");

            await Should.ThrowAsync<MaximumAutoRedirectsException>(async () =>
            {
                using (var server = LocalWebServer.ListenInBackground(TestUriString))
                {
                    server.WithAllResponses(new LocalResponse(HttpStatusCode.Redirect)
                            .WithHeader("Location", redirectUrl));

                    //act
                    await new HttpBuilderFactory().Create()
                            .WithUri(TestUriString)
                            .Advanced.WithRedirectConfiguration(h => h.WithAutoRedirect(1))
                            .ResultAsync();

                }
            });

        }

        [Theory]
        [InlineData(HttpStatusCode.Found)]
        [InlineData(HttpStatusCode.Redirect)]
        [InlineData(HttpStatusCode.MovedPermanently)]
        public async Task AutoRedirectOnTypedCallBuilder_WhenCallRedirects_ExpectRedirectOnByDefaultAndLocationFollowed(HttpStatusCode statusCode)
        {
            var expected = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse { StatusCode = statusCode }.WithHeader("Location", expected))
                    .WithNextResponse(new LocalResponse
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        StatusCode = HttpStatusCode.OK,
                        Content = TestResult.SerializedDefault
                    });

                string actual = null;
                server.WithRequestInspector(r => actual = r.Url.ToString());

                //act
                var result = await new TypedBuilderFactory().Create().WithUri(TestUriString).ResultAsync<TestResult>();

                actual.ShouldBe(expected);
                result.ShouldBe(TestResult.Default());
            }
        }

        [Fact]
        public async Task AutoRedirectOnTypedCallBuilder_WhenNotEnabledCallRedirects_ExpectNotFollowedAndExceptionThrown()
        {
            var redirectUrl = UriHelpers.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithAllResponses(new LocalResponse(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl));

                var calledBack = false;

                //act
                await Should.ThrowAsync<HttpErrorException<string>>(
                      new TypedBuilderFactory().Create().WithUri(TestUriString).Advanced
                         .WithRedirectConfiguration(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                         .SendAsync());

                calledBack.ShouldBeFalse();
            }
        }
    }
}