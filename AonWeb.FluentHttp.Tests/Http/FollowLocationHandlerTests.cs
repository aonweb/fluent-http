using System;
using System.Net;
using System.Text;
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
    public class FollowLocationHandlerTests
    {
        private const string TestUriString = LocalWebServer.DefaultListenerUri;



        #region Test Classes

        public static string TestResultString = @"{""StringProperty"":""TestString"",""IntProperty"":2,""BoolProperty"":true,""DateOffsetProperty"":""2000-01-01T00:00:00-05:00"",""DateProperty"":""2000-01-01T00:00:00""}";
        public static RedirectionHandlerTests.TestResult TestResultValue = new RedirectionHandlerTests.TestResult();

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
                return Equals((RedirectionHandlerTests.TestResult)obj);
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
        public async Task WithCallBuilderCall_WhenStatusIsCreated_ExpectLocationFollowed()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");

            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Created }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Success" });

                string actualUrl = null;
                string actualMethod = null;
                var hasBody = true;
                server.InspectRequest(
                    r =>
                        {
                            actualUrl = r.Url.ToString();
                            actualMethod = r.HttpMethod;
                            hasBody = r.HasEntityBody;
                        });

                //act
                var response = await HttpCallBuilder.Create(TestUriString).AsPost().WithContent("POST CONTENT").ResultAsync();

                var result = response.ReadContents();

                Assert.AreEqual(expected, actualUrl, "Url");
                Assert.AreEqual("GET", actualMethod, "Method");
                Assert.IsFalse(hasBody, "Body");
                Assert.AreEqual("Success", result, "Result");

            }
        }

        [Test]
        public async Task WithTypedCallBuilderCall_WhenStatusIsCreated_ExpectLocationFollowed()
        {
            var expected = Helper.CombineVirtualPaths(TestUriString, "redirect");

            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Created }.AddHeader("Location", expected))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "\"Success\"" });

                string actualUrl = null;
                string actualMethod = null;
                var hasBody = true;
                server.InspectRequest(
                    r =>
                    {
                        actualUrl = r.Url.ToString();
                        actualMethod = r.HttpMethod;
                        hasBody = r.HasEntityBody;
                    });

                //act
                var result = await TypedHttpCallBuilder.Create(TestUriString).AsPost().WithContent("POST CONTENT").ResultAsync<string>();

                Assert.AreEqual(expected, actualUrl, "Url");
                Assert.AreEqual("GET", actualMethod, "Method");
                Assert.IsFalse(hasBody, "Body");
                Assert.AreEqual("Success", result, "Result");
            }
        }

        [Test]
        public async Task WithTypedCallBuilderCall_WhenNotEnabledAndStatusIsCreated_ExpectNotFollowedAndNoException()
        {
            var redirectUrl = Helper.CombineVirtualPaths(TestUriString, "redirect");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { StatusCode = HttpStatusCode.Created }.AddHeader("Location", redirectUrl));

                var calledBack = false;

                //act
                await TypedHttpCallBuilder.Create(TestUriString).Advanced
                    .ConfigureHttpHandler<FollowLocationHandler>(h => h.WithAutoFollow(false).WithCallback(ctx => calledBack = true))
                    .SendAsync();

                Assert.IsFalse(calledBack);
            }
        }
    }
}