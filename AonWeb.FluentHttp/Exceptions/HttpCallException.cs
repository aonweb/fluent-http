using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call.
    /// </summary>
    public class HttpCallException : Exception
    {
        public HttpCallException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpCallException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        [ExcludeFromCodeCoverage]
        public HttpCallException(HttpStatusCode statusCode, string message, Exception exception) :
            base(message, exception)
        {
            StatusCode = statusCode;
        }

        [ExcludeFromCodeCoverage]
        protected HttpCallException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        public HttpStatusCode StatusCode { get; private set; }
    }
}