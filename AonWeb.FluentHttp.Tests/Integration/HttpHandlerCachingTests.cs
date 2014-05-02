using System;
using System.Net;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Integration
{
    [TestFixture]
    public class HttpHandlerCachingTests
    {

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCacheAccrossCallBuilders()
        {
            var serverUrl = LocalWebServer.DefaultListenerUri;
            Helpers.Helper.DeleteUrlCacheEntry(serverUrl);
            using (var server = LocalWebServer.ListenInBackground(serverUrl))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" });

                var result1 = new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result;

                var result2 = new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result;

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCacheAccrossCallBuildersOnDifferentTheatres()
        {
            var serverUrl = LocalWebServer.DefaultListenerUri;
            Helpers.Helper.DeleteUrlCacheEntry(serverUrl);
            using (var server = LocalWebServer.ListenInBackground(serverUrl))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" });

                var result1 = Task.Factory.StartNew(() => 
                    new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result).Result;

                var result2 = Task.Factory.StartNew(() => 
                    new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result).Result;

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOff_ExpectContentsNotCached()
        {
            var serverUrl = LocalWebServer.DefaultListenerUri;
            Helpers.Helper.DeleteUrlCacheEntry(serverUrl);
            using (var server = LocalWebServer.ListenInBackground(serverUrl))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" });

                var result1 = new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Advanced
                    .WithNoCache()
                    .Result().Content.ReadAsStringAsync().Result;

                var result2 = new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Advanced
                    .WithNoCache()
                    .Result().Content.ReadAsStringAsync().Result;

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsCacheAccrossCallBuilders()
        {
            var proxy = new WebProxy(new Uri("http://how002233:8888"), false);

            var serverUrl = LocalWebServer.DefaultListenerUri;
            Helpers.Helper.DeleteUrlCacheEntry(serverUrl);
            using (var server = LocalWebServer.ListenInBackground(serverUrl))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }
                        .AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate")
                        .AddHeader("Expires", "-1"))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }
                        .AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate")
                        .AddHeader("Expires", "-1"));

                var result1 = new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Advanced.ConfigureClient(c => c.WithProxy(proxy))
                    .Result().Content.ReadAsStringAsync().Result;

                var result2 = new HttpCallBuilder()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Advanced.ConfigureClient(c => c.WithProxy(proxy))
                    .Result().Content.ReadAsStringAsync().Result;

                Assert.AreNotEqual(result1, result2);
            }
        }
    }
}