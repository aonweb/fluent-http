using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class RedirectionHandlerTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion

        [Test]
        [TestCase(HttpStatusCode.Found)]
        [TestCase(HttpStatusCode.Redirect)]
        [TestCase(HttpStatusCode.MovedPermanently)]
        public void AutoRedirect_WhenCallRedirects_ExpectRedirectOnByDefaultAndLocationFollowed(HttpStatusCode statusCode)
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = statusCode }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = HttpCallBuilder.Create(TestUriString).ResultAsync().Result.ReadContents();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);

            }
        }

        [Test]
        public void AutoRedirect_WhenCallRedirects_ExpectContentSent()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                var actual = new List<string>();
                server.InspectRequest(r => actual.Add(r.Body));

                //act
                var result = HttpCallBuilder.Create(TestUriString).AsPost().WithContent("Content").ResultAsync().Result.ReadContents();

                Assert.AreEqual(2, actual.Count);
                Assert.AreEqual("Content", actual[1]);
            }
        }

        [Test]
        [TestCase(TestUriString, "/redirect", ExpectedResult = TestUriString + "redirect")]
        [TestCase(TestUriString + "post", "/redirect", ExpectedResult = TestUriString + "redirect")]
        [TestCase(TestUriString + "post", "redirect", ExpectedResult = TestUriString + "post/redirect")]
        public async Task<string> AutoRedirect_WhenCallRedirectsWithRelativePath_ExpectPathHandled(string uri, string path)
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", path))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var response = await HttpCallBuilder.Create(uri).AsPost().WithContent("Content").ResultAsync();
                var result = await response.Content.ReadAsStringAsync();

                Assert.AreEqual("Success", result);

                return actual;
            }
        }

        [Test]
        public async Task AutoRedirect_WhenNotEnabledCallRedirects_ExpectNotFollowed()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", redirectUrl));

                var calledBack = false;

                //act
                await HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureRedirect(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                    .ResultAsync();

                Assert.IsFalse(calledBack);

            }
        }

        [Test]
        public async Task AutoRedirect_WhenEnabledAndCallRedirects_ExpectRedirect()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = await HttpCallBuilder.Create(TestUriString).Advanced.ConfigureRedirect(h => h.WithAutoRedirect()).ResultAsync().ReadContentsAsync();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public async Task WithCallback_WhenAction_ExpectConfigurationApplied()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", redirectUrl))
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", redirectUrl))
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", redirectUrl))
                    .AddResponse(new LocalWebServerResponseInfo());

                int expected = 2;
                int actual = 0;
                server.InspectRequest(r => actual++);

                //act
                await HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RedirectHandler>(h => h.WithCallback(ctx =>
                    {
                        if (ctx.CurrentRedirectionCount >= 1)
                            ctx.ShouldRedirect = false;
                    }))
                    .ResultAsync();

                Assert.AreEqual(expected, actual);

            }
        }

        [Test]
        public async Task AutoRedirect_WithNoLocationHeader_ExpectNoRedirect()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Created })
                    .AddResponse(new LocalWebServerResponseInfo());

                //act
                var result = await HttpCallBuilder.Create(TestUriString).ResultAsync();

                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            }
        }

        [Test]
        public async Task WithRedirectStatusCode_WithAddedStatusCode_ExpectAutoRetry()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.MultipleChoices }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = await HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h => h.WithRedirectStatusCode(HttpStatusCode.MultipleChoices)).ResultAsync().ReadContentsAsync();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public async Task WithRedirectValidator_WithCustomValidator_ExpectValidatorUsed()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.MultipleChoices }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = await HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h =>
                        h.WithRedirectValidator(r => r.Result.StatusCode == HttpStatusCode.MultipleChoices))
                    .ResultAsync().ReadContentsAsync();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void WithRedirectValidator_WithValidatorIsNull_ExpectException()
        {
            //act
            Assert.Throws<ArgumentNullException>(async () => await HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h =>
                        h.WithRedirectValidator(null))
                    .ResultAsync());
        }

        [Test]
        public void WithAutoRedirect_WithMaxRedirect_ExpectException()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }
                        .AddHeader("Location", redirectUrl));

                //act
                Assert.Throws<MaximumAutoRedirectsException>(async () => await HttpCallBuilder.Create(TestUriString).Advanced.ConfigureRedirect(h => h.WithAutoRedirect(1)).ResultAsync());
            }
        }
    }
}