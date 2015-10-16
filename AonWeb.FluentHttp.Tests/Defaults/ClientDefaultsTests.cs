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
            Defaults.Current.Reset();
        }

        [Fact]
        public void DefaultValue_DecompressionMethods_HasDefaultValue()
        {
             Defaults.Current.GetClientDefaults().DecompressionMethods.ShouldBe(DecompressionMethods.GZip | DecompressionMethods.Deflate);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultTimeout_ExpectDefaultValueUsed()
        {
            var expected = TimeSpan.FromSeconds(90);
            Defaults.Current.GetClientDefaults().Timeout = expected;
            var settings = new HttpClientSettings();

             settings.Timeout.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultMaxRequestContentBufferSize_ExpectDefaultValueUsed()
        {
            var expected = 1024;
            Defaults.Current.GetClientDefaults().MaxRequestContentBufferSize = expected;
            var settings = new HttpClientSettings();

             settings.MaxRequestContentBufferSize.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultClientConfiguration_ExpectDefaultValueUsed()
        {
            Action<IHttpClient> expected = c => c.Timeout = TimeSpan.FromSeconds(45);
            Defaults.Current.GetClientDefaults().ClientConfiguration = expected;
            var settings = new HttpClientSettings();

             settings.ClientConfiguration.ShouldBe(expected);
        }
        [Fact]
        public void HttpClientSettings_WithDefaultRequestHeaderConfiguration_ExpectDefaultValueUsed()
        {
            Action<HttpRequestHeaders> expected = h => h.CacheControl = new CacheControlHeaderValue();
            Defaults.Current.GetClientDefaults().RequestHeaderConfiguration = expected;
            var settings = new HttpClientSettings();

             settings.RequestHeaderConfiguration.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultDecompressionMethods_ExpectDefaultValueUsed()
        {
            var expected = DecompressionMethods.GZip;
            Defaults.Current.GetClientDefaults().DecompressionMethods = expected;
            var settings = new HttpClientSettings();

             settings.DecompressionMethods.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultClientCertificateOptions_ExpectDefaultValueUsed()
        {
            var expected = ClientCertificateOption.Manual;
            Defaults.Current.GetClientDefaults().ClientCertificateOptions = expected;
            var settings = new HttpClientSettings();

             settings.ClientCertificateOptions.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultCredentials_ExpectDefaultValueUsed()
        {
            var expected = new NetworkCredential("username", "password");
            Defaults.Current.GetClientDefaults().Credentials = expected;
            var settings = new HttpClientSettings();

             settings.Credentials.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultCookieContainer_ExpectDefaultValueUsed()
        {
            var expected = new CookieContainer();
            Defaults.Current.GetClientDefaults().CookieContainer = expected;
            var settings = new HttpClientSettings();

             settings.CookieContainer.ShouldBe(expected);
        }

        [Fact]
        public void HttpClientSettings_WithDefaultProxy_ExpectDefaultValueUsed()
        {
            var expected = new WebProxy();
            Defaults.Current.GetClientDefaults().Proxy = expected;
            var settings = new HttpClientSettings();

             settings.Proxy.ShouldBe(expected);
        }

    }
}
