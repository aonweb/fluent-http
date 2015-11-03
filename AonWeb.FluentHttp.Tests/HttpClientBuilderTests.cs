using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Settings;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests
{
    [Collection("LocalWebServer Tests")]
    public class HttpClientBuilderTests
    {
        [Fact]
        public void CanConstruct()
        {
            var builder = CreateHttpClientBuilder();

            builder.ShouldNotBeNull();
        }

        #region Configuration

        [Fact]
        public void WithConfiguration_WhenValidAction_ExpectActionCalled()
        {
            var called = false;
            Action<IHttpClient> config = client =>
            {
                called = true;
            };

            var builder = CreateHttpClientBuilder();

            //act
            builder.WithConfiguration(config).Build();

            //assert
            called.ShouldBeTrue();
        }

        [Fact]
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

            var builder = CreateHttpClientBuilder();
            stopwatch.Start();

            //act
            builder.WithConfiguration(config1).WithConfiguration(config2).Build();
            stopwatch.Stop();

            //assert
            called1.ShouldNotBeNull();
            called2.ShouldNotBeNull();
            called1.Value.ShouldBeLessThan(called2.Value, "Second config called before first");
        }

        [Fact]
        public void WithConfiguration_WhenActionIsNull_ExpectNoException()
        {
            Action<IHttpClient> config = null;

            var builder = CreateHttpClientBuilder();

            //act
            builder.WithConfiguration(config).Build();
        }

        #endregion

        #region Headers

        [Fact]
        public void WithHeaders_WhenCalledWithNameAndValue_ExpectAppliedToClient()
        {
            // arrange

            var builder = CreateHttpClientBuilder();

            // act
            var client = builder.WithHeader("Accept", "application/json").Build();

            // assert
            client.DefaultRequestHeaders.Accept.First().MediaType.ShouldBe("application/json");
        }

        [Fact]
        public void WithHeaders_WhenCalledWithNameAndValues_ExpectAppliedToClient()
        {
            // arrange

            var builder = CreateHttpClientBuilder();

            // act
            var client = builder.WithHeader("Accept", new[] { "text/html", "text/xhtml" }).Build();

            // assert
            client.DefaultRequestHeaders.Accept.First().MediaType.ShouldBe("text/html");
            client.DefaultRequestHeaders.Accept.Last().MediaType.ShouldBe("text/xhtml");
        }

        [Fact]
        public void WithHeaders_WhenCalledWithAction_ExpectAppliedToClient()
        {
            // arrange

            var builder = CreateHttpClientBuilder();

            // act
            var client = builder.WithHeadersConfiguration(
                h =>
                {
                    h.AcceptCharset.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
                    h.CacheControl = new CacheControlHeaderValue { NoStore = true };
                }).Build();

            // assert
            client.DefaultRequestHeaders.AcceptCharset.First().Value.ShouldBe(Encoding.UTF8.WebName);
            client.DefaultRequestHeaders.CacheControl.NoStore.ShouldBeTrue();
        }


        #endregion

        [Fact]
        public void WithClientCertificateOptions_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = ClientCertificateOption.Automatic;
            var builder = CreateHttpClientBuilder();
            ClientCertificateOption? actual = null;
            // act
            var client = builder.WithClientCertificateOptions(expected)
                .WithConfiguration(s =>
                {
                    actual = s.ClientCertificateOptions;
                })

                .Build();

            // assert
            actual.ShouldNotBeNull();
            actual.Value.ShouldBe(expected);
        }

        // TODO: LocalWebServer test with verify cookie round trip
        [Fact]
        public void WithCookieContainer_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = new CookieContainer();
            var builder = CreateHttpClientBuilder();
            CookieContainer actual = null;
            // act
            var client = builder.WithUseCookies(expected).WithConfiguration(s =>
            {
                actual = s.CookieContainer;
            }).Build();

            // assert
            actual.ShouldNotBeNull();
            actual.ShouldBe(expected);
        }

        // TODO: LocalWebServer test with verify credentials sent
        [Fact]
        public void WithCredentials_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = new NetworkCredential("username", "password");
            var builder = CreateHttpClientBuilder();
            ICredentials actual = null;
            // act
            var client = builder.WithCredentials(expected).WithConfiguration(s =>
            {
                actual = s.Credentials;
            }).Build();

            // assert
            actual.ShouldNotBeNull();
            actual.ShouldBe(expected);
        }

        // TODO: LocalWebServer test with big content, verify buffer size
        [Fact]
        public void WithMaxRequestBuffer_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = 10000;
            var builder = CreateHttpClientBuilder();
            long? actual = null;
            // act
            var client = builder.WithMaxRequestBufferSize(expected).WithConfiguration(s =>
            {
                actual = s.MaxRequestContentBufferSize;
            }).Build();

            // assert
            actual.ShouldNotBeNull();
            actual.Value.ShouldBe(expected);
        }

        // TODO: LocalWebServer test with big response, verify buffer size
        [Fact]
        public void WithMaxResponseBuffer_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = 10000;
            var builder = CreateHttpClientBuilder();
            long? actual = null;
            // act
            var client = builder.WithMaxResponseBufferSize(expected).WithConfiguration(s =>
            {
                actual = s.MaxResponseContentBufferSize;
            }).Build();

            // assert
            actual.ShouldNotBeNull();
            actual.Value.ShouldBe(expected);
        }

        [Fact]
        public void WithProxy_WhenSet_ExpectSettingsUpdated()
        {
            // arrange
            var expected = new WebProxy("http://localhost:8888");
            var builder = CreateHttpClientBuilder();
            IWebProxy actual = null;
            // act
            var client = builder.WithProxy(expected).WithConfiguration(s =>
            {
                actual = s.Proxy;
            }).Build();

            // assert
            actual.ShouldNotBeNull();
            actual.ShouldBe(expected);
        }

        private IHttpClientBuilder CreateHttpClientBuilder()
        {
            return new HttpClientBuilder(new HttpClientSettings());
        }
    }
}