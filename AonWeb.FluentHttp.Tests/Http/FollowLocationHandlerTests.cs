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
                server.InspectRequest(
                    r =>
                        {
                            actualUrl = r.Url.ToString();
                            actualMethod = r.HttpMethod;
                        });

                //act
                var response = await HttpCallBuilder.Create(TestUriString).ResultAsync();

                var result = response.ReadContents();

                Assert.AreEqual(expected, actualUrl);
                Assert.AreEqual("GET", actualMethod);
                Assert.AreEqual("Success", result);

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
                server.InspectRequest(
                    r =>
                    {
                        actualUrl = r.Url.ToString();
                        actualMethod = r.HttpMethod;
                    });

                //act
                var result = await TypedHttpCallBuilder.Create(TestUriString).ResultAsync<string>();

                Assert.AreEqual(expected, actualUrl);
                Assert.AreEqual("GET", actualMethod);
                Assert.AreEqual("Success", result);

            }
        }
    }
}