using System.Net;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public static class LocalResponseExtensions
    {
        public static ILocalResponse WithContent(this ILocalResponse response, string content)
        {
            response.Content = content;
            return response;
        }

        public static ILocalResponse WithContentEncoding(this ILocalResponse response, Encoding contentEncoding)
        {
            response.ContentEncoding = contentEncoding;
            return response;
        }

        public static ILocalResponse WithContentType(this ILocalResponse response, string contentType)
        {
            response.ContentType = contentType;
            return response;
        }

        public static ILocalResponse WithStatusCode(this ILocalResponse response, HttpStatusCode statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        public static ILocalResponse WithStatusDescription(this ILocalResponse response, string statusDescription)
        {
            response.StatusDescription = statusDescription;
            return response;
        }

        public static ILocalResponse AsTransient(this ILocalResponse response)
        {
            response.IsTransient = true;
            return response;
        }
    }
}