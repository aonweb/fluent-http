using System;
using System.Collections.Generic;
using System.Net;

namespace AonWeb.Fluent.Http.Handlers
{
    public class RetrySettings
    {
        internal const int DefaultMaxAutoRetries = 2;
        internal const int DefaultRetryAfter = 100;
        internal const int DefaultMaxRetryAfter = 5000;

        public RetrySettings()
        {
            AllowAutoRetry = true;
            MaxAutoRetries = DefaultMaxAutoRetries;
            RetryStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.ServiceUnavailable,  };
            RetryAfter = DefaultRetryAfter;
            MaxRetryAfter = DefaultMaxRetryAfter;
        }

        public bool AllowAutoRetry { get; internal set; }
        public int MaxAutoRetries { get; internal set; }
        public int RetryAfter { get; set; }
        public int MaxRetryAfter { get; set; }
        public Action<HttpRetryContext> RetryHandler { get; set; }

        // TODO: allow this to be configurable
        public HashSet<HttpStatusCode> RetryStatusCodes { get; internal set; }
    }
}