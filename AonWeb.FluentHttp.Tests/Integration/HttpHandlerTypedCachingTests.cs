using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Integration
{
    [TestFixture]
    public class HttpHandlerTypedCachingTests
    {
        #region Declarations, Set up, & Tear Down

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

        #endregion

        #region Test Classes

        public static string TestResultString1 = @"{""StringProperty"":""TestString1"",""IntProperty"":1,""BoolProperty"":true,""DateOffsetProperty"":""2000-01-01T00:00:00-05:00"",""DateProperty"":""2000-01-01T00:00:00""}";
        public static string TestResultString2 = @"{""StringProperty"":""TestString2"",""IntProperty"":2,""BoolProperty"":false,""DateOffsetProperty"":""2000-01-02T00:00:00-05:00"",""DateProperty"":""2000-01-02T00:00:00""}";

        public static TestResult TestResultValue1 = new TestResult
        {
            StringProperty = "TestString1",
            IntProperty = 1,
            BoolProperty = true,
            DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
            DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
        };

        public static TestResult TestResultValue2 = new TestResult
        {
            StringProperty = "TestString2",
            IntProperty = 2,
            BoolProperty = false,
            DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
            DateProperty = new DateTime(2000, 1, 2, 0, 0, 0),
        };


        public class TestResult : IEquatable<TestResult>
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public bool BoolProperty { get; set; }
            public DateTimeOffset DateOffsetProperty { get; set; }
            public DateTime DateProperty { get; set; }

            #region Equality

            public bool Equals(TestResult other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return DateProperty.Equals(other.DateProperty)
                    && DateOffsetProperty.Equals(other.DateOffsetProperty)
                    && BoolProperty.Equals(other.BoolProperty)
                    && IntProperty == other.IntProperty
                    && string.Equals(StringProperty, other.StringProperty);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                return Equals((TestResult)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = DateProperty.GetHashCode();
                    hashCode = (hashCode * 397) ^ DateOffsetProperty.GetHashCode();
                    hashCode = (hashCode * 397) ^ BoolProperty.GetHashCode();
                    hashCode = (hashCode * 397) ^ IntProperty;
                    hashCode = (hashCode * 397) ^ StringProperty.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(TestResult left, TestResult right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestResult left, TestResult right)
            {
                return !Equals(left, right);
            }

            #endregion

            public override string ToString()
            {
                return string.Format("s: '{0}', i: {1}, b: {2}, do: {3:r}, d: {4:r}", StringProperty, IntProperty, BoolProperty, DateOffsetProperty, DateProperty);
            }
        }

        public class CacheableTestResult : TestResult, ICacheableHttpResult
        {
            public TimeSpan? Duration
            {
                get
                {
                    return TimeSpan.FromMinutes(5);
                }
            }

            public IEnumerable<string> DependentUris { get { yield break; } }
        }

        public class ExpiredTestResult : TestResult, ICacheableHttpResult
        {
            public TimeSpan? Duration
            {
                get
                {
                    return TimeSpan.FromMinutes(-1);
                }
            }

            public IEnumerable<string> DependentUris { get { yield break; } }
        }

        #endregion

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var builder = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri);

                var result1 =  builder.Result();

                var result2 =  builder.Result();

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuilders()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();


                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_ExpectContentsCachedAccrossCallBuildersOnDifferentThreads()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 =  Task.Factory.StartNew(() =>
                     HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result()).Result;

                var result2 =  Task.Factory.StartNew(() =>
                     HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result()).Result;

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndServerDoesntSendCacheHeaders_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 });

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndServerDoesntSendCacheHeadersButTypeIsCacheable_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 });

                var result1 =  HttpCallBuilder<CacheableTestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var result2 =  HttpCallBuilder<CacheableTestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndServerSendsNoCacheHeadersAndTypeisCacheableButDurationZeroOrLess_ExpectContentsCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 })
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 });

                var result1 =  HttpCallBuilder<ExpiredTestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var result2 =  HttpCallBuilder<ExpiredTestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                Assert.AreEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOff_ExpectContentsNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Advanced.WithCaching(false)
                    .Result();

                var result2 = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Advanced.WithCaching(false)
                    .Result();



                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndNoCacheCalled_ExpectContentsNotCachedAndCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader())
                    .AddResponse(_ => new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader());

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Advanced.WithNoCache()
                    .Result();

                var result3 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                   .Create(LocalWebServer.DefaultListenerUri)
                   .Advanced.WithNoCache()
                   .Result();

                Assert.AreNotEqual(result1, result2);
                Assert.AreNotEqual(result2, result3);
            }
        }

        [Test]
        public void WhenHttpCachingIsOnAndServerSendsNoCacheHeader_ExpectContentsAreNotCached()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString1 }.AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString2 }.AddNoCacheHeader());

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithPost_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri)
                        .Result();

                var invalidatingResult =  HttpCallBuilder<EmptyResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri).AsPost()
                        .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri)
                        .Result();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithPut_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri)
                        .Result();

                var invalidatingResult =  HttpCallBuilder<EmptyResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri).AsPut()
                        .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri)
                        .Result();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithPatch_ExpectCacheInvalidated()
        {

            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri)
                        .Result();

                var invalidatingResult =  HttpCallBuilder<EmptyResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri).AsPatch()
                        .Result();

                var result2 =  HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(LocalWebServer.DefaultListenerUri)
                        .Result();

                Assert.AreNotEqual(result1, result2);
            }
        }

        [Test]
        public void WhenHttpCachingIsOn_WithDelete_ExpectCacheInvalidated()
        {
            using (var server = LocalWebServer.ListenInBackground(LocalWebServer.DefaultListenerUri))
            {
                server
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString1 }.AddPrivateCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo().AddNoCacheHeader())
                    .AddResponse(new LocalWebServerResponseInfo { Body = TestResultString2 }.AddPrivateCacheHeader());

                var result1 = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                var invalidatingResult = HttpCallBuilder<EmptyResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .AsDelete()
                    .Result();

                var result2 = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>
                    .Create(LocalWebServer.DefaultListenerUri)
                    .Result();

                Assert.AreNotEqual(result1, result2);
            }
        }
    }
}