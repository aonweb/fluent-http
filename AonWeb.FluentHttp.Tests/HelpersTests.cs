using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests
{
    public class TypeHelpersTests
    {
        [Fact]
        public void ValidateType_WhenTypeIsTheSame_ExpectNoExceptionReturnTrue()
        {
            var result = TestResult.Default1();

            var actual = TypeHelpers.ValidateType(result, typeof (TestResult));

            actual.ShouldBeTrue();
        }

        [Fact]
        public void ValidateType_WhenRequestedTypeIsSmaller_ExpectNoExceptionReturnTrue()
        {
            var result = new AlternateTestResult();

            var actual = TypeHelpers.ValidateType(result, typeof(TestResult));

            actual.ShouldBeTrue();
        }

        [Fact]
        public void ValidateType_WhenTypeIsLarger_ExpectException()
        {
            var result = TestResult.Default1();

            Should.Throw<TypeMismatchException>(() => TypeHelpers.ValidateType(result, typeof(AlternateTestResult)));
        }

        [Fact]
        public void ValidateType_WhenTypeAreNotSame_ExpectException()
        {
            var result = TestResult.Default1();

            Should.Throw<TypeMismatchException>(() => TypeHelpers.ValidateType(result, typeof(TestResource)));
        }

        [Fact]
        public void ValidateType_WhenTypeIsLargerAndSuppressTypeMismatch_ExpectNoExceptionReturnFalse()
        {
            var result = TestResult.Default1();

            var actual = TypeHelpers.ValidateType(result, typeof(AlternateTestResult), true);

            actual.ShouldBeFalse();
        }

        [Fact]
        public void ValidateType_WhenTypeAreNotSameAndSuppressTypeMismatch_ExpectNoExceptionReturnFalse()
        {
            var result = TestResult.Default1();

            var actual = TypeHelpers.ValidateType(result, typeof(TestResource), true);

            actual.ShouldBeFalse();
        }
    }
}