using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.Hal;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests
{
    public class EncodingTests
    {
        private readonly ITestOutputHelper _logger;

        public EncodingTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Cache.Clear();
        }

        public void CanHandleUtf8Encoding()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                
            }
        }

        public void CanHandleAsciiEncoding()
        {

        }

        [Fact]
        public void CanHandleGzipEncoding()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var buffer = Encoding.UTF8.GetBytes(TestResource.SerializedDefault1);
                var ms = new MemoryStream();
                using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }

                ms.Position = 0;
                var content = new StreamContent(ms);

                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json") { CharSet = "utf-8"};
                content.Headers.ContentEncoding.Add("gzip");

                server.WithNextResponse(new MockHttpResponseMessage(HttpStatusCode.OK).WithHttpContent(content).WithHeader("Vary", "Accept-Encoding"));

                var result = new HalBuilderFactory().Create()
                    .WithLink(server.ListeningUri).ResultAsync<TestResource>();

                result.ShouldNotBeNull();
            }
        }
    }
}