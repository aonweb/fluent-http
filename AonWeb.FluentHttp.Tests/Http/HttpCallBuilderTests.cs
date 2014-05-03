using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Tests.Helpers;
using Moq;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderTests
    {
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

        [Test]
        public void CanConstruct()
        {
            var builder = new HttpCallBuilder();

            Assert.NotNull(builder);
        }

        #region WithUri

        [Test]
        public void WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;
            var builder = new HttpCallBuilder().WithUri(uri);

            _server.InspectRequest(r => Assert.AreEqual(r.RawUrl, uri));

            builder.ResultAsync();
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void WithUri_WhenInvalidString_ExpectException()
        {
            //arrange
            var uri = "blah blah";
            var builder = new HttpCallBuilder();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenNullString_ExpectException()
        {
            //arrange
            string uri = null;
            var builder = new HttpCallBuilder();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenEmptyString_ExpectException()
        {
            //arrange
            var uri = string.Empty;
            var builder = new HttpCallBuilder();

            //act
            builder.WithUri(uri);
        }

        [Test]
        public void WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);
            var builder = new HttpCallBuilder().WithUri(uri);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.RawUrl, uri));

            //act
            builder.ResultAsync();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithUri_WhenNullUri_ExpectException()
        {
            //arrange
            Uri uri = null;
            var builder = new HttpCallBuilder().WithUri(uri);

            //act
            builder.Result();
        }

        [Test]
        public void WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = "http://yahoo.com";
            var uri2 = TestUriString;
            var builder = new HttpCallBuilder().WithUri(uri1).WithUri(uri2);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.RawUrl, uri2));

            //act
            builder.ResultAsync();
        }

        #endregion

        #region WithMethod

        [Test]
        public void WithMethod_WhenValidString_ExpectResultUsesMethod()
        {
            //arrange
            var method = "GET";
            var builder = new HttpCallBuilder().WithUri(TestUriString).WithMethod(method);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.HttpMethod, method));

            //act
            builder.ResultAsync();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenNullString_ExpectException()
        {
            //arrange
            string method = null;
            var builder = new HttpCallBuilder();

            //act
            builder.WithMethod(method);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenEmptyString_ExpectException()
        {
            //arrange
            var method = string.Empty;
            var builder = new HttpCallBuilder();

            //act
            builder.WithMethod(method);
        }

        [Test]
        public void WithMethod_WhenValidMethod_ExpectResultUsesMethod()
        {
            //arrange
            var method = HttpMethod.Get;
            var builder = new HttpCallBuilder().WithUri(TestUriString).WithMethod(method);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.HttpMethod, method.Method));

            //act
            builder.ResultAsync();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithMethod_WhenNullMethod_ExpectException()
        {
            //arrange
            HttpMethod method = null;
            var builder = new HttpCallBuilder().WithMethod(method);

            //act
            builder.Result();
        }

        [Test]
        public void WithMethod_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var method1 = "POST";
            var method2 = "GET";
            var builder = new HttpCallBuilder().WithUri(TestUriString).WithMethod(method1).WithMethod(method2);

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.HttpMethod, method2));

            //act
            builder.ResultAsync();
        }

        #endregion

        [Test]
        public void WithConfiguration_WhenAction_ExpectConfigurationApplied()
        {
            Action<IHttpClient> config = client => client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var builder = new HttpCallBuilder();

            // assert (called after act)
            _server.InspectRequest(r => Assert.AreEqual(r.Headers["Accept"], "application/json"));

            //act
            builder.ConfigureClient(config).ResultAsync();


        }
    }
}
