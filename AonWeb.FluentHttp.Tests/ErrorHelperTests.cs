using System;
using System.Net;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Exceptions.Helpers;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests
{
    public class ErrorHelperTests
    {
        private readonly ITestOutputHelper _logger;

        public ErrorHelperTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Fact]
        public async Task ErrorMessageLooksPretty()
        {
            try
            {
                using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
                {
                    server.WithNextResponse(HttpStatusCode.BadRequest, "Some Error");

                    await new TypedBuilderFactory().Create().WithUri(server.ListeningUri)
                        .WithErrorType<TestError>()
                        .Advanced
                        .WithErrorFactory((context, message, arg3, arg4) => Task.FromResult(new TestError { Result = "TestErrorString" }))
                        .WithExceptionFactory(context =>
                        {
                            if (context.Response != null && context.Request != null)
                                context.Response.RequestMessage = context.Request;

                            return new HttpErrorException<TestError>((TestError) context.Error, context.Response);
                        })
                        .SendAsync();

                }
            }
            catch (HttpCallException ex)
            {
                var message = ex.GetExceptionMessage();

                _logger.WriteLine(message);
            }
        }

        [Fact]
        public async Task ErrorMessageWithPostLooksPretty()
        {
            try
            {
                using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
                {
                    server.WithNextResponse(HttpStatusCode.BadRequest, "Another Error");

                    await new TypedBuilderFactory().Create().WithUri(server.ListeningUri).AsPost().WithContent(TestRequest.Default1())
                        .WithErrorType<TestError>()
                        .Advanced
                        .WithErrorFactory((context, message, arg3, arg4) => Task.FromResult(new TestError { Result = "TestErrorString" }))
                        .WithExceptionFactory(context =>
                        {
                            if (context.Response != null && context.Request != null)
                                context.Response.RequestMessage = context.Request;

                            return new HttpErrorException<TestError>((TestError)context.Error, context.Response);
                        })
                        .SendAsync();

                }
            }
            catch (HttpCallException ex)
            {
                var message = ex.GetExceptionMessage();

                _logger.WriteLine(message);
            }
        }
    }
}