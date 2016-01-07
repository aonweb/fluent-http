using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call
    /// </summary>
    public class HttpErrorException<TError> : HttpCallException
    {
        public HttpErrorException(TError error, HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod)
            : this(error ,statusCode, requestUri, requestMethod, null) { }

        public HttpErrorException(TError error, HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, string message)
            : this(error, statusCode, requestUri, requestMethod, message, null)
        {
            Error = error;
        }

        public HttpErrorException(TError error, HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, string message, Exception exception) :
            base(statusCode, requestUri, requestMethod, GetMessage(error, message,  statusCode,  requestUri,  requestMethod,  exception), exception)
        {
            Error = error;
        }

        public TError Error { get; private set; }

        public static string GetMessage(TError error, string message, HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, Exception exception)
        {
            if (!string.IsNullOrWhiteSpace(message))
                return message;

            var errMessage = error?.ToString();

            if(!string.IsNullOrWhiteSpace(errMessage))
                return errMessage;

            var exMsg = exception != null ? " Additional Error Info: " + exception.Message : null;

            var requestMethodString = requestMethod?.Method ?? "<Unknown Method>";
            var requestUriString = requestUri?.OriginalString ?? "<Unknown Uri>";

            return $"{requestMethodString} - {requestUriString} returned an error response with status code {(int)statusCode} {statusCode}.{exMsg}";
        }
    }
}