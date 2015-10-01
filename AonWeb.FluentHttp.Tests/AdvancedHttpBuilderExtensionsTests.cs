using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests
{
    [Collection("LocalWebServer Tests")]
    public class AdvancedHttpBuilderExtensionsTests
    {
        private readonly ITestOutputHelper _logger;

        public AdvancedHttpBuilderExtensionsTests(ITestOutputHelper logger)
        {
            _logger = logger;
            Defaults.Caching.Enabled = false;
            Cache.Clear();
        }


        #region Client Configuration

        [Fact]
        public async Task WithClientConfiguration_WhenAction_ExpectConfigurationApplied()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var expected = "GoogleBot/1.0";
                string actual = null;
                server.WithRequestInspector(r => actual = r.Headers.UserAgent.First().ToString());

                //act
                await new HttpBuilderFactory().Create().WithUri(server.ListeningUri)
                    .Advanced
                    .WithClientConfiguration(b =>
                        b.WithHeadersConfiguration(h =>
                            h.UserAgent.Add(new ProductInfoHeaderValue("GoogleBot", "1.0"))))
                        .ResultAsync();

                actual.ShouldBe(expected);

            }
        }

        #endregion

        #region Timeout & Cancellation

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task CancelRequest_WhenSuppressCancelOff_ExpectException()
        {
            //arrange
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                var delay = 500;
                var builder = new HttpBuilderFactory().Create().WithUri(uri);
                server.WithRequestInspector(r => Task.Delay(delay));
                Exception exception = null;

                // act
                try
                {
                    var task = builder.Advanced.WithSuppressCancellationExceptions(false).ResultAsync();

                    builder.CancelRequest();

                    await task;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    exception.ShouldNotBeNull();
                    exception.ShouldBeOfType<TaskCanceledException>();
                }
            }
        }

        [Fact]
        public async Task WithTimeout_WithLongCall_ExpectTimeoutBeforeCompletionWithNoException()
        {
            //arrange
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                var delay = 1000;
                var builder = new HttpBuilderFactory().Create().WithUri(uri).Advanced.WithSuppressCancellationExceptions(true);
                server.WithRequestInspector(r => Task.Delay(delay));

                // act
                var watch = new Stopwatch();
                watch.Start();
                var response = await builder.Advanced.WithTimeout(TimeSpan.FromMilliseconds(100)).ResultAsync();

                // assert
                response.ShouldBeNull();

                watch.ElapsedMilliseconds.ShouldBeGreaterThanOrEqualTo(100);
                watch.ElapsedMilliseconds.ShouldBeLessThan(delay);
            }
        }

        [Fact]
        // await Should.ThrowAsync<TypeMismatchException>(async () => );
        public async Task WithTimeout_WithLongCallAndSuppressCancelFalse_ExpectException()
        {
            //arrange
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                var delay = 10000;
                var builder = new HttpBuilderFactory().Create().WithUri(uri);
                server.WithRequestInspector(r => Task.Delay(delay));
                Exception exception = null;

                // act
                try
                {
                    await
                        builder.Advanced.WithTimeout(TimeSpan.FromMilliseconds(100))
                            .WithSuppressCancellationExceptions(false)
                            .ResultAsync();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    exception.ShouldNotBeNull();
                    exception.ShouldBeOfType<TaskCanceledException>();
                }
            }
        }

        [Fact]
        public async Task WithTimeout_WithLongCallAndExceptionHandler_ExpectExceptionHandlerCalled()
        {
            //arrange
            
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var uri = server.ListeningUri;
                var delay = 1000;
                var builder = new HttpBuilderFactory().Create().WithUri(uri).Advanced.WithSuppressCancellationExceptions(false);
                server.WithRequestInspector(r => Task.Delay(delay));
                var callbackCalled = false;
                Exception exception = null;

                // act
                try
                {
                    await builder.Advanced.WithTimeout(TimeSpan.FromMilliseconds(100)).OnException(ctx =>
                    {
                        callbackCalled = true;
                    }).ResultAsync();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    exception.ShouldNotBeNull();
                    exception.ShouldBeOfType<TaskCanceledException>();
                }

                callbackCalled.ShouldBeTrue();
            }
        }

        #endregion

        #region DependentUri

        [Fact]
        public async Task WhenDependentUriIsNull_ExpectNoException()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var builder =
                    new MockHttpBuilderFactory().Create()
                        .WithNextResponse(new MockHttpResponseMessage())
                        .WithUri(server.ListeningUri)
                        .Advanced;

                //act
                var result = await builder
                    .WithDependentUri(null)
                    .ResultAsync();

                result.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task WhenDependentUrisIsNull_ExpectNoException()
        {
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                var builder =
                    new MockHttpBuilderFactory().Create()
                        .WithNextResponse(new MockHttpResponseMessage())
                        .WithUri(server.ListeningUri)
                        .Advanced;

                //act
                var result = await builder
                    .WithDependentUris(null)
                    .ResultAsync();

                result.ShouldNotBeNull();
            }
        }

        #endregion

        /*
         
        
        IHttpCallBuilder WithEncoding(Encoding encoding);
        IHttpCallBuilder WithMediaType(string mediaType);

        IHttpCallBuilder WithClientConfiguration(Action<IHttpClient> configuration);
        IHttpCallBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration);
        IHttpCallBuilder ConfigureRetries(Action<RetryHandler> configuration);
        IHttpCallBuilder ConfigureRedirect(Action<RedirectHandler> configuration);

        IHttpCallBuilder WithHandler(IHttpCallHandler handler);
        IHttpCallBuilder WithHandlerConfiguration<THandler>(Action<THandler> configure) 
            where THandler : class, IHttpCallHandler;
        IHttpCallBuilder WithSuccessfulResponseValidator(Func<HttpResponseMessage, bool> validator);
        IHttpCallBuilder WithExceptionFactory(Func<HttpResponseMessage, Exception> factory);
        IHttpCallBuilder WithNoCache();

        IHttpCallBuilder OnSending(Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(HandlerPriority priority, Action<HttpSendingContext> handler);
        IHttpCallBuilder OnSending(Func<HttpSendingContext, Task> handler);
        IHttpCallBuilder OnSending(HandlerPriority priority, Func<HttpSendingContext, Task> handler);

        IHttpCallBuilder OnSent(Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(HandlerPriority priority, Action<HttpSentContext> handler);
        IHttpCallBuilder OnSent(Func<HttpSentContext, Task> handler);
        IHttpCallBuilder OnSent(HandlerPriority priority, Func<HttpSentContext, Task> handler);

        IHttpCallBuilder OnException(Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(HandlerPriority priority, Action<HttpExceptionContext> handler);
        IHttpCallBuilder OnException(Func<HttpExceptionContext, Task> handler);
        IHttpCallBuilder OnException(HandlerPriority priority, Func<HttpExceptionContext, Task> handler);
         */
    }
}