using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AonWeb.Fluent.Http;
using NUnit.Framework;
namespace AonWeb.Fluent.Tests.Http
{
    [TestFixture()]
    public class HttpClientBuilderTests
    {
        [Test()]
        public void CanConstruct()
        {
            var builder = CreateBuilder();

            Assert.NotNull(builder);
        }

        [Test()]
        public void WithConfiguration_When_Expect()
        {

        }

        [Test()]
        public void WithAutoRedirect_When_Expect()
        {

        }

        [Test()]
        public void WithAutoRedirect_When_Expect1()
        {

        }

        [Test()]
        public void WithDecompression_When_Expect()
        {

        }

        [Test()]
        public void WithClientCertificateOptions_When_Expect()
        {

        }

        [Test()]
        public void WithCookieContainer_When_Expect()
        {

        }

        [Test()]
        public void WithCredentials_When_Expect()
        {

        }

        [Test()]
        public void WithMaxBuffer_When_Expect()
        {

        }

        [Test()]
        public void WithProxy_When_Expect()
        {

        }

        [Test()]
        public void Create_When_Expect()
        {

        }

        private IHttpClientBuilder CreateBuilder()
        {
            return new HttpClientBuilder();
        }
    }
}
