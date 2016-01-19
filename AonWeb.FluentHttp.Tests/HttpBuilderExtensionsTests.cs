using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests
{
    [Collection("LocalWebServer Tests")]
    public class HttpBuilderExtensionsTests
    {
        private const string MockUriString = "http://testsite.com";
        private readonly ITestOutputHelper _logger;

        public HttpBuilderExtensionsTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Cache.DeleteAll();
        }

        private static IMockHttpBuilder CreateBuilder()
        {
            var builder = new MockHttpBuilderFactory().Create();

            builder.Advanced.WithCaching(false);

            return builder;
        }

        #region Http Methods

        [Fact]
        public async Task Defaults_WhenNotMethodSpecified_ExpectGetMethod()
        {
            //arrange
            var expected = "GET";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task AsGet_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "GET";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsGet().ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task AsPut_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PUT";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsPut().ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task AsPost_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "POST";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsPost().ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task AsDelete_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "DELETE";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsDelete().ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task AsPatch_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PATCH";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsPatch().ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task AsHead_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "HEAD";

            var builder = CreateBuilder().WithUri(MockUriString).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsHead().ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        #endregion

        #region Content (string)

        [Fact]
        public async Task WithContent_AsString_ExpectRequestContainsContent()
        {
            //arrange
            var uri = MockUriString;
            var expected = "Content";
            var builder = CreateBuilder().WithUri(uri).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.AsPost().WithContent(expected).ResultAsync();

            //assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithContent_AsStringWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;


            var builder = CreateBuilder().WithUri(uri).Advanced;

            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(content).ResultAsync();

            //assert
            actualAcceptHeader.ShouldBe(expectedEncoding.WebName);
        }

        [Fact]
        public async Task WithContent_AsStringWithNoMediaTypeSpecified_ExpectJsonUsed()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = CreateBuilder().WithUri(uri).Advanced;

            string actualContentType = null;
            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualContentType = ctx.Request.Content.Headers.ContentType.MediaType;
                actualAcceptHeader = ctx.Request.Headers.Accept.First().MediaType;
            });

            //act
            await builder.AsPost().WithContent(content).ResultAsync();

            //assert
            actualContentType.ShouldBe("application/json");
            actualAcceptHeader.ShouldBe(expectedMediaType);
        }

        [Fact]
        public async Task WithContent_AsStringWithEncodingSpecified_ExpectEncodingUsedAndAcceptCharsetAdded()
        {
            //arrange
            var uri = MockUriString;
            var expectedContent = "Content";
            var expectedEncoding = Encoding.ASCII;


            var builder = CreateBuilder().WithUri(uri).Advanced;
            string actualContent = null;
            string actualAcceptHeader = null;

            builder.OnSending(async ctx =>
            {
                actualContent = await ctx.Request.Content.ReadAsStringAsync();
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(expectedContent, expectedEncoding).ResultAsync();

            //assert
            actualContent.ShouldBe(expectedContent);
            actualAcceptHeader.ShouldBe(expectedEncoding.WebName);
        }

        [Fact]
        public async Task WithContent_AsStringWithMediaTypeSpecified_ExpectMediaTypeUsedAndAcceptHeaderSet()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = CreateBuilder().WithUri(uri).Advanced;

            string actualContentType = null;
            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualContentType = ctx.Request.Content.Headers.ContentType.MediaType;
                actualAcceptHeader = ctx.Request.Headers.Accept.First().MediaType;
            });

            //act
            await builder.AsPost().WithContent(content, Encoding.UTF8, expectedMediaType).ResultAsync();

            //assert
            actualContentType.ShouldBe("application/json");
            actualAcceptHeader.ShouldBe(expectedMediaType);
        }

        #endregion

        #region Content (func)

        [Fact]
        public async Task WithContent_AsFactoryFunc_ExpectRequestContainsContent()
        {
            //arrange
            var uri = MockUriString;
            var expected = "Content";
            var builder = CreateBuilder().WithUri(uri).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.AsPost().WithContent(() => expected).ResultAsync();

            //assert
            actual.ShouldBe(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WithContent_AsIsNullOrEmptyInFactory_ExpectRequestContentNull(string content)
        {
            //arrange
            var uri = MockUriString;

            var builder = CreateBuilder().WithUri(uri).Advanced;
            bool actual = false;
            builder.OnSending(ctx => actual = ctx.Request.Content == null);

            //act
            await builder.AsPost().WithContent(() => content).ResultAsync();

            //assert
            actual.ShouldBeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WithContent_AsIsNullOrEmpty_ExpectRequestContentNull(string content)
        {
            //arrange
            var uri = MockUriString;

            var builder = CreateBuilder().WithUri(uri).Advanced;
            bool actual = false;
            builder.OnSending(ctx => actual = ctx.Request.Content == null);

            //act
            await builder.AsPost().WithContent(content).ResultAsync();

            //assert
            actual.ShouldBeTrue();
        }

        [Fact]
        public void WithContent_AsFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = MockUriString;
            Should.Throw<ArgumentNullException>(() => 
                CreateBuilder().WithUri(uri).Advanced.AsPost()
                    .WithContent((Func<string>) null));
        }

        [Fact]
        public async Task WithContent_AsFactoryFuncWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;


            var builder = CreateBuilder().WithUri(uri).Advanced;

            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(() => content).ResultAsync();

            //assert
            actualAcceptHeader.ShouldBe(expectedEncoding.WebName);
        }

        [Fact]
        public async Task WithContent_AsFactoryFuncWithNoMediaTypeSpecified_ExpectJsonUsed()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = CreateBuilder().WithUri(uri).Advanced;

            string actualContentType = null;
            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualContentType = ctx.Request.Content.Headers.ContentType.MediaType;
                actualAcceptHeader = ctx.Request.Headers.Accept.First().MediaType;
            });

            //act
            await builder.AsPost().WithContent(() => content).ResultAsync();

            //assert
            actualContentType.ShouldBe("application/json");
            actualAcceptHeader.ShouldBe(expectedMediaType);
        }

        [Fact]
        public async Task WithContent_AsFactoryFuncWithEncodingSpecified_ExpectEncodingUsedAndAcceptCharsetAdded()
        {
            //arrange
            var uri = MockUriString;
            var expectedContent = "Content";
            var expectedEncoding = Encoding.ASCII;


            var builder = CreateBuilder().WithUri(uri).Advanced;
            string actualContent = null;
            string actualAcceptHeader = null;

            builder.OnSending(async ctx =>
            {
                actualContent = await ctx.Request.Content.ReadAsStringAsync();
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(() => expectedContent, expectedEncoding).ResultAsync();

            //assert
            actualContent.ShouldBe(expectedContent);
            actualAcceptHeader.ShouldBe(expectedEncoding.WebName);
        }

        [Fact]
        public async Task WithContent_AsFactoryFuncWithMediaTypeSpeficied_ExpectMediaTypeUsedAndAcceptHeaderSet()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = CreateBuilder().WithUri(uri).Advanced;

            string actualContentType = null;
            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualContentType = ctx.Request.Content.Headers.ContentType.MediaType;
                actualAcceptHeader = ctx.Request.Headers.Accept.First().MediaType;
            });

            //act
            await builder.WithContent(() => content, Encoding.UTF8, expectedMediaType).AsPost().ResultAsync();

            //assert
            actualContentType.ShouldBe("application/json");
            actualAcceptHeader.ShouldBe(expectedMediaType);
        }

        [Fact]
        public async Task WithContent_AsHttpContentFactoryFunc_ExpectRequestContainsBody()
        {
            //arrange
            var uri = MockUriString;

            var content = new StringContent("Content");
            var builder = CreateBuilder().WithUri(uri).Advanced;
            string actual = null;
            builder.OnSending(async ctx =>
            {
                actual = await ctx.Request.Content.ReadAsStringAsync();
            });

            //act
            await builder.AsPost().WithContent(ctx => content).ResultAsync();

            //assert
            actual.ShouldBe("Content");
        }

        [Fact]
        public void WithContent_AsHttpContentFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = MockUriString;

            Should.Throw<ArgumentNullException>(() => 
                CreateBuilder().WithUri(uri).Advanced.AsPost()
                    .WithContent((Func<IHttpBuilderContext, Task<HttpContent>>)null));
        }

        #endregion

        [Fact]
        public async Task CancelRequest_WhenCalled_ExpectCancelledBeforeCompletion()
        {
            //arrange
            var uri = MockUriString;

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {

                var delay = 500;
                var builder = CreateBuilder().WithUri(uri).Advanced.WithSuppressCancellationExceptions();
                server.WithRequestInspector(r => Task.Delay(delay));

                // act
                var watch = new Stopwatch();
                watch.Start();
                var task = builder.ResultAsync();

                builder.CancelRequest();

                var result = await task;

                // assert
                watch.ElapsedMilliseconds.ShouldBeLessThan(delay);
            }
        }
    }
}