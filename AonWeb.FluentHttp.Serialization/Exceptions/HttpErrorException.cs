using System;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Exceptions.Helpers;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call
    /// </summary>
    public class HttpErrorException<TError> : HttpCallException
    {
        public HttpErrorException(TError error, HttpResponseMessage response)
            : this(error, response, null, null)
        { }

        public HttpErrorException(TError error, HttpResponseMessage response, string message)
            : this(error, response, message, null)
        { }

        public HttpErrorException(TError error, HttpResponseMessage response, string message, Exception exception)
            : base(response, message, exception)
        {
            Error = error;
        }

        public HttpErrorException(TError error, HttpStatusCode statusCode)
            : this(error ,statusCode, null) { }

        public HttpErrorException(TError error, HttpStatusCode statusCode, string message)
            : this(error, statusCode, message, null)
        { }

        public HttpErrorException(TError error, HttpStatusCode statusCode, string message, Exception exception) 
            : base(statusCode, message, exception)
        {
            Error = error;
        }

        public TError Error { get; private set; }

        protected override  string MessageReason => "returned an error response with status code";
    }
}