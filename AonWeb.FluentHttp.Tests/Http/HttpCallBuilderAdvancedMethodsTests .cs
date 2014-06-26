using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Http
{
    [TestFixture]
    public class HttpCallBuilderAdvancedMethodTests
    {
        #region Declarations, Set up, & Tear Down

        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;
        }

        #endregion

        #region WithMethod

        [Test]
        public void WithMethod_WhenValidString_ExpectResultUsesMethod()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var method = "GET";
                var builder = HttpCallBuilder.Create().WithUri(TestUriString).Advanced.WithMethod(method);

                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                //act
                builder.ResultAsync().Wait();

                Assert.AreEqual(method, actual);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenNullString_ExpectException()
        {
            //arrange
            string method = null;
            var builder = HttpCallBuilder.Create();

            //act
            builder.Advanced.WithMethod(method);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMethod_WhenEmptyString_ExpectException()
        {
            //arrange
            var method = string.Empty;
            var builder = HttpCallBuilder.Create();

            //act
            builder.Advanced.WithMethod(method);
        }

        [Test]
        public void WithMethod_WhenValidMethod_ExpectResultUsesMethod()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var method = HttpMethod.Get;
                var builder = HttpCallBuilder.Create().WithUri(TestUriString).Advanced.WithMethod(method);

                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                //act
                builder.ResultAsync().Wait();

                Assert.AreEqual(method.Method, actual);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithMethod_WhenNullMethod_ExpectException()
        {
            //arrange
            HttpMethod method = null;
            var builder = HttpCallBuilder.Create().Advanced.WithMethod(method);

            //act
            builder.ResultAsync();

            Assert.Fail();
        }

        [Test]
        public void WithMethod_WhenCalledMultipleTimes_ExpectLastWins()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                //arrange
                var method1 = "POST";
                var method2 = "GET";
                var builder = HttpCallBuilder.Create().WithUri(TestUriString).Advanced.WithMethod(method1).Advanced.WithMethod(method2);

                string actual = null;
                server.InspectRequest(r => actual = r.HttpMethod);

                //act
                builder.ResultAsync().Wait();

                Assert.AreEqual(method2, actual);
            }
        }

        #endregion

        #region Client Configuration

        [Test]
        public void WithClientConfiguration_WhenAction_ExpectConfigurationApplied()
        {
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                var expected = "GoogleBot/1.0";
                string actual = null;
                server.InspectRequest(r => actual = r.Headers["User-Agent"]);

                //act
                HttpCallBuilder.Create(TestUriString)
                    .Advanced
                    .ConfigureClient(b =>
                        b.WithHeaders(h =>
                            h.UserAgent.Add(new ProductInfoHeaderValue("GoogleBot", "1.0"))))
                        .ResultAsync().Wait();

                Assert.AreEqual(expected, actual);

            }
        }

        #endregion

        #region Timeout & Cancellation

        [Test]
        [ExpectedException(typeof(AggregateException))]
        public void CancelRequest_WhenSuppressCancelOff_ExpectException()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var delay = 500;
                var builder = HttpCallBuilder.Create().WithUri(uri);
                server.InspectRequest(r => Thread.Sleep(delay));

                // act
                var watch = new Stopwatch();
                watch.Start();
                var task = builder.Advanced.WithSuppressCancellationExceptions(false).ResultAsync();

                builder.CancelRequest();

                Task.WaitAll(task);
            }
        }

        [Test]
        public async Task WithTimeout_WithLongCall_ExpectTimeoutBeforeCompletionWithNoException()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var delay = 1000;
                var builder = HttpCallBuilder.Create().WithUri(uri);
                server.InspectRequest(r => Thread.Sleep(delay));

                // act
                var watch = new Stopwatch();
                watch.Start();
                var result = await builder.Advanced.WithTimeout(TimeSpan.FromMilliseconds(100)).ResultAsync();

                // assert
                Assert.IsNull(result);
                Assert.GreaterOrEqual(watch.ElapsedMilliseconds, 100);
                Assert.Less(watch.ElapsedMilliseconds, delay);
            }
        }

        [Test]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task WithTimeout_WithLongCallAndSuppressCancelFalse_ExpectException()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var delay = 10000;
                var builder = HttpCallBuilder.Create().WithUri(uri);
                server.InspectRequest(r => Thread.Sleep(delay));

                // act
                await builder.Advanced.WithTimeout(TimeSpan.FromMilliseconds(100)).WithSuppressCancellationExceptions(false).ResultAsync();
            }
        }

        [Test]
        public async Task WithTimeout_WithLongCallAndExceptionHandler_ExpectExceptionHandlerCalled()
        {
            //arrange
            var uri = TestUriString;
            using (var server = LocalWebServer.ListenInBackground(uri))
            {
                var delay = 1000;
                var builder = HttpCallBuilder.Create().WithUri(uri);
                server.InspectRequest(r => Thread.Sleep(delay));
                var callbackCalled = false;
                // act
                await builder.Advanced.WithTimeout(TimeSpan.FromMilliseconds(100)).OnException(ctx =>
                {
                    callbackCalled = true;
                }).ResultAsync();

                Assert.IsTrue(callbackCalled);
            }
        }

        #endregion

        [Test]
        public async Task WhenDependentUriIsNull_ExpectNoException()
        {

            var builder = new MockHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            //act
            var result = await builder
                .WithDependentUri(null)
                .ResultAsync();

            Assert.NotNull(result);
        }

        [Test]
        public async Task WhenDependentUrisIsNull_ExpectNoException()
        {

            var builder = new MockHttpCallBuilder().WithResponse(new ResponseInfo()).WithUri(TestUriString).Advanced;

            //act
            var result = await builder
                .WithDependentUris(null)
                .ResultAsync();

            Assert.NotNull(result);
        }

        /*
         IHttpCallBuilder WithScheme(string scheme);
        IHttpCallBuilder WithHost(string host);
        IHttpCallBuilder WithPort(int port);
        IHttpCallBuilder WithPath(string absolutePathAndQuery);
        IHttpCallBuilder WithEncoding(Encoding encoding);
        IHttpCallBuilder WithMediaType(string mediaType);

        IHttpCallBuilder ConfigureClient(Action<IHttpClient> configuration);
        IHttpCallBuilder ConfigureClient(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);

        IHttpCallBuilder WithHandler(IHttpCallHandler handler);
        IHttpCallBuilder ConfigureHandler<THandler>(Action<THandler> configure) 
            where THandler : class, IHttpCallHandler;
        IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IHttpCallBuilder WithNoCache();

        IHttpCallBuilder OnSending(Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler);
        IHttpCallBuilder OnSending(HttpCallHandlerPriority priority, Func<HttpSendingContext, Task> handler);

        IHttpCallBuilder OnSent(Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler);
        IHttpCallBuilder OnSent(HttpCallHandlerPriority priority, Func<HttpSentContext, Task> handler);

        IHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler);
        IHttpCallBuilder OnException(HttpCallHandlerPriority priority, Func<HttpExceptionContext, Task> handler);
         */
    }
}
