using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;

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

        public static string GetExceptionMessage(this HttpResponseMessage response, HttpRequestMessage request, Exception exception = null, string prefix = null, string reason = null)
        {
            if (response == null)
                return string.Empty;

            var metadata = new ExceptionResponseMetadata();

            metadata.Apply(response);
            metadata.Apply(request);

            return GetExceptionMessage(metadata, prefix, reason, exception?.Message);
        }
        public static string GetExceptionMessage(this IExceptionResponseMetadata metadata, string prefix = null, string reason = null, string additionalDetails = null, bool includeContentInfo = false)
        {
            if (metadata == null)
                return string.Empty;

            var messageBuilder = new StringBuilder()
                .Append(prefix)
                .Append(string.IsNullOrWhiteSpace(prefix) ? "R" : " r")
                .Append("equest ")
                .Append(metadata.RequestMethod?.Method ?? "<Unknown Method>")
                .Append(" ")
                .Append(metadata.RequestUri?.OriginalString ?? "<Unknown Uri>");


            if (includeContentInfo && metadata.RequestContentLength > 0)
            {
                messageBuilder.Append(", Content ");

                if (!string.IsNullOrWhiteSpace(metadata.ResponseContentType))
                    messageBuilder.Append("Type: " + metadata.ResponseContentType + ", ");

                messageBuilder.Append("Length: " + metadata.RequestContentLength + " bytes");
            }

            if ((int)metadata.StatusCode >= 100)
            {
                if (string.IsNullOrWhiteSpace(reason))
                    reason = "returned";

                messageBuilder
                    .Append(" ")
                    .Append(reason)
                    .Append(" response ")
                    .Append((int)metadata.StatusCode)
                    .Append("-")
                    .Append(metadata.ReasonPhrase ?? "<Unknown ReasonPhrase>");

                if (includeContentInfo && metadata.ResponseContentLength > 0)
                {
                    messageBuilder.Append(", Content ");

                    if (!string.IsNullOrWhiteSpace(metadata.ResponseContentType))
                        messageBuilder.Append("Type: " + metadata.ResponseContentType + ", ");

                    messageBuilder.Append("Length: " + metadata.ResponseContentLength + " bytes");
                }
            }

            messageBuilder.Append(".");

            if (!string.IsNullOrWhiteSpace(additionalDetails))
                messageBuilder.Append(" Additional Details: ").Append(additionalDetails);


            return messageBuilder.ToString();
        }

        internal static void Apply(this IWriteableExceptionResponseMetadata exception, HttpResponseMessage response)
        {
            if (response == null)
                return;

            try
            {
                exception.StatusCode = response.StatusCode;
                exception.ReasonPhrase = response.ReasonPhrase;
                exception.ResponseContentType = response.Content?.Headers?.ContentType?.MediaType;
                exception.ResponseContentLength = response.Content?.Headers?.ContentLength;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            } // don't let exception logging info cause exception
        }

        internal static void Apply(this IWriteableExceptionResponseMetadata exception, HttpRequestMessage request)
        {
            if (request == null)
                return;

            try
            {
                exception.RequestUri = request.RequestUri;
                exception.RequestMethod = request.Method;
                exception.ResponseContentType = request.Headers?.Accept?.ToString() ?? request.Content?.Headers?.ContentType?.MediaType;
                exception.RequestContentLength = request.Content?.Headers?.ContentLength;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}