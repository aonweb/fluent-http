using System;
using System.Net;
using System.Runtime.Serialization;

namespace AonWeb.Fluent.Http.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum number of automatically handled redirect responses from a request is reached.
    /// </summary>
    public class HttpErrorException<TError> : Exception
    {
        public HttpErrorException(TError error, HttpStatusCode statusCode)
        {
            Error = error;
            StatusCode = statusCode;
        }

        public HttpErrorException(TError error, HttpStatusCode statusCode, string message)
            : base(message)
        {
            Error = error;
            StatusCode = statusCode;
        }

        public HttpErrorException(TError error, HttpStatusCode statusCode, string message, Exception exception) :
            base(message, exception)
        {
            Error = error;
            StatusCode = statusCode;
        }

        protected HttpErrorException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        public HttpStatusCode StatusCode { get; private set; }
        public TError Error { get; private set; }
    }
}