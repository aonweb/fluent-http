using System.Threading.Tasks;

using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Integration
{
    [TestFixture]
    public class HttpHandlerCachingTests
    {
        [SetUp]
        public void Setup()
        {
            TestHelper.DeleteUrlCacheEntry(LocalWebServer.DefaultListenerUri);
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCacheAccrossCallBuilders()
        {
            
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" });

                var result1 = HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result;

                var result2 = HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result;

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCacheAccrossCallBuildersOnDifferentThreads()
        {
            
            
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" });

                var result1 = Task.Factory.StartNew(() => 
                    HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result).Result;

                var result2 = Task.Factory.StartNew(() => 
                    HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result).Result;

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOff_ExpectContentsNotCached()
        {
            
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" });

                var result1 = HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Advanced
                    .WithNoCache()
                    .Result().Content.ReadAsStringAsync().Result;

                var result2 = HttpCallBuilder.Create()
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
            
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }
                        .AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate")
                        .AddHeader("Expires", "-1"))
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }
                        .AddHeader("Cache-Control", "private, no-store, no-cache, must-revalidate")
                        .AddHeader("Expires", "-1"));

                var result1 = HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result;

                var result2 = HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result().Content.ReadAsStringAsync().Result;

                Assert.AreNotEqual(result1, result2);
            }
        }
    }
}