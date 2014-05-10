using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderMethodTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion


        #region Create

        [Test]
        public void Create_ReturnsBuilder()
        {
            var builder = HttpCallBuilder.Create();

            Assert.NotNull(builder);
        }

        [Test]
        public async Task Create_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(uri, actual);
            }
        }
        [Test]
        public async Task Create_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(uri, actual);
            }
        }


        #endregion

        #region WithUri

        [Test]
        public async Task WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create().WithUri(uri);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(uri, actual);
            }
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void WithUri_WhenInvalidString_ExpectException()
        {
            //arrange
            var uri = "blah blah";
            var builder = HttpCallBuilder.Create();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenNullString_ExpectException()
        {
            //arrange
            string uri = null;
            var builder = HttpCallBuilder.Create();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenEmptyString_ExpectException()
        {
            //arrange
            var uri = string.Empty;
            var builder = HttpCallBuilder.Create();

            //act
            builder.WithUri(uri);
        }

        [Test]
        public async Task WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create().WithUri(uri);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(uri, actual);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithUri_WhenNullUri_ExpectException()
        {
 
                Uri uri = null;
                HttpCallBuilder.Create().WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task WithUri_WhenNeverSet_ExpectException()
        {
            await HttpCallBuilder.Create().ResultAsync();
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void WithUri_WhenUriIsNotAbsolute_ExpectException()
        {
            //arrange
            var uri = "somedomain.com/path";
            var builder = HttpCallBuilder.Create();

            //act
            builder.WithUri(uri);
        }

        [Test]
        public async Task WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = "http://yahoo.com";
            var uri2 = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri2))
            {
                var builder = HttpCallBuilder.Create().WithUri(uri1).WithUri(uri2);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(uri2, actual);
            }
        }

        #endregion

        #region WithQuerystring

        [Test]
        public async Task WithQueryString_WhenValidValues_ExpectResultUsesUriAndQuerystring()
        {
            //arrange
            var uri = new Uri(TestUriString);
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create().WithUri(uri);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(uri, actual);
            }
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
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString(key, value);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WithQueryString_WithMultipleKeys_ExpectResultQuerystring()
        {
            var uri = new Uri(TestUriString + "?q1=1");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var values = new NameValueCollection { { "q1", "2" }, { "q2", "1 and 2" }, { "q3", "3" } };
                var expected = new Uri(TestUriString + "?q1=2&q2=1+and+2&q3=3");

                var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString(values);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }
        [Test]
        public async Task WithQueryString_WithNullValues_ExpectSameUri()
        {
            var expected = new Uri(TestUriString);
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create().WithUri(expected).WithQueryString(null);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WithQueryString_WithNoValues_ExpectSameUri()
        {
            var expected = new Uri(TestUriString);
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create().WithUri(expected).WithQueryString(new NameValueCollection());
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();
                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WithQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(TestUriString + "?q=1");
            var expected = new Uri(TestUriString + "?q=2");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder =
                    HttpCallBuilder.Create().WithUri(uri).WithQueryString(new NameValueCollection { { "q", "2" } });
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                var result = await builder.ResultAsync();
                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WithQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(TestUriString + "?q=1");
            var expected = new Uri(TestUriString + "?q=2");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString("q", "2");
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();
                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WithQueryString_WithUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(TestUriString + "?q=2");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create().WithQueryString("q2", "1").WithUri(expected);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();
                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WithQueryString_WithBaseUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(TestUriString + "?q=2");
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create().WithQueryString("q2", "1").WithBaseUri(expected);
                string actual = null;
                server.InspectRequest(r => actual = r.Url.OriginalString);

                // act
                await builder.ResultAsync();
                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #region Http Methods

        [Test]
        public async Task Defaults_WhenNotMethodSpecified_ExpectGetMethod()
        {
            //arrange
            var expected = "GET";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString);
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task AsGet_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "GET";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString).AsGet();
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task AsPut_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PUT";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString).AsPut();
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task AsPost_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "POST";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString).AsPost();
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task AsDelete_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "DELETE";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString).AsDelete();
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task AsPatch_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PATCH";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString).AsPatch();
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task AsHead_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "HEAD";
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var builder = HttpCallBuilder.Create(TestUriString).AsHead();
                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                // act
                await builder.ResultAsync();

                // assert
                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #region Content (string)

        [Test]
        public async Task WithContent_AsString_ExpectRequestContainsBody([Values("Content", "", null)]string content)
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(content);
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual(content ?? string.Empty, actual);
            }
        }

        [Test]
        public async Task WithContent_AsStringWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(content);
                
                Encoding actualContentEncoding = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContentEncoding = r.ContentEncoding;
                    actualAcceptHeader = r.Headers["Accept-Charset"];
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual(expectedEncoding, actualContentEncoding, "Content Encoding does not match");
                Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept encoding does not match");
            }
        }

        [Test]
        public async Task WithContent_AsStringWithNoMediaTypeSpecified_ExpectJsonUsed()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(content);

                string actualContentType = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContentType = r.ContentType;
                    actualAcceptHeader = r.AcceptTypes.FirstOrDefault();
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual("application/json; charset=utf-8", actualContentType, "Content-Type do not match");
                Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept do not match");
            }
        }

        [Test]
        public async Task WithContent_AsStringWithEncodingSpecified_ExpectEncodingUsedAndAcceptCharsetAdded()
        {
            //arrange
            var uri = TestUriString;
            var expectedContent = "Content";
            var expectedEncoding = Encoding.ASCII;

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(expectedContent, expectedEncoding);
                string actualContent = null;
                Encoding actualContentEncoding = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContent = r.Body;
                    actualContentEncoding = r.ContentEncoding;
                    actualAcceptHeader = r.Headers["Accept-Charset"];
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual(expectedContent, actualContent, "Content does not match");
                Assert.AreEqual(expectedEncoding, actualContentEncoding, "Content Encoding does not match");
                Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept encoding does not match");
            }
        }

        [Test]
        public async Task WithContent_AsStringWithMediaTypeSpecified_ExpectMediaTypeUsedAndAcceptHeaderSet()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(content, Encoding.UTF8, expectedMediaType);

                string actualContentType = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContentType = r.ContentType;
                    actualAcceptHeader = r.AcceptTypes.FirstOrDefault();
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual("application/json; charset=utf-8", actualContentType, "Content-Type do not match");
                Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept do not match");
            }
        }

        #endregion

        #region Content (func)

        [Test]
        public async Task WithContent_AsFactoryFunc_ExpectRequestContainsBody([Values("Content", "", null)]string content)
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(() => content);
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual(content ?? string.Empty, actual);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithContent_AsFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = TestUriString;
            
            HttpCallBuilder.Create(uri).AsPost().WithContent((Func<string>)null);
                
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithNoEncodingSpecified_ExpectUtf8Used()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedEncoding = Encoding.UTF8;

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(() => content);

                Encoding actualContentEncoding = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContentEncoding = r.ContentEncoding;
                    actualAcceptHeader = r.Headers["Accept-Charset"];
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual(expectedEncoding, actualContentEncoding, "Content Encoding does not match.");
                Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept Charset does not match");
            }
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithNoMediaTypeSpecified_ExpectJsonUsed()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(() => content);

                string actualContentType = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContentType = r.ContentType;
                    actualAcceptHeader = r.AcceptTypes.FirstOrDefault();
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual("application/json; charset=utf-8", actualContentType, "Content Type does not match");
                Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept header does not match");
            }
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithEncodingSpecified_ExpectEncodingUsedAndAcceptCharsetAdded()
        {
            //arrange
            var uri = TestUriString;
            var expectedContent = "Content";
            var expectedEncoding = Encoding.ASCII;

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(() => expectedContent, expectedEncoding);
                string actualContent = null;
                Encoding actualContentEncoding = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContent = r.Body;
                    actualContentEncoding = r.ContentEncoding;
                    actualAcceptHeader = r.Headers["Accept-Charset"];
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual(expectedContent, actualContent, "Content does not match");
                Assert.AreEqual(expectedEncoding, actualContentEncoding, "Content Encoding does not match");
                Assert.AreEqual(expectedEncoding.WebName, actualAcceptHeader, "Accept encoding does not match");
            }
        }

        [Test]
        public async Task WithContent_AsFactoryFuncWithMediaTypeSpeficied_ExpectMediaTypeUsedAndAcceptHeaderSet()
        {
            //arrange
            var uri = TestUriString;
            var content = "Content";
            var expectedMediaType = "application/json";

            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(() => content, Encoding.UTF8, expectedMediaType);

                string actualContentType = null;
                string actualAcceptHeader = null;

                server.InspectRequest(r =>
                {
                    actualContentType = r.ContentType;
                    actualAcceptHeader = r.AcceptTypes.FirstOrDefault();
                });

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual("application/json; charset=utf-8", actualContentType, "Content-Type do not match");
                Assert.AreEqual(expectedMediaType, actualAcceptHeader, "Accept do not match");
            }
        }

        [Test]
        public async Task WithContent_AsHttpContentFactoryFunc_ExpectRequestContainsBody()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var content = new StringContent("Content");
                var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(() => content);
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                await builder.ResultAsync();

                //assert
                Assert.AreEqual("Content", actual);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithContent_AsHttpContentFactoryFuncWithNullValue_ExpectException()
        {
            //arrange
            var uri = TestUriString;
            
            HttpCallBuilder.Create(uri).AsPost().WithContent((Func<HttpContent>)null);
                
        }

        #endregion

        [Test]
        public void CancelRequest_WhenCalled_ExpectCancelledBeforeCompletion()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var delay = 500;
                var builder = HttpCallBuilder.Create().WithUri(uri);
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
