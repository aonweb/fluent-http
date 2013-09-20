using System;
using AonWeb.Fluent.Http;
using Moq;
using NUnit.Framework;

namespace AonWeb.Fluent.Tests.Http
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

        [Test]
        public void WithUri_When_Expect()
        {

        }

        [Test]
        public void WithUri_When_Expect1()
        {

        }

        [Test]
        public void WithMethod_When_Expect()
        {

        }

        [Test]
        public void WithMethod_When_Expect1()
        {

        }

        [Test]
        public void WithConfiguration_When_Expect()
        {

        }

        [Test]
        public void WithResultOfType_When_Expect()
        {

        }

        [Test]
        public void Result_When_Expect()
        {

        }

        [Test]
        public void ResultAsync_When_Expect()
        {

        }

        [Test]
        public void ResultAsync_When_Expect1()
        {

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
