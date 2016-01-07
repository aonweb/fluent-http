using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call.
    /// </summary>
    public class HttpCallException : Exception
    {
        public HttpCallException(HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod)
            : this(statusCode, requestUri, requestMethod, null, null)
        { }

        public HttpCallException(HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, string message)
            : this(statusCode, requestUri, requestMethod, message, null)
        { }

        public HttpCallException(HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, string message, Exception exception) :
            base(GetMessage(message, statusCode, requestUri, requestMethod, exception), exception)
        {
            StatusCode = statusCode;
            RequestUri = requestUri;
            RequestMethod = requestMethod;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public Uri RequestUri { get; private set; }
        public HttpMethod RequestMethod { get; private set; }

        private static string GetMessage(string message, HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, Exception exception)
        {
            if (!string.IsNullOrWhiteSpace(message))
                return message;

            var exMsg = exception != null ? " Additional Error Info: " + exception.Message : null;

            var requestMethodString = requestMethod?.Method ?? "<Unknown Method>";
            var requestUriString = requestUri?.OriginalString ?? "<Unknown Uri>";

            return $"{requestMethodString} - {requestUriString} returned response with status code {(int)statusCode} {statusCode}.{exMsg}";
        }
    }
}