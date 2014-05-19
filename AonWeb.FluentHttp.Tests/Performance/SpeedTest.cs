using System;
using System.Diagnostics;
using System.Net.Http;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Performance
{
    [TestFixture]
    public class SpeedTest
    {
        private const string TestUriString = LocalWebServer.DefaultListenerUri;

        [Test]
        [Ignore]
        public void LessThan15PercentOverhead()
        {
            HttpCallBuilderDefaults.CachingEnabled = false;

            const int iterations = 1000;

            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.EnableLogging = false;

                //warm up
                HttpClientTest(1);
                HttpCallBuilderTest(1);

                var clientElapsed = HttpClientTest(iterations);
                Console.WriteLine("HttpClient Test took {0}", clientElapsed);

                var builderElapsed = HttpCallBuilderTest(iterations);

                Console.WriteLine("HttpCallBuilder Test took {0}", builderElapsed);

                var percentDifference = (decimal)(builderElapsed.Ticks - clientElapsed.Ticks) / (decimal)builderElapsed.Ticks;
                Console.WriteLine("Percent Overhead is {0:p}", percentDifference);
                Assert.LessOrEqual(percentDifference, 0.15m, "Expected HttpCallBuilder to be less than 10% overhead");

            }
        }

        private TimeSpan HttpClientTest(int iterations)
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
                        response = client.GetAsync(TestUriString).Result;
                    }
                    using (var client = new HttpClient())
                    {
                        response = client.PutAsync(TestUriString, new StringContent("Content")).Result;
                    }
                    using (var client = new HttpClient())
                    {
                        response = client.PostAsync(TestUriString, new StringContent("Content")).Result;
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

        private TimeSpan HttpCallBuilderTest(int iterations)
        {
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                for (var i = 0; i < iterations; i++)
                {

                    var result = HttpCallBuilder.Create(TestUriString).ResultAsync().Result;

                    result = HttpCallBuilder.Create(TestUriString).AsPost().WithContent("Content").ResultAsync().Result;

                    result = HttpCallBuilder.Create(TestUriString).AsPut().WithContent("Content").ResultAsync().Result;
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