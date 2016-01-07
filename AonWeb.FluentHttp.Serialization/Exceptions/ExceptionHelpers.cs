using System;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Exceptions.Helpers
{
    public static class ExceptionHelpers
    {
        public static string GetExceptionMessage(this HttpRequestMessage request, Exception exception = null, string prefix = null)
        {
            var metadata = new ExceptionResponseMetadata();

            metadata.Apply(request);

            return GetExceptionMessage(metadata, prefix, null, exception?.Message);
        }

        public static string GetExceptionMessage(this HttpResponseMessage response, Exception exception = null, string prefix = null, string reason = null)
        {
            if (response == null)
                return string.Empty;

            var metadata = new ExceptionResponseMetadata();

            metadata.Apply(response);

            return GetExceptionMessage(metadata, prefix, reason, exception?.Message);
        }

        public static string GetExceptionMessage(this IExceptionResponseMetadata metadata, string prefix = null, string reason = null, string additionalDetails = null)
        {
            if (metadata == null)
                return string.Empty;

            var requestMethodString = metadata.RequestMethod?.Method ?? "<Unknown Method>";
            var requestUriString = metadata.RequestUri?.OriginalString ?? "<Unknown Uri>";
            var responseReason = metadata.ReasonPhrase ?? "<Unknown ReasonPhrase>";
            var requestContent = metadata.RequestContentType;
            var responseContent = metadata.ResponseContentType;

            if (metadata.RequestContentLength.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(requestContent))
                    requestContent += ", ";

                requestContent += metadata.RequestContentLength.Value + "bytes";
            }

            if (string.IsNullOrWhiteSpace(requestContent))
                requestContent = ", Content: " + requestContent;

            if (metadata.ResponseContentLength.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(responseContent))
                    responseContent += ", ";

                responseContent += metadata.ResponseContentLength.Value + "bytes";
            }

            if (string.IsNullOrWhiteSpace(responseContent))
                responseContent = " and content " + responseContent;

            var requestMessage = $"{requestMethodString} - {requestUriString}{requestContent}";

            var msg = prefix ?? string.Empty;

            if ((int)metadata.StatusCode < 100)
            {
                msg += requestMessage;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(reason))
                    reason = "returned response with status code";

                msg += $"{requestMessage} {reason} {metadata.StatusCode}-{responseReason}{responseContent}.";
            }

            var addMsg = !string.IsNullOrWhiteSpace(additionalDetails) ? " Additional Details: " + additionalDetails : null;

            return msg + addMsg;
        }

        internal static void Apply(this IWriteableExceptionResponseMetadata exception, HttpResponseMessage response)
        {
            if (response == null)
                return;

            exception.StatusCode = response.StatusCode;
            exception.ReasonPhrase = response.ReasonPhrase;
            exception.ResponseContentType = response.Content?.Headers?.ContentType?.MediaType;
            exception.ResponseContentLength = response.Content?.Headers?.ContentLength;

            exception.Apply(response.RequestMessage);
        }

        internal static void Apply(this IWriteableExceptionResponseMetadata exception, HttpRequestMessage request)
        {
            if (request == null)
                return;

            exception.RequestUri = request.RequestUri;
            exception.RequestMethod = request.Method;
            exception.ResponseContentType = request.Headers?.Accept?.ToString() ?? request.Content?.Headers?.ContentType?.MediaType;
            exception.RequestContentLength = request.Content?.Headers?.ContentLength;
        }
    }
}