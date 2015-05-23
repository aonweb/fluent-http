using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http {
    [TestFixture]
    public class MockTypedHttpCallBuilderTests {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion

        #region Test Classes

        public static string TestResultString = @"{""StringProperty"":""TestString"",""IntProperty"":2,""BoolProperty"":true,""DateOffsetProperty"":""2000-01-01T00:00:00-05:00"",""DateProperty"":""2000-01-01T00:00:00""}";
        public static TestResult TestResultValue = new TestResult();
        public static TestCachedResult TestCachedResultValue = new TestCachedResult();

        public class TestCachedResult : TestResult, ICacheableHttpResult
        {
            public TimeSpan? Duration
            {
                get
                {
                    return TimeSpan.FromMinutes(10);
                }
            }

            public IEnumerable<Uri> DependentUris
            {
                get
                {
                    return Enumerable.Empty<Uri>();
                }
            }
        }

        public class TestResult : IEquatable<TestResult> {
            public TestResult()
            {
                StringProperty = "TestString";
                IntProperty = 2;
                BoolProperty = true;
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5));
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0);
            }

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
                if (obj.GetType() != GetType())
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
        }

        #endregion

        [Test]
        public async Task WithResultExpectResultReturned()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder().WithResult(TestResultValue).WithUri(TestUriString);

            //act
            var actual = await builder.ResultAsync<TestResult>();

            Assert.AreEqual(TestResultValue, actual);
        }

        [Test]
        public async Task WithErrorExpectResultReturned()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder().WithError(TestResultValue).WithUri(TestUriString);

            TestResult actual = null;

            builder.Advanced.OnError<TestResult>(ctx => actual = ctx.Error);

            //act
            try
            {
                await builder.SendAsync();
            }
            catch (HttpErrorException<TestResult>) { }


            Assert.AreEqual(TestResultValue, actual);
        }


        [Test]
        public async Task WithContent_ExpectNoExceptions()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder().WithResult(TestResultValue).WithUri(TestUriString);

            //act
            var actual = await builder.WithContent(() => TestResultValue).AsPost().ResultAsync<TestResult>();

            Assert.AreEqual(TestResultValue, actual);
        }

        [Test]
        public async Task WithResultWithCachingExpectResultReturned()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder().WithResult(TestCachedResultValue).WithUri(TestUriString);

            //act
            var actual1 = await builder.ResultAsync<TestCachedResult>();
            var actual2 = await builder.ResultAsync<TestCachedResult>();

            Assert.AreEqual(TestResultValue, actual1);
            Assert.AreEqual(actual1, actual2);
        }
    }
}
