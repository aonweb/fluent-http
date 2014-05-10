using System;
using System.Net;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class RedirectionHandlerTests
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
        [TestCase(HttpStatusCode.Redirect)]
        [TestCase(HttpStatusCode.MovedPermanently)]
        [TestCase(HttpStatusCode.Created)]
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
                var result = HttpCallBuilder.Create(TestUriString).Result().ReadContents();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);

            }
        }

        [Test]
        public void AutoRedirect_WhenNotEnabledCallRedirects_ExpectNotFollowed()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", redirectUrl));

                int expected = 2;
                var calledBack = false;

                //act
                HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureRedirect(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                    .Result();

                Assert.IsFalse(calledBack);

            }
        }

        [Test]
        public void AutoRedirect_WhenEnabledAndCallRedirects_ExpectRedirect()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h => h.WithAutoRedirect()).Result().ReadContents();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void WithCallback_WhenAction_ExpectConfigurationApplied()
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
                HttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHandler<RedirectHandler>(h => h.WithCallback(ctx =>
                    {
                        if (ctx.CurrentRedirectionCount >= 1)
                            ctx.ShouldRedirect = false;
                    }))
                    .Result();

                Assert.AreEqual(expected, actual);

            }
        }

        [Test]
        public void AutoRedirect_WithNoLocationHeader_ExpectNoRedirect()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Created })
                    .AddResponse(new LocalWebServerResponseInfo());

                //act
                var result = HttpCallBuilder.Create(TestUriString).Result();

                Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            }
        }

        [Test]
        public void WithRedirectStatusCode_WithAddedStatusCode_ExpectAutoRetry()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.MultipleChoices }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h => h.WithRedirectStatusCode(HttpStatusCode.MultipleChoices)).Result().ReadContents();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        public void WithRedirectValidator_WithCustomValidator_ExpectValidatorUsed()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.MultipleChoices }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h =>
                        h.WithRedirectValidator(r => r.StatusCode == HttpStatusCode.MultipleChoices))
                    .Result().ReadContents();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual("Success", result);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithRedirectValidator_WithValidatorIsNull_ExpectException()
        {
                //act
                HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h =>
                        h.WithRedirectValidator(null))
                    .Result();
        }

        [Test]
        [ExpectedException(typeof(MaximumAutoRedirectsException ))]
        public async Task WithAutoRedirect_WithMaxRedirect_ExpectException()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }
                        .AddHeader("Location", redirectUrl));

                //act
                await HttpCallBuilder.Create(TestUriString).Advanced.ConfigureRedirect(h => h.WithAutoRedirect(1)).ResultAsync();

            }
        }
    }
}