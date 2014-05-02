using System;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;

using Moq;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderTests
    {
        private string TestUriString = "http://google.com";
        private Mock<IHttpClient> _client;
        private Mock<IHttpClientBuilder> _factory;

        [Test]
        public void CanConstruct()
        {
            var builder = CreateBuilder();

            Assert.NotNull(builder);
        }

        #region WithUri

        [Test]
        public void WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = TestUriString;
            var builder = CreateBuilder().WithUri(uri);

            //act
            builder.ResultAsync();

            //assert
            _client.VerifyRequest(r => r.RequestUri.OriginalString == uri);
        }

        [Test]
        [ExpectedException(typeof(UriFormatException))]
        public void WithUri_WhenInvalidString_ExpectException()
        {
            //arrange
            var uri = "blah blah";
            var builder = CreateBuilder();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenNullString_ExpectException()
        {
            //arrange
            string uri = null;
            var builder = CreateBuilder();

            //act
            builder.WithUri(uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithUri_WhenEmptyString_ExpectException()
        {
            //arrange
            var uri = string.Empty;
            var builder = CreateBuilder();

            //act
            builder.WithUri(uri);
        }

        [Test]
        public void WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(TestUriString);
            var builder = CreateBuilder().WithUri(uri);

            //act
            builder.ResultAsync();

            //assert
            _client.VerifyRequest(r => r.RequestUri == uri);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithUri_WhenNullUri_ExpectException()
        {
            //arrange
            Uri uri = null;
            var builder = CreateBuilder().WithUri(uri);

            //act
            builder.Result();
        }

        [Test]
        public void WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = "http://yahoo.com";
            var uri2 = TestUriString;
            var builder = CreateBuilder().WithUri(uri1).WithUri(uri2);

            //act
            builder.ResultAsync();

            //assert
            _client.VerifyRequest(r => r.RequestUri.OriginalString == uri2);
        }

        #endregion

        #region WithMethod

        [Test]
        public void WithMethod_WhenValidString_ExpectResultUsesMethod()
        {
            //arrange
            var method = "GET";
            var builder = CreateBuilder().WithUri(TestUriString).WithMethod(method);

            //act
            builder.ResultAsync();

            //assert
            _client.VerifyRequest(r => r.Method.Method == method);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenNullString_ExpectException()
        {
            //arrange
            string method = null;
            var builder = CreateBuilder();

            //act
            builder.WithMethod(method);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenEmptyString_ExpectException()
        {
            //arrange
            var method = string.Empty;
            var builder = CreateBuilder();

            //act
            builder.WithMethod(method);
        }

        [Test]
        public void WithMethod_WhenValidMethod_ExpectResultUsesMethod()
        {
            //arrange
            var method = HttpMethod.Get;
            var builder = CreateBuilder().WithUri(TestUriString).WithMethod(method);

            //act
            builder.ResultAsync();

            //assert
            _client.VerifyRequest(r => r.Method == method);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithMethod_WhenNullMethod_ExpectException()
        {
            //arrange
            HttpMethod method = null;
            var builder = CreateBuilder().WithMethod(method);

            //act
            builder.Result();
        }

        [Test]
        public void WithMethod_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var method1 = "POST";
            var method2 = "GET";
            var builder = CreateBuilder().WithUri(TestUriString).WithMethod(method1).WithMethod(method2);

            //act
            builder.ResultAsync();

            //assert
            _client.VerifyRequest(r => r.Method.Method == method2);
        }

        #endregion

        [Test]
        public void WithConfiguration_WhenAction_ExpectClientFactoryCalled()
        {
            Action<IHttpClient> config = client => { };

            var builder = CreateBuilder();

            //act
            builder.ConfigureClient(config).ResultAsync();

            //assert
            _factory.Verify(f => f.Configure(It.Is<Action<IHttpClient>>(a => a == config)), Times.Once);
        }

        private HttpCallBuilder CreateBuilder()
        {
            _client = new Mock<IHttpClient>();
            _factory = new Mock<IHttpClientBuilder>();

            _factory.Setup(f => f.Create()).Returns(() => _client.Object);

            return new HttpCallBuilder(_factory.Object);
        }
    }
}
