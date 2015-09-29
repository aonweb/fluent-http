using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Serialization;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;

namespace AonWeb.FluentHttp.Tests
{
    public class TypedBuilderTests
    {
        private static readonly Uri MockUri = new Uri("http://testsite.com");

        public TypedBuilderTests()
        {
            Defaults.Caching.Enabled = false;
            Cache.Clear();
        }

        #region Results

        [Fact]
        public async Task WhenComplexTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            var expected = TestResult.Default1();
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithContent(TestResult.SerializedDefault1))
                .WithUri(MockUri);

            //act
            var actual = await builder.ResultAsync<TestResult>();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenSimpleTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithContent("true"))
                .WithUri(MockUri);


            //act
            var actual = await builder.ResultAsync<bool>();

            actual.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenStringTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var expected = "some string data";
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithContent("\"" + expected + "\""))
                .WithUri(MockUri);

            //act
            var actual = await builder.ResultAsync<string>();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenStringTypedGet_WithPlainTextResponseResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var expected = "some string data";

            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("text/plain")
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithContent(expected))
                .WithUri(MockUri);

            //act
            var actual = await builder.ResultAsync<string>();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenEmptyStringTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                )
                .WithUri(MockUri);

            //act
            var actual = await builder.ResultAsync<string>();

            actual.ShouldBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task WhenEmptyTypedGet_WithValidResponse_ExpectValidDeserializedResult()
        {
            //arrange
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                )
                .WithUri(MockUri);

            //act
            var actual = await builder.ResultAsync<TestResult>();

            actual.ShouldBeNull();
        }

        #endregion

        #region Content

        [Fact]
        public async Task WhenPostingComplexType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = TestResult.SerializedDefault1;
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()).WithUri(MockUri).Advanced;

            string actual = null;
            builder.OnSendingWithContent<TestResult>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent(() => TestResult.Default1()).AsPost().SendAsync();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenPuttingSimpleTyped_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "true";
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()).WithUri(MockUri).Advanced;

            string actual = null;
            builder.OnSending<EmptyResult, bool>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent(() => true).AsPut().SendAsync();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenPostingStringType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "some string data";
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()).WithUri(MockUri).Advanced;

            string actual = null;
            builder.OnSendingWithContent<string>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            await builder.WithContent(() => expected).AsPost().SendAsync();

            actual.ShouldBe("\"" + expected + "\"");
        }

        [Fact]
        public async Task WhenPostingEmptyStringType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "null";
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()).WithUri(MockUri).Advanced;

            string actual = null;
            builder.OnSending<EmptyResult, string>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent<string>(() => null).AsPost().SendAsync();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenPostingEmptyType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = "null";
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()).WithUri(MockUri).Advanced;

            //arrange
            string actual = null;
            builder.OnSendingWithContent<TestResult>(ctx => actual = ctx.Request.Content.ReadAsStringAsync().Result);

            //act
            await builder.WithContent<TestResult>(() => null).AsPost().SendAsync();

            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenContentSetMultipleTimes_ExpectLastContentWins()
        {
            //arrange
            var expected = "Content3";
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()).WithUri(MockUri).Advanced;

            string actual = null;
            builder.OnSendingWithContent<string>(async ctx => actual = await ctx.Request.Content.ReadAsAsync<string>());

            //act
            await builder.WithContent("Content1").WithContent("Content2").WithContent("Content3").AsPost().SendAsync();

            actual.ShouldBe(expected);
        }

        #endregion

        #region Errors

        [Fact]
        public async Task WhenCallFailsAndErrorIsComplexType_ExpectRequestContentSerializedCorrectly()
        {
            //arrange
            var expected = TestResult.Default1();
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()

                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithContent(TestResult.SerializedDefault1))
                .WithUri(MockUri)
                .WithErrorType<TestResult>();

            // act
            var ex = await Should.ThrowAsync<HttpErrorException<TestResult>>(builder.ResultAsync<EmptyResult>());

            //assert
            ex.Error.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenCallFailsAndErrorIsSimpleTyped_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()

                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithContent("true"))
                .WithUri(MockUri)
                .WithErrorType<bool>();

            // act
            var ex = await Should.ThrowAsync<HttpErrorException<bool>>(builder.ResultAsync<EmptyResult>());

            //assert
            ex.Error.ShouldBeTrue();
        }

        [Fact]
        public async Task WhenCallFailsAndErrorIsStringType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var expected = "some string data";
            var builder = new MockTypedBuilderFactory().Create().WithNextResponse(new MockHttpResponseMessage()

                .WithContentEncoding(Encoding.UTF8)
                .WithContentType("application/json")
                .WithStatusCode(HttpStatusCode.BadRequest)
                .WithContent("\"" + expected + "\""))
            .WithUri(MockUri)
            .WithErrorType<string>();

            // act
            var ex = await Should.ThrowAsync<HttpErrorException<string>>(builder.ResultAsync<EmptyResult>());

            //assert
            ex.Error.ShouldBe(expected);
        }

        [Fact]
        public async Task WhenCallFailsAndErrorIsEmptyStringType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()

                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithContent(null))
                .WithUri(MockUri)
                .WithErrorType<string>();

            // act
            var ex = await Should.ThrowAsync<HttpErrorException<string>>(builder.ResultAsync<EmptyResult>());

            //assert
            ex.Error.ShouldBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task WhenCallFailsAndErrorIsEmptyType_ExpectExceptionWithCorrectlyDeserializedError()
        {
            //arrange
            var builder = new MockTypedBuilderFactory().Create()
                .WithNextResponse(new MockHttpResponseMessage()
                    .WithContentEncoding(Encoding.UTF8)
                    .WithContentType("application/json")
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithContent(null))
                .WithUri(MockUri)
                .WithErrorType<TestResult>();

            // act
            var ex = await Should.ThrowAsync<HttpErrorException<TestResult>>(builder.ResultAsync<EmptyResult>());

            //assert
            ex.Error.ShouldBeNull();
        }

        //#endregion
        #endregion 
    }
}