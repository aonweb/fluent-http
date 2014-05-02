using System;
using System.Net;
using System.Runtime.Serialization;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum number of automatically handled redirect responses from a request is reached.
    /// </summary>
    public class MaximumAutoRedirectsException : HttpCallException
    {
        public MaximumAutoRedirectsException(HttpStatusCode statusCode)
            : base(statusCode) { }

        public MaximumAutoRedirectsException(HttpStatusCode statusCode, string message)
            : base(statusCode, message) { }

        public MaximumAutoRedirectsException(HttpStatusCode statusCode, string message, Exception exception) :
            base(statusCode, message, exception) { }

        protected MaximumAutoRedirectsException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}