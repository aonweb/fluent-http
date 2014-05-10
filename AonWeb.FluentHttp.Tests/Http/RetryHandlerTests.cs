using System;
using System.Net;

using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class RetryHandlerTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion

        [Test]
        public void AutoRetry_WhenCallFails_ExpectRetryOnByDefault()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                //act
                var result = HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(h => h.WithAutoRetry(1, TimeSpan.FromMilliseconds(2)))
                        .Result().ReadContents();

                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void WithAutoRetry_WhenEnabledAndCallFails_ExpectRetryWithDefaults()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable });

                int expected = 3;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(h => h.WithAutoRetry())
                        .Result().ReadContents();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AutoRetry_WhenNotEnabledAndCallFails_ExpectNoRetry()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable });

                var calledBack = false;

                //act
                HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureRetries(h => h.WithAutoRetry(false).WithCallback(ctx => calledBack = true))
                        .Result();

                Assert.IsFalse(calledBack);
            }
        }

        [Test]
        public void AutoRetry_WithAutoRetryConfigurationApplied_ExpectConfigurationHonored()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" })
                    ;
                int expected = 4;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(h => h.WithAutoRetry(3, TimeSpan.FromMilliseconds(3)))
                    .Result();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void AutoRetry_ExpectStopAfterMaximum()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable });

                int expected = 3;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                var result = HttpCallBuilder.Create(TestUriString).Result();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
            }
        }

        [Test]
        public void WithRetryStatusCode_WithAddedStatusCode_ExpectAutoRetry()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.InternalServerError });

                int expected = 3;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                var result = HttpCallBuilder.Create(TestUriString).Advanced.ConfigureRetries(h => h.WithRetryStatusCode(HttpStatusCode.InternalServerError)).Result();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            }
        }

        [Test]
        public void WithRetryValidator_WithCustomValidator_ExpectValidatorUsed()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.InternalServerError });

                int expected = 3;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                var result = HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRetries(h => 
                        h.WithRetryValidator(r => r.StatusCode == HttpStatusCode.InternalServerError))
                    .Result();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithRetryValidator_WithValidatorIsNull_ExpectException()
        {
            //act
            HttpCallBuilder.Create(TestUriString)
                .Advanced.ConfigureRetries(h =>
                    h.WithRetryValidator(null))
                .Result();
        }

        [Test]
        public void WithCallback_WithCallback_ExpectConfigurationApplied()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable })
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success"})
                    ;
                int expected = 2;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(h => h.WithCallback(ctx =>
                    {
                        if (ctx.CurrentRetryCount >= 1)
                            ctx.ShouldRetry = false;
                    }))
                    .Result();

                Assert.AreEqual(expected, actual);

            }
        }

        [Test]
        public void AutoRetry_WithRetryHeaderDelta_ExpectHeaderHonored()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable }.AddHeader("Retry-After", "3"))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success"});

                var expected = TimeSpan.FromSeconds(3);
                var actual = TimeSpan.Zero;

                //act
                var result = HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(h => h.WithCallback(ctx =>
                        { actual = ctx.RetryAfter; }))
                    .Result().ReadContents();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void AutoRetry_WithRetryHeaderDate_ExpectHeaderHonored()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable }
                        .AddHeader("Retry-After", DateTime.UtcNow.AddSeconds(2).ToString("r")))
                    .AddResponse(new LocalWebServerResponseInfo{ Body = "Success"});

                var actual = TimeSpan.Zero;

                //act
                var result = HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(h => h.WithCallback(ctx =>
                    { actual = ctx.RetryAfter; }))
                    .Result().ReadContents();

                Assert.Greater(actual.TotalMilliseconds, 1);
                Assert.LessOrEqual(actual.TotalMilliseconds, 2500);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void WithAutoRetry_WithRetryHeaderGreaterThanMax_ExpectRetryNotCalled()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.ServiceUnavailable }.AddHeader("Retry-After", "6000"))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success"});

                var expected = TimeSpan.FromMilliseconds(2);
                var actual = TimeSpan.Zero;

                //act
                var result = HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RetryHandler>(
                        h =>
                            {
                                h.WithAutoRetry(1, TimeSpan.FromMilliseconds(2));
                                h.WithCallback(ctx =>
                                {
                                    actual = ctx.RetryAfter;
                                });
                            })
                    .Result();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(HttpStatusCode.ServiceUnavailable, result.StatusCode);
            }
        }
    }
}