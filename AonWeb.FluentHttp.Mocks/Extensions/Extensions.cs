using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Mocks
{
    public static class Extensions
    {

        public static T AsTransient<T>(this T mock)
            where T : IMaybeTransient
        {
            mock.IsTransient = true;

            return mock;
        }

        public static T AsIntransient<T>(this T mock)
            where T : IMaybeTransient
        {
            mock.IsTransient = false;

            return mock;
        }
        #region ITypedResultMocker<TMocker>

        public static TMocker WithResult<TMocker, TResult>(this ITypedResultMocker<TMocker> mock, TResult result)
            where TMocker : ITypedResultMocker<TMocker>
        {
            return mock.WithResult((r) => true, context => new MockResult<TResult>(result));
        }

        public static TMocker WithError<TMocker, TError>(this ITypedResultMocker<TMocker> mock, TError error)
            where TMocker : ITypedResultMocker<TMocker>
        {
            return mock.WithError((r) => true, context => new MockResult<TError>(error, new MockHttpResponseMessage(HttpStatusCode.InternalServerError)));
        }

        #endregion

        public static TMocker WithResult<TMocker, TResult>(this ITypedResultWithResponseMocker<TMocker> mock, TResult result)
            where TMocker : ITypedResultWithResponseMocker<TMocker>
        {
            return mock.WithResult(r1 => true, ctx1 => new MockResult<TResult>(result), r2 => true, ctx2 => new MockHttpResponseMessage());
        }

        public static TMocker WithError<TMocker, TError>(this ITypedResultWithResponseMocker<TMocker> mock, TError error)
            where TMocker : ITypedResultWithResponseMocker<TMocker>
        {
            return mock.WithError(r1 => true, ctx1 => new MockResult<TError>(error), r2 => true, ctx2 => new MockHttpResponseMessage(HttpStatusCode.InternalServerError));
        }

        //public static TMocker WithError<TMocker, TError>(this ITypedResultMocker<TMocker> mock, TError error) 
        //    where TMocker : ITypedResultMocker<TMocker>
        //{
        //    return mock.WithError((r, c) => error);
        //}

        //#endregion

        //#region ITypedResultMocker<TMocker>

        //public static TMocker WithResult<TMocker, TResult>(this ITypedResultMocker<TMocker> mock, TResult result)
        //    where TMocker : ITypedResultMocker<TMocker>
        //{
        //    return mock.WithResult(result, HttpStatusCode.OK);
        //}

        //public static TMocker WithError<TMocker, TError>(this ITypedResultMocker<TMocker> mock, TError error)
        //    where TMocker : ITypedResultMocker<TMocker>
        //{
        //    return mock.WithError(error, HttpStatusCode.InternalServerError);
        //}

        //public static TMocker WithResult<TMocker, TResult>(this ITypedResultMocker<TMocker> mock, TResult result, HttpStatusCode statusCode)
        //    where TMocker : ITypedResultMocker<TMocker>
        //{
        //    return mock.WithResult((r, c) => new Tuple<IMockResponse, TResult>(new MockHttpResponseMessage(statusCode).WithContent("This allows for caching to work properly"), result));
        //}

        //public static TMocker WithError<TMocker, TError>(this ITypedResultMocker<TMocker> mock, TError error, HttpStatusCode statusCode) 
        //    where TMocker : ITypedResultMocker<TMocker>
        //{
        //    return mock.WithError((r, c) => error, new MockResponse(statusCode));
        //}

        //#endregion

        #region Read Contents

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