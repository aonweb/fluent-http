using System.Net.Http;

namespace AonWeb.FluentHttp.Helpers
{
    internal static class ExceptionHelpers
    {
        internal static string DetailsForException(this HttpRequestMessage request)
        {
            return request == null ? string.Empty : $"Request: {request.Method} - {request.RequestUri}{request.Content.DetailsForException()}";
        }

        internal static string DetailsForException(this HttpResponseMessage response)
        {
            return response == null ? string.Empty : $"{response.RequestMessage.DetailsForException()}{response.Content.DetailsForException()}";
        }

        internal static string DetailsForException(this HttpContent content)
        {
            if (content == null)
                return string.Empty;

            var type = string.Empty;
            long length = 0;

            if (content.Headers != null)
            {
                if (content.Headers.ContentType != null)
                    type = content.Headers.ContentType.MediaType;

                if (content.Headers.ContentLength != null)
                    length = content.Headers.ContentLength.Value;
            }

            if (string.IsNullOrWhiteSpace(type) || length <= 0)
                return string.Empty;

            return $", Content: {type}, {length} bytes";
        }
    }
}