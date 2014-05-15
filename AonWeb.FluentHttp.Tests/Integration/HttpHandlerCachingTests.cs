using System;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Integration
{
    [TestFixture]
    public class HttpHandlerCachingTests
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = true;
        }

        [SetUp]
        public void Setup()
        {
            HttpCallBuilderDefaults.ClearCache();
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" }.AddPublicCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" }.AddPublicCacheHeader());

                var builder = HttpCallBuilder.Create().WithUri(LocalWebServer.DefaultListenerUri).Advanced.WithCaching();

                var response1 =  builder.Result();
                var result1 = response1.ReadContents();

                var response2 =  builder.Result();
                var result2 = response2.ReadContents();

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuilders()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());

                var response1 =  HttpCallBuilder.Create().WithUri(LocalWebServer.DefaultListenerUri).Result();
                var result1 = response1.ReadContents();

                

                var response2 =  HttpCallBuilder.Create().WithUri(LocalWebServer.DefaultListenerUri).Result();
                var result2 = response2.ReadContents();

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuildersOnDifferentThreads()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());

                var response1 =  Task.Factory.StartNew(() =>
                     HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result()).Result;

                var result1 = response1.ReadContents();

                var response2 =  Task.Factory.StartNew(() =>
                     HttpCallBuilder.Create()
                    .WithUri(LocalWebServer.DefaultListenerUri)
                    .Result()).Result;

                var result2 = response2.ReadContents();

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOff_ExpectContentsNotCached()
        {
            
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());
                
                var response1 =  HttpCallBuilder.Create()
                        .WithUri(LocalWebServer.DefaultListenerUri)
                        .Advanced.WithNoCache()
                        .Result();
                    
                var result1 = response1.ReadContents();

                var response2 =  HttpCallBuilder.Create()
                        .WithUri(LocalWebServer.DefaultListenerUri)
                        .Advanced.WithNoCache()
                        .Result();

                var result2 = response2.ReadContents();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsAreNotCached()
        {
            
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }.AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }.AddNoCacheHeader());

                var response1 =  HttpCallBuilder.Create()
                        .WithUri(LocalWebServer.DefaultListenerUri)
                        .Result();

                var result1 = response1.ReadContents();

                var response2 =  HttpCallBuilder.Create()
                        .WithUri(LocalWebServer.DefaultListenerUri)
                        .Result();

                var result2 = response2.ReadContents();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithPost_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());

                var response1 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri)
                        .Result();
                var result1 = response1.ReadContents();

                 HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri).AsPost()
                        .Result();

                var response2 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri)
                        .Result();
                var result2 = response2.ReadContents();

                Assert.AreNotEqual(result1, result2);
            }
        }


        [Test]
        public void WithDependendUrls_ExpectPostInvalidatesDependents()
        {
            var parentUri = LocalWebServer.DefaultListenerUri;
            var childUri = new Uri(Helper.CombineVirtualPaths(parentUri, "child"));
            using (var server = LocalWebServer.ListenInBackground(parentUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response2" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response2" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response3" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response3" }.AddPrivateCacheHeader());

                var parent1 = HttpCallBuilder.Create(parentUri).Advanced.WithDependentUri(childUri).Result().ReadContents();
                var child1 = HttpCallBuilder.Create(childUri).Result().ReadContents();
                var parent2 = HttpCallBuilder.Create(parentUri).Result().ReadContents();
                var child2 = HttpCallBuilder.Create(childUri).Advanced.Result().ReadContents();

                Assert.AreEqual(parent1, parent2);
                Assert.AreEqual(child1, child2);

                server.RemoveNextResponse().RemoveNextResponse();

                HttpCallBuilder.Create(parentUri).AsPost().Result();

                var parent3 = HttpCallBuilder.Create(parentUri).Result().ReadContents();
                var child3 = HttpCallBuilder.Create(childUri).Result().ReadContents();

                Assert.AreEqual("Parent Response3", parent3);
                Assert.AreEqual("Child Response3", child3);
            }
        }

        [Test]
        public void WithDependendUrlsThatAreSelfReferential_ExpectPostNoException()
        {
            var parentUri = new Uri(LocalWebServer.DefaultListenerUri);
            var childUri = new Uri(Helper.CombineVirtualPaths(parentUri.ToString(), "child"));
            var grandchildUri = new Uri(Helper.CombineVirtualPaths(parentUri.ToString(), "grandchild"));

            using (var server = LocalWebServer.ListenInBackground(parentUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Grandchild Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response2" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response2" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Grandchild Response2" }.AddPrivateCacheHeader());

                var parent1 = HttpCallBuilder.Create(parentUri).Advanced.WithDependentUris(new[] { parentUri, grandchildUri }).Result().ReadContents();
                var child1 = HttpCallBuilder.Create(childUri).Advanced.WithDependentUris(new[] { childUri, grandchildUri }).Result().ReadContents();
                var grandchild1 = HttpCallBuilder.Create(grandchildUri).Advanced.WithDependentUris(new[] { parentUri, childUri }).Result().ReadContents();

                HttpCallBuilder.Create(parentUri).AsPost().Result();

            }
            
        }

        [Test]
        public void WithDependendUrls2LevelsDeep_ExpectPostInvalidatesDependents()
        {
            var parentUri = new Uri(LocalWebServer.DefaultListenerUri);
            var childUri = new Uri(Helper.CombineVirtualPaths(parentUri.ToString(), "child"));
            var grandchildUri = new Uri(Helper.CombineVirtualPaths(parentUri.ToString(), "grandchild"));

            using (var server = LocalWebServer.ListenInBackground(parentUri))
            {

                server.AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Grandchild Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Parent Response2" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Child Response2" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Grandchild Response2" }.AddPrivateCacheHeader());

                var parent1 = HttpCallBuilder.Create(parentUri).Advanced.WithDependentUri(childUri).Result().ReadContents();
                var child1 = HttpCallBuilder.Create(childUri).Advanced.WithDependentUri(grandchildUri).Result().ReadContents();
                var grandchild1 = HttpCallBuilder.Create(grandchildUri).Result().ReadContents();

                HttpCallBuilder.Create(parentUri).AsPost().Result();

                var parent2 = HttpCallBuilder.Create(parentUri).Advanced.WithDependentUri(childUri).Result().ReadContents();
                var child2 = HttpCallBuilder.Create(childUri).Result().ReadContents();
                var grandchild2 = HttpCallBuilder.Create(childUri).Result().ReadContents();

                Assert.AreNotEqual(parent1, parent2);
                Assert.AreNotEqual(child1, child2);
                Assert.AreNotEqual(grandchild1, grandchild2);
            }
        }


        [Test]
        public void WhenHttpCachingIsOn_WithPut_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());

                var response1 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri)
                        .Result();
                var result1 = response1.ReadContents();

                 HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri).AsPut()
                        .Result();

                var response2 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri)
                        .Result();
                var result2 = response2.ReadContents();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithPatch_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());

                var response1 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri)
                        .Result();
                var result1 = response1.ReadContents();

                 HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri).AsPatch()
                        .Result();

                var response2 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri)
                        .Result();
                var result2 = response2.ReadContents();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithDelete_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response1" }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = "Response2" }.AddPrivateCacheHeader());

                var response1 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri).Result();

                var result1 = response1.ReadContents();

                HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri).AsDelete().Result();

                var response2 =  HttpCallBuilder.Create(LocalWebServer.DefaultListenerUri).Result();

                var result2 = response2.ReadContents();

                Assert.AreNotEqual(result1, result2);
            }
        }
    }
}