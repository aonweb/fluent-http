using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Performance
{
    [Collection("LocalWebServer Tests")]
    public class SpeedTest
    {
        private readonly ITestOutputHelper _logger;

        public SpeedTest(ITestOutputHelper logger)
        {
            this._logger = logger;
        }

        [Fact]
        public async Task LessThan15PercentOverhead()
        {
            Defaults.Caching.Enabled = false;

            const int iterations = 1000;

            using (var server = LocalWebServer.ListenInBackground(new XUnitMockLogger(_logger)))
            {
                server.WithLogging(false);

                //warm up
                await HttpClientTest(server.ListeningUri, 1);
                await HttpCallBuilderTest(server.ListeningUri, 1);

                var clientElapsed = await HttpClientTest(server.ListeningUri, iterations);
                _logger.WriteLine("HttpClient Test took {0}", clientElapsed);

                var builderElapsed = await HttpCallBuilderTest(server.ListeningUri, iterations);

                _logger.WriteLine("HttpCallBuilder Test took {0}", builderElapsed);

                var percentDifference = (decimal)(builderElapsed.Ticks - clientElapsed.Ticks) / (decimal)builderElapsed.Ticks;
                _logger.WriteLine("Percent Overhead is {0:p}", percentDifference);

                percentDifference.ShouldBeLessThanOrEqualTo(0.15m);
            }
        }

        private async Task<TimeSpan> HttpClientTest(Uri listeningUri, int iterations)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                for (var i = 0; i < iterations; i++)
                {
                    HttpResponseMessage response;
                    using (var client = new HttpClient())
                    {
                        response = await client.GetAsync(listeningUri);
                    }
                    using (var client = new HttpClient())
                    {
                        response = await client.PutAsync(listeningUri, new StringContent("Content"));
                    }
                    using (var client = new HttpClient())
                    {
                        response = await client.PostAsync(listeningUri, new StringContent("Content"));
                    }
                }
                return watch.Elapsed;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private async Task<TimeSpan> HttpCallBuilderTest(Uri listeningUri, int iterations)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                for (var i = 0; i < iterations; i++)
                {

                    await new HttpBuilderFactory().Create().WithUri(listeningUri).ResultAsync();

                    await new HttpBuilderFactory().Create().WithUri(listeningUri).AsPost().WithContent("Content").ResultAsync();

                    await new HttpBuilderFactory().Create().WithUri(listeningUri).AsPut().WithContent("Content").ResultAsync();
                }

                return watch.Elapsed;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}