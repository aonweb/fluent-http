using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [Test]
        public void WithConfiguration_WhenValidAction_ExpectActionCalled()
        {
            var called = false;
            Action<IHttpClient> config = client =>
            {
                called = true;
            };

            var builder = CreateBuilder();

            //act
            builder.Configure(config).Create();

            //assert
            Assert.IsTrue(called);
        }

        [Test]
        public void WithConfiguration_WhenMultipleConfig_ExpectAllCalledInOrder()
        {
            var stopwatch = new Stopwatch();

            TimeSpan? called1 = null;
            TimeSpan? called2 = null;
            Action<IHttpClient> config1 = client =>
            {
                called1 = stopwatch.Elapsed;
            };
            Action<IHttpClient> config2 = client =>
            {
                called2 = stopwatch.Elapsed;
            };

            var builder = CreateBuilder();
            stopwatch.Start();

            //act
            builder.Configure(config1).Configure(config2).Create();
            stopwatch.Stop();

            //assert
            Assert.NotNull(called1, "First config not called");
            Assert.NotNull(called2, "Second config not called");
            Assert.Greater(called2, called1, "Second config called before first");
        }

        [Test]
        public void WithConfiguration_WhenActionIsNull_ExpectNoException()
        {
            Action<IHttpClient> config = null;

            var builder = CreateBuilder();

            //act
            builder.Configure(config).Create();

            //assert
            Assert.Pass();
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
