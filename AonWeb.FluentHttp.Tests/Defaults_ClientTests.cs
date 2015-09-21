using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Client;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests
{
    [TestFixture]
    public class Defaults_ClientTests
    {
        [SetUp]
        public void Setup()
        {
            Defaults.Reset();
        }

        public void DefaultValue_DecompressionMethods_HasDefaultValue()
        {
            Assert.AreEqual(DecompressionMethods.GZip | DecompressionMethods.Deflate, Defaults.Client.DecompressionMethods);
        }

        public void HttpClientSettings_WithDefaultTimeout_ExpectDefaultValueUsed()
        {
            var expected = TimeSpan.FromSeconds(90);
            Defaults.Client.Timeout = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.Timeout);
        }

        public void HttpClientSettings_WithDefaultMaxRequestContentBufferSize_ExpectDefaultValueUsed()
        {
            var expected = 1024;
            Defaults.Client.MaxRequestContentBufferSize = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.MaxRequestContentBufferSize);
        }

        public void HttpClientSettings_WithDefaultClientConfiguration_ExpectDefaultValueUsed()
        {
            Action<IHttpClient> expected = c => c.Timeout = TimeSpan.FromSeconds(45);
            Defaults.Client.ClientConfiguration = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.ClientConfiguration);
        }

        public void HttpClientSettings_WithDefaultRequestHeaderConfiguration_ExpectDefaultValueUsed()
        {
            Action<HttpRequestHeaders> expected = h => h.CacheControl = new CacheControlHeaderValue();
            Defaults.Client.RequestHeaderConfiguration = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.RequestHeaderConfiguration);
        }

        public void HttpClientSettings_WithDefaultDecompressionMethods_ExpectDefaultValueUsed()
        {
            var expected = DecompressionMethods.GZip;
            Defaults.Client.DecompressionMethods = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.DecompressionMethods);
        }

        public void HttpClientSettings_WithDefaultClientCertificateOptions_ExpectDefaultValueUsed()
        {
            var expected = ClientCertificateOption.Manual;
            Defaults.Client.ClientCertificateOptions = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.ClientCertificateOptions);
        }

        public void HttpClientSettings_WithDefaultCredentials_ExpectDefaultValueUsed()
        {
            var expected = new NetworkCredential("username", "password");
            Defaults.Client.Credentials = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.Credentials);
        }

        public void HttpClientSettings_WithDefaultCookieContainer_ExpectDefaultValueUsed()
        {
            var expected = new CookieContainer();
            Defaults.Client.CookieContainer = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.CookieContainer);
        }

        public void HttpClientSettings_WithDefaultProxy_ExpectDefaultValueUsed()
        {
            var expected = new WebProxy();
            Defaults.Client.Proxy = expected;
            var settings = new HttpClientSettings();

            Assert.AreEqual(expected, settings.Proxy);
        }

    }
}
