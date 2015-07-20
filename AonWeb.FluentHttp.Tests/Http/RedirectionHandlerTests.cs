using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Serialization;
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

        #region Test Classes

        public static string TestResultString = @"{""StringProperty"":""TestString"",""IntProperty"":2,""BoolProperty"":true,""DateOffsetProperty"":""2000-01-01T00:00:00-05:00"",""DateProperty"":""2000-01-01T00:00:00""}";
        public static TestResult TestResultValue = new TestResult();

        public class TestResult : IEquatable<TestResult>
        {
            public TestResult()
            {
                StringProperty = "TestString";
                IntProperty = 2;
                BoolProperty = true;
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5));
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0);
            }

            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public bool BoolProperty { get; set; }
            public DateTimeOffset DateOffsetProperty { get; set; }
            public DateTime DateProperty { get; set; }

            #region Equality

            public bool Equals(TestResult other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return DateProperty.Equals(other.DateProperty)
                    && DateOffsetProperty.Equals(other.DateOffsetProperty)
                    && BoolProperty.Equals(other.BoolProperty)
                    && IntProperty == other.IntProperty
                    && string.Equals(StringProperty, other.StringProperty);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((TestResult)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = DateProperty.GetHashCode();
                    hashCode = (hashCode * 397) ^ DateOffsetProperty.GetHashCode();
                    hashCode = (hashCode * 397) ^ BoolProperty.GetHashCode();
                    hashCode = (hashCode * 397) ^ IntProperty;
                    hashCode = (hashCode * 397) ^ StringProperty.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(TestResult left, TestResult right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestResult left, TestResult right)
            {
                return !Equals(left, right);
            }

            #endregion
        }

        #endregion

        [Test]
        [TestCase(HttpStatusCode.Found)]
        [TestCase(HttpStatusCode.Redirect)]
        [TestCase(HttpStatusCode.MovedPermanently)]
        public async Task AutoRedirect_WhenCallRedirects_ExpectRedirectOnByDefaultAndLocationFollowed(HttpStatusCode statusCode)
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
                var response = await HttpCallBuilder.Create(TestUriString).ResultAsync();
                var result = await response.ReadContentsAsync();

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
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task WithRedirectValidator_WithValidatorIsNull_ExpectException()
        {
                //act
                await HttpCallBuilder.Create(TestUriString)
                    .Advanced.ConfigureRedirect(h =>
                        h.WithRedirectValidator(null))
                    .ResultAsync();

            Assert.Fail();
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

        [Test]
        [TestCase(HttpStatusCode.Found)]
        [TestCase(HttpStatusCode.Redirect)]
        [TestCase(HttpStatusCode.MovedPermanently)]
        public async Task AutoRedirectOnTypedCallBuilder_WhenCallRedirects_ExpectRedirectOnByDefaultAndLocationFollowed(HttpStatusCode statusCode)
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = statusCode }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        StatusCode = HttpStatusCode.OK,
                        Body = TestResultString
                    });

                string actual = null;
                server.InspectRequest(r => actual = r.Url.ToString());

                //act
                var result = await TypedHttpCallBuilder.Create(TestUriString).ResultAsync<TestResult>();

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(TestResultValue, result);

            }
        }

        [Test]
        public void AutoRedirectOnTypedCallBuilder_WhenNotEnabledCallRedirects_ExpectNotFollowedAndExceptionThrown()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Redirect }.AddHeader("Location", redirectUrl));

                var calledBack = false;

                //act
                Assert.Catch<HttpErrorException<string>>(async () => await TypedHttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureRedirect(h => h.WithAutoRedirect(false).WithCallback(ctx => calledBack = true))
                    .SendAsync());

                Assert.IsFalse(calledBack);
            }
        }
    }
}