using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum number of automatically handled redirect responses from a request is reached.
    /// </summary>
    public class MaximumAutoRedirectsException : HttpCallException
    {
        public MaximumAutoRedirectsException(HttpResponseMessage response, int redirectCount)
            : base(response)
        {
            RedirectCount = redirectCount;
        }

        public int RedirectCount { get; }

        protected override string MessagePrefix => $"The maximum automatic redirection limit ({RedirectCount}) was reached for request";

        protected override string MessageReason => "with response";
    }
}