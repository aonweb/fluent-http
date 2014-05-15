using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class TypedHttpCallBuilderTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion

        #region Test Classes

        public static string TestResultString = @"{""StringProperty"":""TestString"",""IntProperty"":2,""BoolProperty"":true,""DateOffsetProperty"":""2000-01-01T00:00:00-05:00"",""DateProperty"":""2000-01-01T00:00:00""}";
        public static TestResult TestResultValue = new TestResult();

        public class TestResult : IEquatable<TestResult>
        {
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
        }

        #endregion

        #region Results

        [Test]
        public void WhenComplexTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK,
                    Body = TestResultString
                });
                var builder = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(TestUriString);

                //act
                var actual = builder.ResultAsync().Result;

                Assert.AreEqual(TestResultValue, actual);
            }
        }

        [Test]
        public void WhenSimpleTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK,
                    Body = "true"
                });

                var builder = HttpCallBuilder<bool, EmptyRequest, EmptyError>.Create(TestUriString);

                //act
                var actual = builder.ResultAsync().Result;

                Assert.IsTrue(actual);
            }
        }

        [Test]
        public void WhenStringTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var expected = "some string data";
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK,
                    Body = "\"" + expected + "\""
                });

                var builder = HttpCallBuilder<string, EmptyRequest, EmptyError>.Create(TestUriString);

                //act
                var actual = builder.ResultAsync().Result;

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void WhenEmptyStringTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK
                });

                var builder = HttpCallBuilder<string, EmptyRequest, EmptyError>.Create(TestUriString);

                //act
                var actual = builder.ResultAsync().Result;

                Assert.IsNullOrEmpty(actual);
            }
        }

        [Test]
        public void WhenEmptyTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK
                });

                var builder = HttpCallBuilder<TestResult, EmptyRequest, EmptyError>.Create(TestUriString);

                //act
                var actual = builder.ResultAsync().Result;

                Assert.IsNull(actual);
            }
        }

        #endregion

        #region Content

        [Test]
        public void WhenPostingComplexType_ExpectRequestContentSerializedCorrectly()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var expected = TestResultString;
                server.AddResponse(new LocalWebServerResponseInfo());

                //arrange
                var builder = HttpCallBuilder<EmptyResult, TestResult, EmptyError>.Create(TestUriString).AsPost();
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                builder.WithContent(()=> TestResultValue).SendAsync().Wait();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void WhenPuttingSimpleTyped_ExpectRequestContentSerializedCorrectly()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var expected = "true";
                server.AddResponse(new LocalWebServerResponseInfo());

                var builder = HttpCallBuilder<EmptyResult, bool, EmptyError>.Create(TestUriString).AsPut();
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                builder.WithContent(() => true).SendAsync().Wait();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public async Task WhenPostingStringType_ExpectRequestContentSerializedCorrectly()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var expected = "some string data";
                server.AddResponse(new LocalWebServerResponseInfo());

                var builder = HttpCallBuilder<EmptyResult, string, EmptyError>.Create(TestUriString).AsPost();
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                var result = await builder.WithContent(() => expected).ResultAsync();

                Assert.AreEqual("\"" + expected + "\"", actual);
            }
        }

        [Test]
        public void WhenPostingEmptyStringType_ExpectRequestContentSerializedCorrectly()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var expected = "null";
                server.AddResponse(new LocalWebServerResponseInfo());

                var builder = HttpCallBuilder<EmptyResult, string, EmptyError>.Create(TestUriString).AsPost();
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                builder.WithContent(() => null).SendAsync().Wait();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void WhenPostingEmptyType_ExpectRequestContentSerializedCorrectly()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var expected = "null";
                server.AddResponse(new LocalWebServerResponseInfo());

                //arrange
                var builder = HttpCallBuilder<EmptyResult, TestResult, EmptyError>.Create(TestUriString).AsPost();
                string actual = null;
                server.InspectRequest(r => actual = r.Body);

                //act
                builder.WithContent(() => null).SendAsync().Wait();

                Assert.AreEqual(expected, actual);
            }
        }

        #endregion

        #region Errors

        [Test]
        public async Task WhenCallFailsAndErrorIsComplexType_ExpectRequestContentSerializedCorrectly()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = TestResultString
                });
                var builder = HttpCallBuilder<EmptyResult, EmptyRequest, TestResult>.Create(TestUriString);

                //act
                try
                {
                    await builder.ResultAsync();
                    Assert.Fail();
                }
                catch (HttpErrorException<TestResult> ex)
                {
                    Assert.AreEqual(TestResultValue, ex.Error);
                } 
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsSimpleTyped_ExpectExceptionWithCorrectlyDeserializedError()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = "true"
                });
                var builder = HttpCallBuilder<EmptyResult, EmptyRequest, bool>.Create(TestUriString);

                //act
                try
                {
                    await builder.ResultAsync();
                    Assert.Fail();
                }
                catch (HttpErrorException<bool> ex)
                {
                    Assert.IsTrue(ex.Error);
                }
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsStringType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var expected = "some string data";
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = "\"" + expected + "\""
                });
                var builder = HttpCallBuilder<EmptyResult, EmptyRequest, string>.Create(TestUriString);

                //act
                try
                {
                    await builder.ResultAsync();
                    Assert.Fail();
                }
                catch (HttpErrorException<string> ex)
                {
                    Assert.AreEqual(expected, ex.Error);
                }
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsEmptyStringType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = null
                });
                var builder = HttpCallBuilder<EmptyResult, EmptyRequest, string>.Create(TestUriString);

                //act
                try
                {
                    await builder.ResultAsync();
                    Assert.Fail();
                }
                catch (HttpErrorException<string> ex)
                {
                    Assert.IsNullOrEmpty(ex.Error);
                }
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsEmptyType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                server.AddResponse(new LocalWebServerResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = null
                });
                var builder = HttpCallBuilder<EmptyResult, EmptyRequest, TestResult>.Create(TestUriString);

                //act
                try
                {
                    await builder.ResultAsync();
                    Assert.Fail();
                }
                catch (HttpErrorException<TestResult> ex)
                {
                    Assert.IsNull(ex.Error);
                }
            }
        }

        #endregion
    }
}
