using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Mocks
{
    public static class Extensions
    {
        #region IHttpMocker<T>

        public static T WithResponse<T>(this IHttpMocker<T> mock, HttpResponseMessage response) 
            where T : IHttpMocker<T>
        {
            return mock.WithResponse(r => response);
        }

        public static T WithResponse<T>(this IHttpMocker<T> mock, ResponseInfo response) 
            where T : IHttpMocker<T>
        {
            return mock.WithResponse(r => response.ToHttpResponseMessage());
        }

        public static T WithResponse<T>(this IHttpMocker<T> mock, HttpStatusCode statusCode) 
            where T : IHttpMocker<T>
        {
            return mock.WithResponse(new ResponseInfo(statusCode));
        }

        public static T WithResponse<T>(this IHttpMocker<T> mock, HttpStatusCode statusCode, string content)
            where T : IHttpMocker<T>
        {
            return mock.WithResponse(new ResponseInfo(statusCode) { Body = content });
        }

        public static T WithOkResponse<T>(this IHttpMocker<T> mock) where T : IHttpMocker<T>
        {
            return mock.WithResponse(HttpStatusCode.OK);
        }

        public static T WithOkResponse<T>(this IHttpMocker<T> mock, string content) where T : IHttpMocker<T>
        {
            return mock.WithResponse(HttpStatusCode.OK, content);
        }

        #endregion

        #region IHttpTypedMocker<T>

        public static T WithResult<T, TResult>(this IHttpTypedMocker<T> mock, TResult result)
            where T : IHttpTypedMocker<T>
        {
            return mock.WithResult((r, c) => result);
        }

        public static T WithError<T, TError>(this IHttpTypedMocker<T> mock, TError error) 
            where T : IHttpTypedMocker<T>
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
            return mock.WithResult((r, c) => result, new ResponseInfo(statusCode));
        }

        public static T WithError<T, TError>(this IMockTypedBuilder<T> mock, TError error, HttpStatusCode statusCode) 
            where T : IMockTypedBuilder<T>
        {
            return mock. WithError((r, c) => error, new ResponseInfo(statusCode));
        }

        #endregion

        #region Read Contents

        public static string ReadContents(this HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                return reader.ReadToEnd();
        }

        public static string ReadContents(this Task<HttpResponseMessage> response)
        {
            return response.Result.ReadContents();
        }

        public static string ReadContents(this HttpResponseMessage response)
        {
            return response.Content.ReadAsStringAsync().Result;
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