using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum number of automatically handled redirect responses from a request is reached.
    /// </summary>
    public class MaximumAutoRedirectsException : HttpCallException
    {
        [ExcludeFromCodeCoverage]
        public MaximumAutoRedirectsException(HttpStatusCode statusCode)
            : base(statusCode) { }

        public MaximumAutoRedirectsException(HttpStatusCode statusCode, string message)
            : base(statusCode, message) { }

        [ExcludeFromCodeCoverage]
        public MaximumAutoRedirectsException(HttpStatusCode statusCode, string message, Exception exception) :
            base(statusCode, message, exception) { }

        [ExcludeFromCodeCoverage]
        protected MaximumAutoRedirectsException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }
    }
}