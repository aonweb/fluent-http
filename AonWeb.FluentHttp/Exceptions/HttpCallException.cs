using System;
using System.Net;

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

        public HttpCallException(HttpStatusCode statusCode, string message, Exception exception) :
            base(message, exception)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}