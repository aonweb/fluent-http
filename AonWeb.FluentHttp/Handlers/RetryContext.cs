using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class RetryContext
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public HttpRequestMessage RequestMessage { get; internal set; }
        public Uri Uri { get; internal set; }
        public bool ShouldRetry { get; set; }
        public TimeSpan RetryAfter { get; internal set; }
        public int CurrentRetryCount { get; internal set; }
    }
}