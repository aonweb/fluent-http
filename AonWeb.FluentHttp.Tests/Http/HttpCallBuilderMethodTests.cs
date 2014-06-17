using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderMethodTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion

        #region Create

        [Test]
        public void Create_ReturnsBuilder()
        {
            var builder = TypedHttpCallBuilder.Create();

            Assert.NotNull(builder);
        }

        [Test]
        public async Task Create_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.ResultAsync();

            // assert
            Assert.AreEqual(uri, actual);
        }
        [Test]
        public async Task Create_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.ResultAsync();

            // assert
            Assert.AreEqual(uri, actual);
        }


        #endregion

        #region WithUri

        [Test]
        public async Task WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.ResultAsync();

            // assert
            Assert.AreEqual(uri, actual);
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void WithUri_WhenInvalidString_ExpectException()
        {
            //arrange
            var uri = "blah blah";
            var builder = MockHttpCallBuilder.CreateMock();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenNullString_ExpectException()
        {
            //arrange
            string uri = null;
            var builder = MockHttpCallBuilder.CreateMock();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenEmptyString_ExpectException()
        {
            //arrange
            var uri = string.Empty;
            var builder = MockHttpCallBuilder.CreateMock();

            //act
            builder.WithUri(uri);
        }

        [Test]
        public async Task WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.ResultAsync();

            // assert
            Assert.AreEqual(uri, actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithUri_WhenNullUri_ExpectException()
        {

            Uri uri = null;
            MockHttpCallBuilder.CreateMock().WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WithUri_WhenNeverSet_ExpectException()
        {
            await MockHttpCallBuilder.CreateMock().ResultAsync();
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void WithUri_WhenUriIsNotAbsolute_ExpectException()
        {
            //arrange
            var uri = "somedomain.com/path";
            var builder = MockHttpCallBuilder.CreateMock();

            //act
            builder.WithUri(uri);
        }

        [Test]
        public async Task WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = "http://yahoo.com";
            var uri2 = TestUriString;

            var builder = MockHttpCallBuilder.CreateMock(uri1);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithUri(uri2).ResultAsync();

            // assert
            Assert.AreEqual(uri2, actual);

        }

        #endregion

        #region WithQuerystring

        [Test]
        public async Task WithQueryString_WhenValidValues_ExpectResultUsesUriAndQuerystring()
        {
            //arrange
            var uri = new Uri(TestUriString);

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.ResultAsync();

            // assert
            Assert.AreEqual(uri, actual);
        }

        [Test]
        [TestCase("q", "1", TestUriString + "?q=1")]
        [TestCase("q", "1 and 2", TestUriString + "?q=1+and+2")]
        [TestCase("q", null, TestUriString + "?q=")]
        [TestCase("q", "", TestUriString + "?q=")]
        [TestCase(null, "1", TestUriString)]
        public async Task WithQueryString_WhenValidValues_ExpectResultQuerystring(string key, string value, string expected)
        {
            //arrange
            var uri = new Uri(TestUriString);

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString(key, value).ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WithQueryString_WithMultipleKeys_ExpectResultQuerystring()
        {
            var uri = new Uri(TestUriString + "?q1=1");

            var values = new NameValueCollection { { "q1", "2" }, { "q2", "1 and 2" }, { "q3", "3" } };
            var expected = new Uri(TestUriString + "?q1=2&q2=1+and+2&q3=3");

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString(values).ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public async Task WithQueryString_WithNullValues_ExpectSameUri()
        {
            var expected = new Uri(TestUriString);

            var builder = MockHttpCallBuilder.CreateMock(expected);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString((NameValueCollection)null).ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WithQueryString_WithNoValues_ExpectSameUri()
        {
            var expected = new Uri(TestUriString);

            var builder = MockHttpCallBuilder.CreateMock(expected);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString(new NameValueCollection()).ResultAsync();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WithQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(TestUriString + "?q=1");
            var expected = new Uri(TestUriString + "?q=2");

            var builder =
                MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            var result = await builder.WithQueryString(new NameValueCollection { { "q", "2" } }).ResultAsync();
            // assert
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public async Task WithQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(TestUriString + "?q=1");
            var expected = new Uri(TestUriString + "?q=2");

            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString("q", "2").ResultAsync();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WithQueryString_WithUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(TestUriString + "?q=2");

            var builder = MockHttpCallBuilder.CreateMock();
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString("q2", "1").WithUri(expected).ResultAsync();
            // assert
            Assert.AreEqual(expected, actual);

        }

        [Test]
        public async Task WithQueryString_WithBaseUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(TestUriString + "?q=2");

            var builder = MockHttpCallBuilder.CreateMock();
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri.OriginalString);

            // act
            await builder.WithQueryString("q2", "1").WithBaseUri(expected).ResultAsync();
            // assert
            Assert.AreEqual(expected, actual);

        }

        #endregion

        #region Http Methods

        [Test]
        public async Task Defaults_WhenNotMethodSpecified_ExpectGetMethod()
        {
            //arrange
            var expected = "GET";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsGet_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "GET";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsGet().ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsPut_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PUT";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsPut().ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsPost_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "POST";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsPost().ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsDelete_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "DELETE";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsDelete().ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsPatch_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PATCH";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsPatch().ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task AsHead_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "HEAD";

            var builder = MockHttpCallBuilder.CreateMock(TestUriString);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Method.Method);

            // act
            await builder.AsHead().ResultAsync();

            // assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Content (string)

        [Test]
        public async Task WithContent_AsString_ExpectRequestContainsContent()
        {
            //arrange
            var uri = TestUriString;
            var expected = "Content";
            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.AsPost().WithContent(expected).ResultAsync();

            //assert
            Assert.AreEqual(expected ?? string.Empty, actual);
        }

        [Test]
        public async Task WithContent_AsStringWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;


            var builder = MockHttpCallBuilder.CreateMock(uri);

            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(content).ResultAsync();

            //assert
            Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept encoding does not match");
        }

        [Test]
        public async Task WithContent_AsStringWithNoMediaTypeSpecified_ExpectJsonUsed()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = MockHttpCallBuilder.CreateMock(uri);

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
            Assert.AreEqual("application/json", actualContentType, "Content-Type do not match");
            Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept do not match");
        }

        [Test]
        public async Task WithContent_AsStringWithEncodingSpecified_ExpectEncodingUsedAndAcceptCharsetAdded()
        {
            //arrange
            var uri = TestUriString;
            var expectedContent = "Content";
            var expectedEncoding = Encoding.ASCII;


            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actualContent = null;
            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualContent = ctx.Request.Content.ReadAsStringAsync().Result;
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(expectedContent, expectedEncoding).ResultAsync();

            //assert
            Assert.AreEqual(expectedContent, actualContent, "Content does not match");
            Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept encoding does not match");
        }

        [Test]
        public async Task WithContent_AsStringWithMediaTypeSpecified_ExpectMediaTypeUsedAndAcceptHeaderSet()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = MockHttpCallBuilder.CreateMock(uri);

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
            Assert.AreEqual("application/json", actualContentType, "Content-Type do not match");
            Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept do not match");
        }

        #endregion

        #region Content (func)

        [Test]
        public async Task WithContent_AsFactoryFunc_ExpectRequestContainsContent()
        {
            //arrange
            var uri = TestUriString;
            var expected = "Content";
            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.AsPost().WithContent(() => expected).ResultAsync();

            //assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WithContent_AsIsNullOrEmptyInFactory_ExpectRequestContentNull([Values("", null)]string content)
        {
            //arrange
            var uri = TestUriString;

            var builder = MockHttpCallBuilder.CreateMock(uri);
            bool actual = false;
            builder.OnSending(ctx => actual = ctx.Request.Content == null);

            //act
            await builder.AsPost().WithContent(() => content).ResultAsync();

            //assert
            Assert.IsTrue(actual);
        }

        [Test]
        public async Task WithContent_AsIsNullOrEmpty_ExpectRequestContentNull([Values("", null)]string content)
        {
            //arrange
            var uri = TestUriString;

            var builder = MockHttpCallBuilder.CreateMock(uri);
            bool actual = false;
            builder.OnSending(ctx => actual = ctx.Request.Content == null);

            //act
            await builder.AsPost().WithContent(content).ResultAsync();

            //assert
            Assert.IsTrue(actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithContent_AsFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = TestUriString;

            MockHttpCallBuilder.CreateMock(uri).AsPost().WithContent((Func<string>)null);

        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;


            var builder = MockHttpCallBuilder.CreateMock(uri);

            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(() => content).ResultAsync();

            //assert
            Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept Charset does not match");
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithNoMediaTypeSpecified_ExpectJsonUsed()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = MockHttpCallBuilder.CreateMock(uri);

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
            Assert.AreEqual("application/json", actualContentType, "Content Type does not match");
            Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept header does not match");
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithEncodingSpecified_ExpectEncodingUsedAndAcceptCharsetAdded()
        {
            //arrange
            var uri = TestUriString;
            var expectedContent = "Content";
            var expectedEncoding = Encoding.ASCII;


            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actualContent = null;
            string actualAcceptHeader = null;

            builder.OnSending(ctx =>
            {
                actualContent = ctx.Request.Content.ReadAsStringAsync().Result;
                actualAcceptHeader = ctx.Request.Content.Headers.ContentType.CharSet;
            });

            //act
            await builder.AsPost().WithContent(() => expectedContent, expectedEncoding).ResultAsync();

            //assert
            Assert.AreEqual(expectedContent, actualContent, "Content does not match");
            Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept encoding does not match");
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithMediaTypeSpeficied_ExpectMediaTypeUsedAndAcceptHeaderSet()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";


            var builder = MockHttpCallBuilder.CreateMock(uri);

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
            Assert.AreEqual("application/json", actualContentType, "Content-Type do not match");
            Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept do not match");
        }

        [Test]
        public async Task WithContent_AsHttpContentFactoryFunc_ExpectRequestContainsBody()
        {
            //arrange
            var uri = TestUriString;

            var content = new StringContent("Content");
            var builder = MockHttpCallBuilder.CreateMock(uri);
            string actual = null;
            builder.OnSending(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.AsPost().WithContent(() => content).ResultAsync();

            //assert
            Assert.AreEqual("Content", actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithContent_AsHttpContentFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = TestUriString;

            MockHttpCallBuilder.CreateMock(uri).AsPost().WithContent((Func<HttpContent>)null);

        }

        #endregion

        [Test]
        public void CancelRequest_WhenCalled_ExpectCancelledBeforeCompletion()
        {
            //arrange
            var uri = TestUriString;

            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {

                var delay = 500;
                var builder = MockHttpCallBuilder.CreateMock(uri);
                server.InspectRequest(r => Thread.Sleep(delay));

                // act
                var watch = new Stopwatch();
                watch.Start();
                var task = builder.ResultAsync();

                builder.CancelRequest();

                Task.WaitAll(task);

                // assert
                Assert.Less(watch.ElapsedMilliseconds, delay);
            }
        }
    }
}
