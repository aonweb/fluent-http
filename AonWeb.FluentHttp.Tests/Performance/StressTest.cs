using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Mocks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Performance
{
    public class StressTest
    {
        private static readonly Random Rng = new Random();

        private readonly ITestOutputHelper _logger;

        public StressTest(ITestOutputHelper logger)
        {
            _logger = logger;
        }

        [Fact]
        public void StressStuff()
        {

            const int iterations = 1000;
            var tasks = new List<Task>();
            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithLogging(false);
                server.WithResponse(ctx=> true, ctx => GetResponse());
                 var watch = new Stopwatch();
                watch.Start();
                for (var i = 0; i < iterations; i++)
                {
                    tasks.Add(CreateBuilderAndCall(server.ListeningUri));
                    Thread.Sleep(Rng.Next(0, 50));
                }

                Task.WaitAll(tasks.ToArray());
                _logger.WriteLine("Completed {0} calls in {1}", iterations, watch.Elapsed);
            }
        }

        private async Task CreateBuilderAndCall(Uri listeningUri)
        {
            var path = GetPathSuffix();
            var method = GetMethod();

            var builder = new HttpBuilderFactory().Create().WithUri(listeningUri);

            if (!string.IsNullOrWhiteSpace(path))
                builder = builder.WithPath(path);

            builder = builder.Advanced.WithMethod(method);

            if (method != HttpMethod.Get && method != HttpMethod.Delete)
                builder = builder.WithContent(ResponseContent());

            await builder.ResultAsync().ConfigureAwait(false);
        }

        private IMockResponse GetResponse()
        {
            return new MockHttpResponseMessage
            {
                ContentString = ResponseContent()
            };
        }

        public string GetPathSuffix()
        {
            var p = Rng.Next(0, 10);

            if (p == 0)
                return string.Empty;

            return "path" + p;
        }

        public HttpMethod GetMethod()
        {
            var p = Rng.Next(1, 6);

            if (p == 4)
                return HttpMethod.Post;
            if (p == 5)
                return HttpMethod.Put;
            if (p == 6)
                return HttpMethod.Delete;
            
            return HttpMethod.Get;
        }

        public string ResponseContent()
        {
            var i = Rng.Next(50, 1000);
            var buffer = new byte[i];
            Rng.NextBytes(buffer);

            return Convert.ToBase64String(buffer);
        }
    }
}