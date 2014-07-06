using System;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;

using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.HAL
{
    public class AdvancedHalCallBuilderTests
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
        public static TestRequest TestRequestValue = new TestRequest();

        public class TestResult2 : HalResource { }

        public class TestRequest2 : HalRequest { }

        public class TestRequest : HalRequest,IEquatable<TestRequest>
        {
            public TestRequest()
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

            public bool Equals(TestRequest other)
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
                return Equals((TestRequest)obj);
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

            public static bool operator ==(TestRequest left, TestRequest right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TestRequest left, TestRequest right)
            {
                return !Equals(left, right);
            }

            #endregion
        }

        public class TestResult : HalResource, IEquatable<TestResult> {
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

        public class SubTestResult : TestResult {
            public bool SubBoolProperty { get; set; }
        }

        #endregion

        #region Handlers

        [Test]
        [ExpectedException(typeof(TypeMismatchException))]
        public async Task WhenResultAndSendingHandlerTypesMismatch_ExpectException()
        {

            var builder = HalCallBuilder.Create().WithLink(TestUriString);

            //act
            await builder.Advanced.OnSendingWithResult<TestResult2>(ctx => { }).ResultAsync<TestResult>();

            Assert.Fail();
        }

        [Test]
        public async Task WhenResultAndSendingTypesMismatchAndSuppressTypeException_ExpectResult()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var actual = await builder.Advanced.WithSuppressTypeMismatchExceptions().OnSendingWithResult<TestResult2>(ctx => { }).ResultAsync<TestResult>();

            Assert.NotNull(actual);
        }

        [Test]
        [ExpectedException(typeof(TypeMismatchException))]
        public async Task WhenContentAndSendingHandlerTypesMismatch_ExpectException()
        {

            var builder = HalCallBuilder.Create().WithLink(TestUriString);

            //act
            await builder.WithContent(TestRequestValue).AsPost().Advanced.OnSendingWithContent<TestRequest2>(ctx => { }).ResultAsync<TestResult>();

            Assert.Fail();
        }

        [Test]
        public async Task WhenContentAndSendingHandlerTypesMismatchAndSuppressTypeException_ExpectResult()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var actual = await builder.WithContent(TestRequestValue)
                .AsPost()
                .Advanced
                .WithSuppressTypeMismatchExceptions().OnSendingWithContent<TestRequest2>(ctx => { }).ResultAsync<TestResult>();

            Assert.NotNull(actual);
        }

        [Test]
        [ExpectedException(typeof(TypeMismatchException))]
        public async Task WhenErrorAndErrorHandlerTypesMismatch_ExpectException()
        {

            var builder = new MockHalCallBuilder().WithError(TestResultValue).WithLink(TestUriString);

            //act
            await builder.WithErrorType<TestResult>().Advanced.OnError<TestResult2>(ctx => { }).ResultAsync<TestResult>();

            Assert.Fail();
        }

        [Test]
        public async Task WhenErrorAndErrorHandlerTypesMismatchAndSuppressTypeException_ExpectResult()
        {

            var builder = new MockHalCallBuilder().WithError(TestResultValue).WithLink(TestUriString);

            //act
            var actual = await builder
                .WithErrorType<TestResult>()
                .Advanced
                .WithExceptionFactory(context => null)
                .WithSuppressTypeMismatchExceptions()
                .OnError<TestResult2>(ctx => { })
                .ResultAsync<TestResult>();

            Assert.IsNull(actual);
        }

        [Test]
        [ExpectedException(typeof(TypeMismatchException))]
        public async Task WhenDefaultResultAndResultTypesMismatch_ExpectException()
        {

            var builder = new MockHalCallBuilder().WithError(TestResultValue).WithLink(TestUriString);

            //act
            await builder.WithDefaultResult(TestResultValue).Advanced.WithExceptionFactory(context => null).ResultAsync<TestResult2>();

            Assert.Fail();
        }

        [Test]
        public async Task WhenDefaultResultAndResultTypesMismatchAndSuppressTypeException_ExpectNullResult()
        {

            var builder = new MockHalCallBuilder().WithError(TestResultValue).WithLink(TestUriString);

            //act
            var actual = await builder.WithDefaultResult(TestResultValue)
                 .Advanced.WithSuppressTypeMismatchExceptions()
                 .WithExceptionFactory(context => null)
                 .ResultAsync<TestResult2>();

            Assert.IsNull(actual);
        }

        [Test]
        public async Task WhenHandlerIsSubTypeOfResult_ExpectSuccess()
        {

            var builder = new MockHalCallBuilder().WithResponse(new ResponseInfo { Body = TestResultString }).WithLink(TestUriString);

            //act
            var called = false;
            var result = await builder
                .Advanced
                .OnResult<TestResult>(ctx => { called = true; })
                .ResultAsync<SubTestResult>();

            Assert.NotNull(result);
            Assert.IsTrue(called);
        }

        [Test]
        [ExpectedException(typeof(TypeMismatchException))]
        public async Task WhenHandlerIsSuperTypeOfResult_ExpectException()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var actual = await builder
                .Advanced
                .OnResult<SubTestResult>(ctx => { })
                .ResultAsync<TestResult>();

            Assert.IsNull(actual);
        }

        [Test]
        public async Task WhenSendingContentHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var called = false;
            var result = await builder.WithContent(TestRequestValue)
                .AsPost()
                .Advanced
                .OnSendingWithContent<IHalRequest>(ctx => { called = true; })
                .ResultAsync<TestResult>();

            Assert.NotNull(result);
            Assert.IsTrue(called);
        }

        [Test]
        public async Task WhenSendingResultHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var called = false;
            var result = await builder.WithContent(TestRequestValue)
                .AsPost()
                .Advanced
                .OnSendingWithResult<IHalResource>(ctx => { called = true; })
                .ResultAsync<TestResult>();

            Assert.NotNull(result);
            Assert.IsTrue(called);
        }

        [Test]
        public async Task WhenSentHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var called = false;
            var result = await builder.WithContent(TestRequestValue)
                .AsPost()
                .Advanced
                .OnSent<IHalResource>(ctx => { called = true; })
                .ResultAsync<TestResult>();

            Assert.NotNull(result);
            Assert.IsTrue(called);
        }

        [Test]
        public async Task WhenResultHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalCallBuilder().WithResult(TestResultValue).WithLink(TestUriString);

            //act
            var called = false;
            var result = await builder
                .Advanced
                .OnResult<IHalResource>(ctx => { called = true; })
                .ResultAsync<TestResult>();

            Assert.NotNull(result);
            Assert.IsTrue(called);
        }

        [Test]
        public async Task WhenErrorHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalCallBuilder().WithError(TestResultValue).WithLink(TestUriString);

            //act
            var called = false;
            var result = await builder
                .Advanced
                .OnError<object>(ctx => { called = true; })
                .WithExceptionFactory(context => null)
                .ResultAsync<SubTestResult>();

            Assert.Null(result);
            Assert.IsTrue(called);
        }

        [Test]
        public async Task WhenErrorTypeSetMultipleTimes_ExpectLastWins()
        {

            var builder = new MockHalCallBuilder().WithError(TestResultValue).WithLink(TestUriString);

            //act
            Type type = null;
            TestResult error = null;
            var result = await builder
                .WithErrorType<TestResult2>()
                .WithErrorType<string>()
                .WithErrorType<TestResult>()
                .Advanced
                .OnError<object>(ctx =>
                {
                    type = ctx.ErrorType;
                    error = ctx.Error as TestResult;
                })
                .WithExceptionFactory(context => null)
                .ResultAsync<TestResult>();

            Assert.NotNull(error);
            Assert.Null(result);
            Assert.AreEqual(typeof(TestResult), type);
        }

        [Test]
        public async Task OnSending_CanUseSmallerType()
        {
            //arrange
            var sendingCalled = false;
            var sendingWithContentCalled = false;
            var sendingWithResultCalled = false;

            var builder =
                new MockHalCallBuilder()
                .VerifyOnSending<IHalResource, IHalRequest>(ctx => { })
                    .VerifyOnSendingWithContent<IHalRequest>(ctx => { })
                    .VerifyOnSendingWithResult<IHalResource>(ctx => { })
                    .WithResult(TestResultValue)
                    .WithLink(TestUriString)
                    .WithContent(TestRequestValue)
                    .Advanced.OnSending<IHalResource, IHalRequest>(ctx => sendingCalled = true)
                    .OnSendingWithContent<IHalRequest>(ctx => sendingWithContentCalled = true)
                    .OnSendingWithResult<IHalResource>(ctx => sendingWithResultCalled = true);

            //act
            var actual1 = await builder.ResultAsync<TestResult>();

            Assert.IsTrue(sendingCalled, "Sending was not called");
            Assert.IsTrue(sendingWithContentCalled, "SendingWithContent was not called");
            Assert.IsTrue(sendingWithResultCalled, "SendingWithResult was not called");
        }

        [Test]
        public async Task OnSending_CanUseSmallerType2()
        {
            //arrange
            var sendingCalled = false;
            var sendingWithContentCalled = false;
            var sendingWithResultCalled = false;

            var builder =
                new QueuedMockHalCallBuilder()
                .VerifyOnSending<IHalResource, IHalRequest>(ctx => { })
                    .VerifyOnSendingWithContent<IHalRequest>(ctx => { })
                    .VerifyOnSendingWithResult<IHalResource>(ctx => { })
                    .WithResult(TestResultValue)
                    .WithLink(TestUriString)
                    .WithContent(TestRequestValue)
                    .Advanced.OnSending<IHalResource, IHalRequest>(ctx => sendingCalled = true)
                    .OnSendingWithContent<IHalRequest>(ctx => sendingWithContentCalled = true)
                    .OnSendingWithResult<IHalResource>(ctx => sendingWithResultCalled = true);

            //act
            var actual1 = await builder.ResultAsync<TestResult>();

            Assert.IsTrue(sendingCalled, "Sending was not called");
            Assert.IsTrue(sendingWithContentCalled, "SendingWithContent was not called");
            Assert.IsTrue(sendingWithResultCalled, "SendingWithResult was not called");
        }

        #endregion 
    }
}