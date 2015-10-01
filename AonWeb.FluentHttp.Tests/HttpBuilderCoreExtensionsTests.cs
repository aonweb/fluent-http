using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Mocks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests
{
    public class HttpBuilderCoreExtensionsTests
    {
        private const string MockUriString = "http://testsite.com";
        private readonly ITestOutputHelper _logger;

        public HttpBuilderCoreExtensionsTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Caching.Enabled = false;
            Cache.Clear();
        }

        #region WithUri

        [Fact]
        public async Task WithUri_WhenValidString_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.ResultAsync();

            // assert
            uri.ShouldBe(actual);
        }

        [Fact]
        public void WithUri_WhenInvalidString_ExpectException()
        {
            //arrange
            var uri = "blah blah";
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<UriFormatException>(() => builder.WithUri(uri));
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public void WithUri_WhenNullString_ExpectException()
        {
            //arrange
            string uri = null;
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<ArgumentException>(() => builder.WithUri(uri));
        }

        [Fact]
        public void WithUri_WhenEmptyString_ExpectException()
        {
            //arrange
            var uri = string.Empty;
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<ArgumentException>(() => builder.WithUri(uri));
        }

        [Fact]
        public async Task WithUri_WhenValidUri_ExpectResultUsesUri()
        {
            //arrange
            var uri = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.ResultAsync();

            // assert
            uri.ShouldBe(actual);
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public void WithUri_WhenNullUri_ExpectException()
        {

            Uri uri = null;
            Should.Throw<ArgumentNullException>(() => new MockHttpBuilderFactory().Create().WithUri(uri));
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WithUri_WhenNeverSet_ExpectException()
        {
            await Should.ThrowAsync<InvalidOperationException>(new MockHttpBuilderFactory().Create().ResultAsync());
        }

        [Fact]
        public void WithUri_WhenUriIsNotAbsolute_ExpectException()
        {
            //arrange
            var uri = "somedomain.com/path";
            var builder = new MockHttpBuilderFactory().Create();

            //act
            Should.Throw<UriFormatException>(() => builder.WithUri(uri));
        }

        [Fact]
        public async Task WithUri_WhenCalledMultipleTimes_ExpectLastWins()
        {
            //arrange
            var uri1 = new Uri("http://yahoo.com");
            var uri2 = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri1).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithUri(uri2).ResultAsync();

            // assert
            uri2.ShouldBe(actual);

        }

        #endregion

        #region WithQuerystring

        [Fact]
        public async Task WithQueryString_WhenValidValues_ExpectResultUsesUriAndQuerystring()
        {
            //arrange
            var uri = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.ResultAsync();

            // assert
            uri.ShouldBe(actual);
        }

        [Theory]
        [InlineData("q", "1", MockUriString + "?q=1")]
        [InlineData("q", "1 and 2", MockUriString + "?q=1%20and%202")]
        [InlineData("q", null, MockUriString + "?q=")]
        [InlineData("q", "", MockUriString + "?q=")]
        [InlineData(null, "1", MockUriString)]
        public async Task WithQueryString_WhenValidValues_ExpectResultQuerystring(string key, string value, string expectedUriString)
        {
            //arrange
            var uri = new Uri(MockUriString);
            var expected = new Uri(expectedUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(key, value).ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WithMultipleKeys_ExpectResultQuerystring()
        {
            var uri = new Uri(MockUriString + "?q1=1");

            var values = new Dictionary<string, string> { { "q1", "2" }, { "q2", "1 and 2" }, { "q3", "3" } };
            var expected = new Uri(MockUriString + "?q1=2&q2=1%20and%202&q3=3");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(values).ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }
        [Fact]
        public async Task WithQueryString_WithNullValues_ExpectSameUri()
        {
            var expected = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(expected).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(null).ResultAsync();

            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WithNoValues_ExpectSameUri()
        {
            var expected = new Uri(MockUriString);

            var builder = new MockHttpBuilderFactory().Create().WithUri(expected).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString(new Dictionary<string, string>()).ResultAsync();
            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            var result = await builder.WithQueryString(new Dictionary<string, string> { { "q", "2" } }).ResultAsync();

            // assert
            actual.ShouldBe(expected);

        }

        [Fact]
        public async Task WithQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString("q", "2").ResultAsync();
            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithAppendQueryString_WhenUsingCollectionAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=1&q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            var result = await builder.WithAppendQueryString(new Dictionary<string, string> { { "q", "2" } }).ResultAsync();

            // assert
            actual.ShouldBe(expected);

        }

        [Fact]
        public async Task WithAppendQueryString_WhenUsingNameAndValueAndValueAlreadyExists_ExpectValueReplaced()
        {
            var uri = new Uri(MockUriString + "?q=1");
            var expected = new Uri(MockUriString + "?q=1&q=2");

            var builder = new MockHttpBuilderFactory().Create().WithUri(uri).Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithAppendQueryString("q", "2").ResultAsync();
            // assert
            actual.ShouldBe(expected);
        }

        [Fact]
        public async Task WithQueryString_WithUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString("q2", "1").WithUri(expected).ResultAsync();
            // assert
            actual.ShouldBe(expected);

        }

        [Fact]
        public async Task WithQueryString_WithBaseUriSetAfter_ExpectValueReplaced()
        {
            var expected = new Uri(MockUriString + "?q=2");

            var builder = new MockHttpBuilderFactory().Create().Advanced;
            Uri actual = null;
            builder.OnSending(ctx => actual = ctx.Request.RequestUri);

            // act
            await builder.WithQueryString("q2", "1").WithBaseUri(expected).ResultAsync();
            // assert
            actual.ShouldBe(expected);

        }

        #endregion
        // IHttpCallBuilder WithPath(string absolutePathAndQuery);
    }
}