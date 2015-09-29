using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Client;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests
{
    public class ClientDefaultsTests
    {

        public ClientDefaultsTests()
        {
            Defaults.Reset();
        }

        [Fact]
        public void DefaultValue_DecompressionMethods_HasDefaultValue()
        {
             Defaults.Client.DecompressionMethods.ShouldBe(DecompressionMethods.GZip | DecompressionMethods.Deflate);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultTimeout_ExpectDefaultValueUsed()
        {
            var expected = TimeSpan.FromSeconds(90);
            Defaults.Client.Timeout = expected;
            var settings = new HttpClientSettings();

             settings.Timeout.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultMaxRequestContentBufferSize_ExpectDefaultValueUsed()
        {
            var expected = 1024;
            Defaults.Client.MaxRequestContentBufferSize = expected;
            var settings = new HttpClientSettings();

             settings.MaxRequestContentBufferSize.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultClientConfiguration_ExpectDefaultValueUsed()
        {
            Action<IHttpClient> expected = c => c.Timeout = TimeSpan.FromSeconds(45);
            Defaults.Client.ClientConfiguration = expected;
            var settings = new HttpClientSettings();

             settings.ClientConfiguration.ShouldBe(expected);
        }
        [Fact]
        public void HttpClientSettings_WithDefaultRequestHeaderConfiguration_ExpectDefaultValueUsed()
        {
            Action<HttpRequestHeaders> expected = h => h.CacheControl = new CacheControlHeaderValue();
            Defaults.Client.RequestHeaderConfiguration = expected;
            var settings = new HttpClientSettings();

             settings.RequestHeaderConfiguration.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultDecompressionMethods_ExpectDefaultValueUsed()
        {
            var expected = DecompressionMethods.GZip;
            Defaults.Client.DecompressionMethods = expected;
            var settings = new HttpClientSettings();

             settings.DecompressionMethods.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultClientCertificateOptions_ExpectDefaultValueUsed()
        {
            var expected = ClientCertificateOption.Manual;
            Defaults.Client.ClientCertificateOptions = expected;
            var settings = new HttpClientSettings();

             settings.ClientCertificateOptions.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultCredentials_ExpectDefaultValueUsed()
        {
            var expected = new NetworkCredential("username", "password");
            Defaults.Client.Credentials = expected;
            var settings = new HttpClientSettings();

             settings.Credentials.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultCookieContainer_ExpectDefaultValueUsed()
        {
            var expected = new CookieContainer();
            Defaults.Client.CookieContainer = expected;
            var settings = new HttpClientSettings();

             settings.CookieContainer.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultProxy_ExpectDefaultValueUsed()
        {
            var expected = new WebProxy();
            Defaults.Client.Proxy = expected;
            var settings = new HttpClientSettings();

             settings.Proxy.ShouldBe(expected);
        }

    }
}
