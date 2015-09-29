using System;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests
{
    public class AdvancedHalBuilderExtensionsTests
    {
        private static readonly Uri MockUri = new Uri("http://testsite.com");

        public AdvancedHalBuilderExtensionsTests()
        {
            Defaults.Caching.Enabled = false;
            Cache.Clear();
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WhenResultAndSendingHandlerTypesMismatch_ExpectException()
        {

            var builder = new MockHalBuilderFactory().Create().WithLink(MockUri);

            //act
            await builder.Advanced.OnSendingWithResult<AlternateTestResource>(ctx => { }).ResultAsync<TestResource>();
        }

        [Fact]
        public async Task WhenResultAndSendingTypesMismatchAndSuppressTypeException_ExpectResult()
        {

            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            //act
            var actual = await builder.Advanced.WithSuppressTypeMismatchExceptions().OnSendingWithResult<AlternateTestResource>(ctx => { }).ResultAsync<TestResource>();

            actual.ShouldNotBeNull();
        }

        [Fact]
        public async Task WhenContentAndSendingHandlerTypesMismatch_ExpectException()
        {

            var builder = new MockHalBuilderFactory().Create()
                .VerifyOnSendingWithContent<AlternateTestRequest>(ctx =>
                {
                    var content = ctx.Content;
                })
                .WithLink(MockUri)
                .WithContent(TestRequest.Default1())
                .AsPost();

            //act
            await Should.ThrowAsync<TypeMismatchException>(async () =>
                await builder.ResultAsync<TestResource>());
        }

        [Fact]
        public async Task WhenContentAndSendingHandlerTypesMismatchAndSuppressTypeException_ExpectResult()
        {

            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            //act
            var actual = await builder.WithContent(TestRequest.Default1())
                .AsPost()
                .Advanced
                .WithSuppressTypeMismatchExceptions().OnSendingWithContent<AlternateTestRequest>(ctx => { }).ResultAsync<TestResource>();

            actual.ShouldNotBeNull();
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WhenErrorAndErrorHandlerTypesMismatch_ExpectException()
        {

            var builder = new MockHalBuilderFactory().Create().WithError(TestResource.Default1()).WithLink(MockUri);

            // act & assert
            await Should.ThrowAsync<TypeMismatchException>(
                    builder.WithErrorType<TestResource>().Advanced.OnError<AlternateTestResource>(ctx =>
                    {
                        var error = ctx.Error;
                    }).ResultAsync<TestResource>());

        }

        [Fact]
        public async Task WhenErrorAndErrorHandlerTypesMismatchAndSuppressTypeException_ExpectResult()
        {

            var builder = new MockHalBuilderFactory().Create().WithError(TestResource.Default1()).WithLink(MockUri);

            //act
            var actual = await builder
                .WithErrorType<TestResource>()
                .Advanced
                .WithExceptionFactory(context => null)
                .WithSuppressTypeMismatchExceptions()
                .OnError<AlternateTestResource>(ctx => { })
                .ResultAsync<TestResource>();

            actual.ShouldBeNull();
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WhenDefaultResultAndResultTypesMismatch_ExpectException()
        {

            var builder = new MockHalBuilderFactory().Create().WithError(TestResource.Default1()).WithLink(MockUri);

            //act
            await Should.ThrowAsync<TypeMismatchException>(
                builder.WithDefaultResult(TestResource.Default1())
                    .Advanced.WithExceptionFactory(context => null)
                    .ResultAsync<AlternateTestResource>()
                );
        }

        [Fact]
        public async Task WhenDefaultResultAndResultTypesMismatchAndSuppressTypeException_ExpectNullResult()
        {

            var builder = new MockHalBuilderFactory().Create().WithError(TestResource.Default1()).WithLink(MockUri);

            //act
            var actual = await builder.WithDefaultResult(TestResource.Default1())
                 .Advanced.WithSuppressTypeMismatchExceptions()
                 .WithExceptionFactory(context => null)
                 .ResultAsync<AlternateTestResource>();

            actual.ShouldBeNull();
        }

        [Fact]
        public async Task WhenHandlerIsSubTypeOfResult_ExpectSuccess()
        {

            var builder = new MockHalBuilderFactory().Create().WithNextResponseOk(TestResource.SerializedDefault1).WithLink(MockUri);

            //act
            var called = false;
            var result = await builder
                .Advanced
                .OnResult<TestResource>(ctx => { called = true; })
                .ResultAsync<SubTestResource>();

            result.ShouldNotBeNull();
            called.ShouldBeTrue();
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WhenHandlerIsSuperTypeOfResult_ExpectException()
        {
            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            // act & assert
            await Should.ThrowAsync<TypeMismatchException>(async () =>
            {

                var actual = await builder
                    .Advanced
                    .OnResult<SubTestResource>(ctx =>
                    {
                        var r = ctx.Result;
                    })
                    .ResultAsync<TestResource>();

                actual.ShouldBeNull();
            });
        }

        [Fact]
        public async Task WhenSendingContentHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            //act
            var called = false;
            var result = await builder.WithContent(TestRequest.Default1())
                .AsPost()
                .Advanced
                .OnSendingWithContent<IHalRequest>(ctx => { called = true; })
                .ResultAsync<TestResource>();

            result.ShouldNotBeNull();
            called.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenSendingResultHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            //act
            var called = false;
            var result = await builder.WithContent(TestRequest.Default1())
                .AsPost()
                .Advanced
                .OnSendingWithResult<IHalResource>(ctx => { called = true; })
                .ResultAsync<TestResource>();

            result.ShouldNotBeNull();
            called.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenSentHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            //act
            var called = false;
            var result = await builder.WithContent(TestRequest.Default1())
                .AsPost()
                .Advanced
                .OnSent<IHalResource>(ctx => { called = true; })
                .ResultAsync<TestResource>();

            result.ShouldNotBeNull();
            called.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenResultHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalBuilderFactory().Create().WithResult(TestResource.Default1()).WithLink(MockUri);

            //act
            var called = false;
            var result = await builder
                .Advanced
                .OnResult<IHalResource>(ctx => { called = true; })
                .ResultAsync<TestResource>();

            result.ShouldNotBeNull();
            called.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenErrorHandlerIsObjectType_ExpectSuccess()
        {

            var builder = new MockHalBuilderFactory().Create().WithError(TestResource.Default1()).WithLink(MockUri);

            //act
            var called = false;
            var result = await builder
                .WithErrorType<TestResource>()
                .Advanced
                .OnError<object>(ctx =>
                {
                    called = true;
                })
                .WithExceptionFactory(context => null)
                .ResultAsync<SubTestResource>();

            result.ShouldBeNull();
            called.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenErrorTypeSetMultipleTimes_ExpectLastWins()
        {

            var builder = new MockHalBuilderFactory().Create().WithError(TestResource.Default1()).WithLink(MockUri);

            //act
            Type type = null;
            TestResource error = null;
            var result = await builder
                .WithErrorType<AlternateTestResource>()
                .WithErrorType<string>()
                .WithErrorType<TestResource>()
                .Advanced
                .OnError<object>(ctx =>
                {
                    type = ctx.ErrorType;
                    error = ctx.Error as TestResource;
                })
                .WithExceptionFactory(context => null)
                .ResultAsync<TestResource>();

            error.ShouldNotBeNull();
            result.ShouldBeNull();
            typeof(TestResource).ShouldBe(type);
        }

        [Fact]
        public async Task OnSending_CanUseSmallerType()
        {
            //arrange
            var sendingCalled = false;
            var sendingWithContentCalled = false;
            var sendingWithResultCalled = false;

            var builder =
                new MockHalBuilderFactory().Create()
                .VerifyOnSending<IHalResource, IHalRequest>(ctx => { })
                    .VerifyOnSendingWithContent<IHalRequest>(ctx => { })
                    .VerifyOnSendingWithResult<IHalResource>(ctx => { })
                    .WithResult(TestResource.Default1())
                    .WithLink(MockUri)
                    .WithContent(TestRequest.Default1())
                    .Advanced.OnSending<IHalResource, IHalRequest>(ctx => sendingCalled = true)
                    .OnSendingWithContent<IHalRequest>(ctx => sendingWithContentCalled = true)
                    .OnSendingWithResult<IHalResource>(ctx => sendingWithResultCalled = true);

            //act
            var actual1 = await builder.ResultAsync<TestResource>();

            sendingCalled.ShouldBeTrue("Sending was not called");
            sendingWithContentCalled.ShouldBeTrue("SendingWithContent was not called");
            sendingWithResultCalled.ShouldBeTrue("SendingWithResult was not called");
        }

        [Fact]
        public async Task OnSending_CanUseSmallerType2()
        {
            //arrange
            var sendingCalled = false;
            var sendingWithContentCalled = false;
            var sendingWithResultCalled = false;

            var builder =
                new MockHalBuilderFactory().Create()
                .VerifyOnSending<IHalResource, IHalRequest>(ctx => { })
                    .VerifyOnSendingWithContent<IHalRequest>(ctx => { })
                    .VerifyOnSendingWithResult<IHalResource>(ctx => { })
                    .WithResult(TestResource.Default1())
                    .WithLink(MockUri)
                    .WithContent(TestRequest.Default1())
                    .Advanced.OnSending<IHalResource, IHalRequest>(ctx => sendingCalled = true)
                    .OnSendingWithContent<IHalRequest>(ctx => sendingWithContentCalled = true)
                    .OnSendingWithResult<IHalResource>(ctx => sendingWithResultCalled = true);

            //act
            var actual1 = await builder.ResultAsync<TestResource>();

            sendingCalled.ShouldBeTrue("Sending was not called");
            sendingWithContentCalled.ShouldBeTrue("SendingWithContent was not called");
            sendingWithResultCalled.ShouldBeTrue("SendingWithResult was not called");
        }
    }
}