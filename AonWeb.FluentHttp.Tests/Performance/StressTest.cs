using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Mocks.WebServer;
using AonWeb.FluentHttp.Tests.Helpers;
using NUnit.Framework;

namespace AonWeb.FluentHttp.Tests.Performance
{
    public class StressTest
    {
        private const string TestUriString = LocalWebServer.DefaultListenerUri;
        private static Random Rng = new Random();

        [Test]
        [Ignore]
        public void LessThan15PercentOverhead()
        {

            const int iterations = 1000;
            var tasks = new List<Task>();
            using (var server = LocalWebServer.ListenInBackground(TestUriString))
            {
                server.EnableLogging = false;
                server.AddResponse(r => GetResponse());
                 var watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    tasks.Add(CreateBuilderAndCall());
                    Thread.Sleep(Rng.Next(0, 50));
                }

                Task.WaitAll(tasks.ToArray());
                Console.WriteLine("Completed in {0}", watch.Elapsed);
            }
        }

        private async Task CreateBuilderAndCall()
        {
            var path = GetPathSuffix();
            var method = GetMethod();

            var builder = HttpCallBuilder.Create(TestUriString);

            if (!string.IsNullOrWhiteSpace(path))
                builder = builder.WithRelativePath(path);

            builder = builder.Advanced.WithMethod(method);

            if (method != HttpMethod.Get && method != HttpMethod.Delete)
                builder = builder.WithContent(ResponseBody());

            await builder.ResultAsync().ConfigureAwait(false);
        }

        private LocalWebServerResponseInfo GetResponse()
        {
            return new LocalWebServerResponseInfo
            {
                Body = ResponseBody()
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

        public string ResponseBody()
        {
            var i = Rng.Next(50, 1000);
            var buffer = new byte[i];
            Rng.NextBytes(buffer);

            return Convert.ToBase64String(buffer);
        }
    }
}