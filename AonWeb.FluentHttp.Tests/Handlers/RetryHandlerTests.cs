using System;
using System.Net;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests.Handlers
{
    public class RetryHandlerTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        public RetryHandlerTests()
        {
            Defaults.Caching.Enabled = false;
        }

        #endregion

        [Fact]
        public async Task AutoRetry_WhenCallFails_ExpectRetryOnByDefault()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse().WithContent("Success"));

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(h => h.WithAutoRetry(1, TimeSpan.FromMilliseconds(2)))
                        .ResultAsync().ReadContentsAsync();

                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task WithAutoRetry_WhenEnabledAndCallFails_ExpectRetryWithDefaults()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable));

                int expected = Defaults.Handlers.MaxAutoRetries;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(h => h.WithAutoRetry())
                        .ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
            }
        }

        [Fact]
        public async Task AutoRetry_WhenNotEnabledAndCallFails_ExpectNoRetry()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable));

                var calledBack = false;

                //act
                await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithRetryConfiguration(h => h.WithAutoRetry(false).WithCallback(ctx => calledBack = true))
                        .ResultAsync();

                calledBack.ShouldBeFalse();
            }
        }

        [Fact]
        public async Task AutoRetry_WithAutoRetryConfigurationApplied_ExpectConfigurationHonored()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse().WithContent("Success"))
                    ;
                int expected = 4;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(h => h.WithAutoRetry(3, TimeSpan.FromMilliseconds(3)))
                    .ResultAsync();

                actual.ShouldBe(expected);
            }
        }

        [Fact]
        public async Task AutoRetry_ExpectStopAfterMaximum()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithAllResponses(new LocalResponse(HttpStatusCode.ServiceUnavailable));

                int expected = 3;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).ResultAsync();

                actual.ShouldBe(expected);
                result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
            }
        }

        [Fact]
        public void WithRetryStatusCode_WithAddedStatusCode_ExpectAutoRetry()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithAllResponses(new LocalResponse(HttpStatusCode.InternalServerError));

                int expected = 3;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                var result = new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced.WithRetryConfiguration(h => h.WithRetryStatusCode(HttpStatusCode.InternalServerError)).ResultAsync().Result;

                actual.ShouldBe(expected);
                result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            }
        }

        [Fact]
        public void WithRetryValidator_WithCustomValidator_ExpectValidatorUsed()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithAllResponses(new LocalResponse(HttpStatusCode.InternalServerError));

                int expected = 3;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                var result = new HttpBuilderFactory().Create().WithUri(TestUriString)
                    .Advanced.WithRetryConfiguration(h =>
                        h.WithRetryValidator(r => r.Result.StatusCode == HttpStatusCode.InternalServerError))
                    .ResultAsync().Result;

                actual.ShouldBe(expected);
                result.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            }
        }

        [Fact]
        public void WithRetryValidator_WithValidatorIsNull_ExpectException()
        {
            //act
            Should.Throw<ArgumentNullException>(() => new HttpBuilderFactory().Create().WithUri(TestUriString)
                .Advanced.WithRetryConfiguration(h =>
                    h.WithRetryValidator(null)));
        }

        [Fact]
        public async Task WithCallback_WithCallback_ExpectConfigurationApplied()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable))
                    .WithNextResponse(new LocalResponse { Content = "Success" });

                int expected = 2;
                int actual = 0;
                server.WithRequestInspector(r => actual++);

                //act
                await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(h => h.WithCallback(ctx =>
                    {
                        if (ctx.CurrentRetryCount >= 1)
                            ctx.ShouldRetry = false;
                    }))
                    .ResultAsync();

                actual.ShouldBe(expected);

            }
        }

        [Fact]
        public async Task AutoRetry_WithRetryHeaderDelta_ExpectHeaderHonored()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable).WithHeader("Retry-After", "3"))
                    .WithNextResponse(new LocalResponse { Content = "Success" });

                var expected = TimeSpan.FromSeconds(3);
                var actual = TimeSpan.Zero;

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(h => h.WithCallback(ctx =>
                    {
                        actual = ctx.RetryAfter;
                    }))
                    .ResultAsync().ReadContentsAsync();

                actual.ShouldBe(expected);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public async Task AutoRetry_WithRetryHeaderDate_ExpectHeaderHonored()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable)
                        .WithHeader("Retry-After", DateTime.UtcNow.AddSeconds(2).ToString("r")))
                    .WithNextResponse(new LocalResponse { Content = "Success" });

                var actual = TimeSpan.Zero;

                //act
                var result = await new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(h => h.WithCallback(ctx =>
                    { actual = ctx.RetryAfter; }))
                    .ResultAsync().ReadContentsAsync();


                actual.TotalMilliseconds.ShouldBeGreaterThan(1);
                actual.TotalMilliseconds.ShouldBeLessThanOrEqualTo(2500);
                result.ShouldBe("Success");
            }
        }

        [Fact]
        public void WithAutoRetry_WithRetryHeaderGreaterThanMax_ExpectRetryNotCalled()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .WithNextResponse(new LocalResponse(HttpStatusCode.ServiceUnavailable).WithHeader("Retry-After", "6000"))
                    .WithNextResponse(new LocalResponse { Content = "Success" });

                var expected = TimeSpan.FromMilliseconds(2);
                var actual = TimeSpan.Zero;

                //act
                var result = new HttpBuilderFactory().Create().WithUri(TestUriString).Advanced
                    .WithHandlerConfiguration<RetryHandler>(
                        h =>
                            {
                                h.WithAutoRetry(1, TimeSpan.FromMilliseconds(2));
                                h.WithCallback(ctx =>
                                {
                                    actual = ctx.RetryAfter;
                                });
                            })
                    .ResultAsync().Result;

                actual.ShouldBe(expected);
                result.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}