using System;
using System.Collections.Specialized;
using System.Net.Http;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderMethodTests
    {
        #region Declarations, Set up, & Tear Down
        
        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        private LocalWebServer _server;

        [SetUp]
        public void Setup()
        {
            _server = LocalWebServer.ListenInBackground(TestUriString);
        }

        [TearDown]
        public void TearDown()
        {
            if (_server != null)
                _server.Stop();
        }

        #endregion

        [Test]
        public void CanConstruct()
        {
            var builder = HttpCallBuilder.Create();

            Assert.NotNull(builder);
        }

        #region WithUri

        [Test]
        public void WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;
            var builder = HttpCallBuilder.Create().WithUri(uri);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(uri, actual);
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
        public void WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);
            var builder = HttpCallBuilder.Create().WithUri(uri);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(uri, actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithUri_WhenNullUri_ExpectException()
        {
            //arrange
            Uri uri = null;
            var builder = HttpCallBuilder.Create().WithUri(uri);

            //act
            builder.Result();
        }

        [Test]
        public void WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = "http://yahoo.com";
            var uri2 = TestUriString;
            var builder = HttpCallBuilder.Create().WithUri(uri1).WithUri(uri2);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(uri2, actual);
        }

        #endregion

        #region WithQuerystring

        [Test]
        public void WithQueryString_WhenValidValues_ExpectResultUsesUriAndQuerystring()
        {
            //arrange
            var uri = new Uri(TestUriString);
            var builder = HttpCallBuilder.Create().WithUri(uri);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(uri, actual);
        }

        [Test]
        [TestCase("q", "1", TestUriString + "?q=1")]
        [TestCase( "q", "1 and 2", TestUriString + "?q=1+and+2")]
        [TestCase( "q", null, TestUriString + "?q=")]
        [TestCase( "q", "", TestUriString + "?q=")]
        [TestCase( null, "1", TestUriString)]
        public void WithQueryString_WhenValidValues_ExpectResultQuerystring(string key, string value, string expected)
        {
            //arrange
            var uri = new Uri(TestUriString);
            var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString(key, value);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithQueryString_WithMultipleKeys_ExpectResultQuerystring()
        {
            var uri = new Uri(TestUriString + "?q1=1");
            var values = new NameValueCollection { { "q1", "2" }, { "q2", "1 and 2" }, { "q3", "3" } };
            var expected = new Uri(TestUriString + "?q1=2&q2=1+and+2&q3=3");

            var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString(values);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void WithQueryString_WithNullValues_ExpectSameUri()
        {
            var expected = new Uri(TestUriString);

            var builder = HttpCallBuilder.Create().WithUri(expected).WithQueryString(null);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithQueryString_WithNoValues_ExpectSameUri()
        {
            var expected = new Uri(TestUriString);

            var builder = HttpCallBuilder.Create().WithUri(expected).WithQueryString(new NameValueCollection());
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(TestUriString + "?q=1");
            var expected = new Uri(TestUriString + "?q=2");

            var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString(new NameValueCollection { { "q", "2" } });
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(TestUriString + "?q=1");
            var expected = new Uri(TestUriString + "?q=2");

            var builder = HttpCallBuilder.Create().WithUri(uri).WithQueryString("q", "2");
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithQueryString_WithUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(TestUriString + "?q=2");

            var builder = HttpCallBuilder.Create().WithQueryString("q2", "1").WithUri(expected);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();
            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithQueryString_WithBaseUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(TestUriString + "?q=2");

            var builder = HttpCallBuilder.Create().WithQueryString("q2", "1").WithBaseUri(expected);
            string actual = null;
            _server.InspectRequest(r => actual = r.RawUrl);

            // act
            builder.Result();
            // assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Http Methods

        [Test]
        public void Defaults_WhenNotMethodSpecified_ExpectGetMethod()
        {
            //arrange
            var expected = "GET";
            var builder = HttpCallBuilder.Create(TestUriString);
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsGet_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "GET";
            var builder = HttpCallBuilder.Create(TestUriString).AsGet();
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsPut_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PUT";
            var builder = HttpCallBuilder.Create(TestUriString).AsPut();
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsPost_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "POST";
            var builder = HttpCallBuilder.Create(TestUriString).AsPost();
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsDelete_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "DELETE";
            var builder = HttpCallBuilder.Create(TestUriString).AsDelete();
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsPatch_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "PATCH";
            var builder = HttpCallBuilder.Create(TestUriString).AsPatch();
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AsHead_WhenCalled_ExpectResultUsesMethod()
        {
            //arrange
            var expected = "HEAD";
            var builder = HttpCallBuilder.Create(TestUriString).AsHead();
            string actual = null;
            _server.InspectRequest(r => actual = r.HttpMethod);

            // act
            builder.Result();

            // assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Content

        [Test]
        public void WithContent_WhenValid_ExpectRequestContainsBody([Values("Content", "", null)]string content)
        {
            //arrange
            var uri = TestUriString;
            var builder = HttpCallBuilder.Create(uri).AsPost().WithContent(content);
            string actual = null;
            _server.InspectRequest(r => actual = r.Body);

            //act
            builder.Result();

            //assert
            Assert.AreEqual(content ?? string.Empty, actual);
        }

        #endregion
        /*
         IHttpCallBuilder AsGet();
        IHttpCallBuilder AsPut();
        IHttpCallBuilder AsPost();
        IHttpCallBuilder AsDelete();
        IHttpCallBuilder AsPatch();
        IHttpCallBuilder AsHead();
        IHttpCallBuilder WithContent(string content);
        IHttpCallBuilder WithContent(string content, Encoding encoding);
        IHttpCallBuilder WithContent(string content, Encoding encoding, string mediaType);
        IHttpCallBuilder WithContent(Func<string> contentFactory);
        IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding);
        IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding, string mediaType);
        IHttpCallBuilder WithContent(Func<HttpContent> contentFactory);
        HttpResponseMessage Result();
        Task<HttpResponseMessage> Result();

        IHttpCallBuilder CancelRequest();

        IAdvancedHttpCallBuilder Advanced { get; }
         */
    }
}
