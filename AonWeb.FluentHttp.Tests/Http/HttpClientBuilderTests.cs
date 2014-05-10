using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using AonWeb.FluentHttp.Client;
using NUnit.Framework;
namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpClientBuilderTests
    {
        [Test]
        public void CanConstruct()
        {
            var builder = CreateBuilder();

            Assert.NotNull(builder);
        }

        #region Configuration

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

        #endregion

        #region Headers

        [Test]
        public void WithHeaders_WhenCalledWithNameAndValue_ExpectAppliedToClient()
        {
            // arrange
            
            var builder = CreateBuilder();

            // act
            var client = builder.WithHeaders("Accept", "application/json").Create();

            // assert
            Assert.AreEqual("application/json", client.DefaultRequestHeaders.Accept.First().MediaType);
        }

        [Test]
        public void WithHeaders_WhenCalledWithNameAndValues_ExpectAppliedToClient()
        {
            // arrange

            var builder = CreateBuilder();

            // act
            var client = builder.WithHeaders("Accept", new[] { "text/html", "text/xhtml" }).Create();

            // assert
            Assert.AreEqual("text/html", client.DefaultRequestHeaders.Accept.First().MediaType);
            Assert.AreEqual("text/xhtml", client.DefaultRequestHeaders.Accept.Last().MediaType);
        }

        [Test]
        public void WithHeaders_WhenCalledWithAction_ExpectAppliedToClient()
        {
            // arrange

            var builder = CreateBuilder();

            // act
            var client = builder.WithHeaders(
                h =>
                    {
                        h.AcceptCharset.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
                        h.CacheControl = new CacheControlHeaderValue{ NoStore = true };
                    }).Create();

            // assert
            Assert.AreEqual(Encoding.UTF8.WebName, client.DefaultRequestHeaders.AcceptCharset.First().Value);
            Assert.IsTrue(client.DefaultRequestHeaders.CacheControl.NoStore);
        }


        #endregion

        [Test]
        public void WithClientCertificateOptions_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = ClientCertificateOption.Automatic;
            var builder = CreateBuilder();

            // act
            var client = builder.WithClientCertificateOptions(expected).Create();

            // assert
            Assert.AreEqual(expected, builder.Settings.ClientCertificateOptions);
        }

        // TODO: LocalWebServer test with verify cookie round trip
        [Test]
        public void WithCookieContainer_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = new CookieContainer();
            var builder = CreateBuilder();

            // act
            var client = builder.WithUseCookies(expected).Create();

            // assert
            Assert.AreEqual(expected, builder.Settings.CookieContainer);
        }

        // TODO: LocalWebServer test with verify credentials sent
        [Test]
        public void WithCredentials_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = new NetworkCredential("username", "password");
            var builder = CreateBuilder();

            // act
            var client = builder.WithCredentials(expected).Create();

            // assert
            Assert.AreEqual(expected, builder.Settings.Credentials);
        }

        // TODO: LocalWebServer test with big content, verify buffer size
        [Test]
        public void WithMaxBuffer_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = 10000;
            var builder = CreateBuilder();

            // act
            var client = builder.WithMaxBufferSize(expected).Create();

            // assert
            Assert.AreEqual(expected, builder.Settings.MaxRequestContentBufferSize);
        }

        [Test]
        public void WithProxy_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = new WebProxy("http://localhost:8888");
            var builder = CreateBuilder();

            // act
            var client = builder.WithProxy(expected).Create();

            // assert
            Assert.AreEqual(expected, builder.Settings.Proxy);
        }

        private IHttpClientBuilder CreateBuilder()
        {
            return new HttpClientBuilder();
        }
    }
}
