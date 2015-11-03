using System;
using System.Net;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call
    /// </summary>
    public class HttpErrorException<TError> : HttpCallException
    {
        public HttpErrorException(TError error, HttpStatusCode statusCode)
            : this(error ,statusCode, error?.ToString())
        {
            Error = error;
        }

        public HttpErrorException(TError error, HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        {
            Error = error;
        }

        public HttpErrorException(TError error, HttpStatusCode statusCode, string message, Exception exception) :
            base(statusCode, message, exception)
        {
            Error = error;
        }

        public TError Error { get; private set; }
    }
}