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
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Handlers
{
    [Collection("LocalWebServer Tests")]
    public class RedirectionHandlerTests
    {
        private readonly ITestOutputHelper _logger;

        public RedirectionHandlerTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Current.GetCachingDefaults().Enabled = false;
        }

        [Theory]
        [InlineData(HttpStatusCode.Found)]
        [InlineData(HttpStatusCode.Redirect)]
        [InlineData(HttpStatusCode.MovedPermanently)]
        public async Task AutoRedirect_WhenCallRedirects_ExpectRedirectOnByDefaultAndLocationFollowed(HttpStatusCode statusCode)
        {
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");

                server
                    .WithNextResponse(new MockHttpResponseMessage { StatusCode = statusCode }.WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actual = null;
                server.WithRequestInspector(r => actual = r.RequestUri);

                //act
                var response = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync();
                var result = await response.ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");

            }
        }

        [Fact]
        public async Task AutoRedirect_WhenCallRedirects_ExpectContentSent()
        {
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");

                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                var actual = new List<string>();
                server.WithRequestInspector(async r =>
                {
                    var content = await r.Content.ReadAsStringAsync();
                    actual.Add(content);
                });

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).AsPost().WithContent("Content").ResultAsync().Result.ReadContentsAsync();

                actual.Count.ShouldBe(2);
                actual[1].ShouldBe("Content");
            }
        }

        [Theory]
        [InlineData("", "/redirect",  "redirect")]
        [InlineData("post", "/redirect",  "redirect")]
        [InlineData("post", "redirect",  "post/redirect")]
        public async Task AutoRedirect_WhenCallRedirectsWithRelativePath_ExpectPathHandled(string uriData, string path, string expectedData)
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = UriHelpers.CombineVirtualPaths(server.ListeningUri, uriData);
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, expectedData);

                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", path))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actual = null;
                server.WithRequestInspector(r => actual = r.RequestUri);

                //act
                var response = await new HttpBuilderFactory().Create().WithUri(uri).AsPost().WithContent("Content").ResultAsync();
                var result = await response.Content.ReadAsStringAsync();

                result.ShouldBe("Success");

                actual.ShouldBe(expected);
            }
        }

        [Fact]
        public async Task AutoRedirect_WhenNotEnabledCallRedirects_ExpectNotFollowed()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var redirectUrl = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl.ToString()));

                var calledBack = false;

                //act
                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).Advanced
                    .WithRedirectConfiguration(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                    .ResultAsync();

                calledBack.ShouldBeFalse();
            }
        }

        [Fact]
        public async Task AutoRedirect_WhenEnabledAndCallRedirects_ExpectRedirect()
        {
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server.WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actual = null;
                server.WithRequestInspector(r => actual = r.RequestUri);

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).Advanced.WithRedirectConfiguration(h => h.WithAutoRedirect()).ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithCallback_WhenAction_ExpectConfigurationApplied()
        {
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var redirectUrl = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage());

                int expected = 2;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).Advanced
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
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server
                    .WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.Created))
                    .WithNextResponse(new MockHttpResponseMessage());

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync();

                result.StatusCode.ShouldBe(HttpStatusCode.Created);
            }
        }

        [Fact]
        public async Task WithRedirectStatusCode_WithAddedStatusCode_ExpectAutoRetry()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server.WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.MultipleChoices).WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actual = null;
                server.WithRequestInspector(r => actual = r.RequestUri);

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                    .Advanced.WithRedirectConfiguration(h => h.WithRedirectStatusCode(HttpStatusCode.MultipleChoices)).ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithRedirectValidator_WithCustomValidator_ExpectValidatorUsed()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server.WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.MultipleChoices).WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage().WithContent("Success"));

                Uri actual = null;
                server.WithRequestInspector(r => actual = r.RequestUri);

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
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
            Should.Throw<ArgumentNullException>(() => new HttpBuilderFactory().Create().WithUri("http://somedomain.com")
                .Advanced.WithRedirectConfiguration(h =>
                    h.WithRedirectValidator(null)));
        }

        [Fact]
        public async Task WithAutoRedirect_WithMaxRedirect_ExpectException()
        {
            await Should.ThrowAsync<MaximumAutoRedirectsException>(async () =>
            {
                using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
                {
                    var redirectUrl = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                    server.WithAllResponses(new MockHttpResponseMessage(HttpStatusCode.Redirect)
                            .WithHeader("Location", redirectUrl.ToString()));

                    //act
                    await new HttpBuilderFactory().Create()
                            .WithUri(server.ListeningUri)
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
           
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server
                    .WithNextResponse(new MockHttpResponseMessage { StatusCode = statusCode }.WithHeader("Location", expected.ToString()))
                    .WithNextResponse(new MockHttpResponseMessage
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        StatusCode = HttpStatusCode.OK,
                        ContentString = TestResult.SerializedDefault1
                    });

                Uri actual = null;
                server.WithRequestInspector(r => actual = r.RequestUri);

                //act
                var result = await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).ResultAsync<TestResult>();

                actual.ShouldBe(expected);
                result.ShouldBe(TestResult.Default1());
            }
        }

        [Fact]
        public async Task AutoRedirectOnTypedCallBuilder_WhenNotEnabledCallRedirects_ExpectNotFollowedAndExceptionThrown()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var redirectUrl = UriHelpers.CombineVirtualPaths(server.ListeningUri, "redirect");
                server.WithAllResponses(new MockHttpResponseMessage(HttpStatusCode.Redirect).WithHeader("Location", redirectUrl.ToString()));

                var calledBack = false;

                //act
                await Should.ThrowAsync<HttpErrorException<string>>(
                      new TypedBuilderFactory().Create().WithUri(server.ListeningUri).Advanced
                         .WithRedirectConfiguration(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                         .SendAsync());

                calledBack.ShouldBeFalse();
            }
        }
    }
}