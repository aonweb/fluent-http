using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Mocks.WebServer;

namespace AonWeb.FluentHttp.Mocks
{
    public static class Extensions
    {
        #region IResponseMocker<T>

        public static T WithResponse<T>(this IResponseMocker<T> mock, HttpResponseMessage response) 
            where T : IResponseMocker<T>
        {
            return mock.WithResponse(r => response);
        }

        public static T WithResponse<T>(this IResponseMocker<T> mock, ILocalResponse response) 
            where T : IResponseMocker<T>
        {
            return mock.WithResponse(r => response.ToHttpResponseMessage());
        }

        public static T WithResponse<T>(this IResponseMocker<T> mock, HttpStatusCode statusCode) 
            where T : IResponseMocker<T>
        {
            return mock.WithResponse(new LocalResponse(statusCode));
        }

        public static T WithResponse<T>(this IResponseMocker<T> mock, HttpStatusCode statusCode, string content)
            where T : IResponseMocker<T>
        {
            return mock.WithResponse(new LocalResponse(statusCode).WithContent(content));
        }

        public static T WithOkResponse<T>(this IResponseMocker<T> mock) where T : IResponseMocker<T>
        {
            return mock.WithResponse(HttpStatusCode.OK);
        }

        public static T WithOkResponse<T>(this IResponseMocker<T> mock, string content) where T : IResponseMocker<T>
        {
            return mock.WithResponse(HttpStatusCode.OK, content);
        }

        #endregion

        #region ITypedResultMocker<T>

        public static T WithResult<T, TResult>(this ITypedResultMocker<T> mock, TResult result)
            where T : ITypedResultMocker<T>
        {
            return mock.WithResult((r, c) => result);
        }

        public static T WithError<T, TError>(this ITypedResultMocker<T> mock, TError error) 
            where T : ITypedResultMocker<T>
        {
            return mock.WithError((r, c) => error);
        }

        #endregion

        #region IMockTypedBuilder<T>

        public static T WithResult<T, TResult>(this IMockTypedBuilder<T> mock, TResult result)
            where T : IMockTypedBuilder<T>
        {
            return mock.WithResult(result, HttpStatusCode.OK);
        }

        public static T WithError<T, TError>(this IMockTypedBuilder<T> mock, TError error)
            where T : IMockTypedBuilder<T>
        {
            return mock.WithError(error, HttpStatusCode.InternalServerError);
        }

        public static T WithResult<T, TResult>(this IMockTypedBuilder<T> mock, TResult result, HttpStatusCode statusCode)
            where T : IMockTypedBuilder<T>
        {
            return mock.WithResult((r, c) => result, new LocalResponse(statusCode).WithContent("This allows for caching to work properly"));
        }

        public static T WithError<T, TError>(this IMockTypedBuilder<T> mock, TError error, HttpStatusCode statusCode) 
            where T : IMockTypedBuilder<T>
        {
            return mock.WithError((r, c) => error, new LocalResponse(statusCode));
        }

        #endregion

        #region Read Contents

        public static string ReadContents(this HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                return reader.ReadToEnd();
        }

        public static async Task<string> ReadContentsAsync(this Task<HttpResponseMessage> responseTask)
        {
            var r = await responseTask;

            return await r.ReadContentsAsync();
        }

        public static async Task<string> ReadContentsAsync(this HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        #endregion
    }
}