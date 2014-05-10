using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call
    /// </summary>
    public class HttpErrorException<TError> : HttpCallException
    {
        public HttpErrorException(TError error, HttpStatusCode statusCode)
            : base(statusCode)
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

        [ExcludeFromCodeCoverage]
        protected HttpErrorException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }

        public TError Error { get; private set; }
    }
}