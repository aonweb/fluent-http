using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests
{
    [TestFixture]
    public class Sandbox
    {
        public class TestClass
        {
            public bool TestProperty { get; set; }
        }

        [Test]
        public async Task CanCatchExceptionFromHttpClientSendAsyncWithCustomSerialization()
        {
            var caughtException = false;

            try
            {
                var content = await CreateContent(
                    new TestClass(),
                    "application/json",
                    new JsonMediaTypeFormatter());

                var request = new HttpRequestMessage(HttpMethod.Get, "http://foo.com")
                {
                    Content = content
                };

                await new HttpClient().SendAsync(request);
            }
            catch (Exception)
            {
                caughtException = true;
            }

            Assert.IsTrue(caughtException);
        }

        private static async Task<HttpContent> CreateContent<T>(
            T value,
            string mediaType,
            MediaTypeFormatter formatter)
        {
            var type = typeof(T);
            var header = new MediaTypeHeaderValue(mediaType);

            HttpContent content;
            using (var stream = new MemoryStream())
            {
                await formatter.WriteToStreamAsync(type, value, stream, null, null);

                content = new ByteArrayContent(stream.ToArray());
            }

            formatter.SetDefaultContentHeaders(type, content.Headers, header);

            return content;
        }
    }
}
