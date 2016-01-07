using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum number of automatically handled redirect responses from a request is reached.
    /// </summary>
    public class MaximumAutoRedirectsException : HttpCallException
    {
        public MaximumAutoRedirectsException(HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod)
            : this(statusCode, requestUri, requestMethod, null)
        { }

        public MaximumAutoRedirectsException(HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, string message)
            : this(statusCode, requestUri, requestMethod, message, null)
        { }

        public MaximumAutoRedirectsException(HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, string message, Exception exception) :
            base(statusCode, requestUri, requestMethod, GetMessage(message, statusCode, requestUri, requestMethod, exception), exception)
        { }

        public static string GetMessage(string message, HttpStatusCode statusCode, Uri requestUri, HttpMethod requestMethod, Exception exception)
        {
            if (!string.IsNullOrWhiteSpace(message))
                return message;

            var exMsg = exception != null ? " Additional Error Info: " + exception.Message : null;

            var requestMethodString = requestMethod?.Method ?? "<Unknown Method>";
            var requestUriString = requestUri?.OriginalString ?? "<Unknown Uri>";

            return $"The maximum automatic redirection limit was reached for request {requestMethodString} - {requestUriString} with response code {(int)statusCode} {statusCode}.{exMsg}";
        }
    }
}