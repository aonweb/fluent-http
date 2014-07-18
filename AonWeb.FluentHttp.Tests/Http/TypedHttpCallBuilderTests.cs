using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class TypedMockHttpCallBuilderTests
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

        #region Results

        [Test]
        public async Task WhenComplexTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {

            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK,
                    Body = TestResultString
                })
                .WithUri(TestUriString);

            //act
            var actual = await builder.ResultAsync<TestResult>();

            Assert.AreEqual(TestResultValue, actual);
        }

        [Test]
        public async Task WhenSimpleTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK,
                    Body = "true"
                })
                .WithUri(TestUriString);


            //act
            var actual = await builder.ResultAsync<bool>();

            Assert.IsTrue(actual);
        }

        [Test]
        public async Task WhenStringTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var expected = "some string data";
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK,
                    Body = "\"" + expected + "\""
                })
                .WithUri(TestUriString);

            //act
            var actual = await builder.ResultAsync<string>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WhenStringTypedGet_WithPlainTextResponseResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var expected = "some string data";

            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "text/plain",
                    StatusCode = HttpStatusCode.OK,
                    Body = expected
                })
                .WithUri(TestUriString);

            //act
            var actual = await builder.ResultAsync<string>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WhenEmptyStringTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK
                })
                .WithUri(TestUriString);

            //act
            var actual = await builder.ResultAsync<string>();

            Assert.IsNullOrEmpty(actual);
        }

        [Test]
        public async Task WhenEmptyTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.OK
                })
                .WithUri(TestUriString);

            //act
            var actual = await builder.ResultAsync<TestResult>();

            Assert.IsNull(actual);
        }

        #endregion

        #region Content

        [Test]
        public async Task WhenPostingComplexType_ExpectRequestContentSerializedCorrectly()
        {
            HttpCallBuilderDefaults.DefaultCacheStoreFactory().Clear();

            //arrange
            var expected = TestResultString;
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            string actual = null;
            builder.OnSendingWithContent<TestResult>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent(() => TestResultValue).AsPost().SendAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WhenPuttingSimpleTyped_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "true";
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            string actual = null;
            builder.OnSending<EmptyResult, bool>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent(() => true).AsPut().SendAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WhenPostingStringType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "some string data";
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            string actual = null;
            builder.OnSendingWithContent<string>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            await builder.WithContent(() => expected).AsPost().SendAsync();

            Assert.AreEqual("\"" + expected + "\"", actual);
        }

        [Test]
        public async Task WhenPostingEmptyStringType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "null";
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            string actual = null;
            builder.OnSending<EmptyResult, string>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent<string>(() => null).AsPost().SendAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WhenPostingEmptyType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "null";
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            //arrange
            string actual = null;
            builder.OnSendingWithContent<TestResult>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent<TestResult>(() => null).AsPost().SendAsync();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public async Task WhenContentSetMultipleTimes_ExpectLastContentWins()
        {
            //arrange
            var expected = "Content3";
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            string actual = null;
            builder.OnSendingWithContent<string>(async ctx => actual = await ctx.Request.Content.ReadAsAsync<string>());

            //act
            await builder.WithContent("Content1").WithContent("Content2").WithContent("Content3").AsPost().SendAsync();

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Errors

        [Test]
        public async Task WhenCallFailsAndErrorIsComplexType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = TestResultString
                })
                .WithUri(TestUriString);

            //act
            try
            {
                await builder.WithErrorType<TestResult>().ResultAsync<EmptyResult>();
                Assert.Fail();
            }
            catch (HttpErrorException<TestResult> ex)
            {
                Assert.AreEqual(TestResultValue, ex.Error);
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsSimpleTyped_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = "true"
                })
                .WithUri(TestUriString);

            //act
            try
            {
                await builder.WithErrorType<bool>().ResultAsync<EmptyResult>();
                Assert.Fail();
            }
            catch (HttpErrorException<bool> ex)
            {
                Assert.IsTrue(ex.Error);
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsStringType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var expected = "some string data";
            var builder = new MockTypedHttpCallBuilder().WithResponse(new ResponseInfo
            {
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json",
                StatusCode = HttpStatusCode.BadRequest,
                Body = "\"" + expected + "\""
            })
            .WithUri(TestUriString);

            //act
            try
            {
                await builder.WithErrorType<string>().ResultAsync<EmptyResult>();
                Assert.Fail();
            }
            catch (HttpErrorException<string> ex)
            {
                Assert.AreEqual(expected, ex.Error);
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsEmptyStringType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = null
                })
                .WithUri(TestUriString);

            //act
            try
            {
                await builder.WithErrorType<string>().ResultAsync<EmptyResult>();
                Assert.Fail();
            }
            catch (HttpErrorException<string> ex)
            {
                Assert.IsNullOrEmpty(ex.Error);
            }
        }

        [Test]
        public async Task WhenCallFailsAndErrorIsEmptyType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var builder = new MockTypedHttpCallBuilder()
                .WithResponse(new ResponseInfo
                {
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    StatusCode = HttpStatusCode.BadRequest,
                    Body = null
                })
                .WithUri(TestUriString);

            //act
            try
            {
                await builder.WithErrorType<TestResult>().ResultAsync<EmptyResult>();
                Assert.Fail();
            }
            catch (HttpErrorException<TestResult> ex)
            {
                Assert.IsNull(ex.Error);
            }
        }

        #endregion
    }
}
