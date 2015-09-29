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
    public class HttpBuilderExtensionsTests
    {
        private const string MockUriString = "http://testsite.com";
        private readonly ITestOutputHelper _logger;

        public HttpBuilderExtensionsTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Caching.Enabled = false;
            Cache.Clear();
        }

        #region WithUri

        [Fact]
        public async Task WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.ResultAsync();

            // assert
            uri.ShouldBe(actual);
        }

        [Fact]
        public void WithUri_WhenInvalidString_ExpectException()
        {
            //arrange
            var uri = "blah blah";
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<UriFormatException>(() => builder.WithUri(uri));
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public void WithUri_WhenNullString_ExpectException()
        {
            //arrange
            string uri = null;
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<ArgumentException>(() => builder.WithUri(uri));
        }

        [Fact]
        public void WithUri_WhenEmptyString_ExpectException()
        {
            //arrange
            var uri = string.Empty;
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<ArgumentException>(() => builder.WithUri(uri));
        }

        [Fact]
        public async Task WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.ResultAsync();

            // assert
            uri.ShouldBe(actual);
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public void WithUri_WhenNullUri_ExpectException()
        {

            Uri uri = null;
            Should.Throw<ArgumentNullException>(() => new MockHttpBuilderFactory().Create().WithUri(uri));
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WithUri_WhenNeverSet_ExpectException()
        {
            await Should.ThrowAsync<InvalidOperationException>(new MockHttpBuilderFactory().Create().ResultAsync());
        }

        [Fact]
        public void WithUri_WhenUriIsNotAbsolute_ExpectException()
        {
            //arrange
            var uri = "somedomain.com/path";
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<UriFormatException>(() => builder.WithUri(uri));
        }

        [Fact]
        public async Task WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = new Uri("http://yahoo.com");
            var uri2 = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri1).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithUri(uri2).ResultAsync();

            // assert
            uri2.ShouldBe(actual);

        }

        #endregion

        #region WithQuerystring

        [Fact]
        public async Task WithQueryString_WhenValidValues_ExpectResultUsesUriAndQuerystring()
        {
            //arrange
            var uri = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.ResultAsync();

            // assert
            uri.ShouldBe(actual);
        }

        [Theory]
        [InlineData("q", "1", MockUriString + "?q=1")]
        [InlineData("q", "1 and 2", MockUriString + "?q=1%20and%202")]
        [InlineData("q", null, MockUriString + "?q=")]
        [InlineData("q", "", MockUriString + "?q=")]
        [InlineData(null, "1", MockUriString)]
        public async Task WithQueryString_WhenValidValues_ExpectResultQuerystring(string key, string value, string expectedUriString)
        {
            //arrange
            var uri = new Uri(MockUriString);
            var expected = new Uri(expectedUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(key, value).ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WithMultipleKeys_ExpectResultQuerystring()
        {
            var uri = new Uri(MockUriString + "?q1=1");

            var values = new Dictionary<string, string> { { "q1", "2" }, { "q2", "1 and 2" }, { "q3", "3" } };
            var expected = new Uri(MockUriString + "?q1=2&q2=1%20and%202&q3=3");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(values).ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }
        [Fact]
        public async Task WithQueryString_WithNullValues_ExpectSameUri()
        {
            var expected = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(expected).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(null).ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WithNoValues_ExpectSameUri()
        {
            var expected = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(expected).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(new Dictionary<string, string>()).ResultAsync();
            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            var result = await builder.WithQueryString(new Dictionary<string, string> { { "q", "2" } }).ResultAsync();

            // assert
            actual.ShouldBe(expected);

        }

        [Fact]
        public async Task WithQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString("q", "2").ResultAsync();
            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithAppendQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=1&q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            var result = await builder.WithAppendQueryString(new Dictionary<string, string> { { "q", "2" } }).ResultAsync();

            // assert
            actual.ShouldBe(expected);

        }

        [Fact]
        public async Task WithAppendQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=1&q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithAppendQueryString("q", "2").ResultAsync();
            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WithUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString("q2", "1").WithUri(expected).ResultAsync();
            // assert
            actual.ShouldBe(expected);

        }

        [Fact]
        public async Task WithQueryString_WithBaseUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString("q2", "1").WithBaseUri(expected).ResultAsync();
            // assert
            actual.ShouldBe(expected);

        }

        #endregion

        #region Http Methods

        [Fact]
        public async Task Defaults_WhenNotMethodSpecified_ExpectGetMethod()
        {
            //arrange
            var expected = "GET";

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(MockUriString).Advanced;
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
            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;

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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;

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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;

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
            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
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

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            bool actual = false;
            builder.OnSending(ctx => actual = ctx.Request.Content == null);

            //act
            await builder.AsPost().WithContent(content).ResultAsync();

            //assert
            actual.ShouldBeTrue();
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public void WithContent_AsFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = MockUriString;
            Should.Throw<ArgumentNullException>(() => 
                new MockHttpBuilderFactory().Create().WithUri(uri).Advanced.AsPost()
                    .WithContent((Func<string>) null));
        }

        [Fact]
        public async Task WithContent_AsFactoryFuncWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = MockUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;

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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;

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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
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


            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;

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
            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.AsPost().WithContent(ctx => content).ResultAsync();

            //assert
            actual.ShouldBe("Content");
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public void WithContent_AsHttpContentFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = MockUriString;

            Should.Throw<ArgumentNullException>(() => 
                new MockHttpBuilderFactory().Create().WithUri(uri).Advanced.AsPost()
                    .WithContent((Func<IHttpBuilderContext, HttpContent>)null));
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
                var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced.WithSuppressCancellationExceptions();
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