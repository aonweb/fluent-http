using System.Net;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class FollowLocationHandlerTests
    {
        private const string TestUriString = LocalWebServer.DefaultListenerUri;

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
    }
}