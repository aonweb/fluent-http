using System;
using System.Net;

namespace AonWeb.FluentHttp.Mocks
{
    public static class ResponseMockerExtensions
    {

        public static TMocker WithResponse<TMocker>(this IResponseMocker<TMocker> mocker, Action<IMockRequestContext, IMockResponse> configure)
            where TMocker : IResponseMocker<TMocker>
        {
            return mocker.WithAllResponses(ctx =>
            {
                var response = new MockHttpResponseMessage();

                configure?.Invoke(ctx, response);

                return response;
            });
        }

        public static TMocker WithResponse<TMocker>(this IResponseMocker<TMocker> mocker, Predicate<IMockRequestContext> predicate, IMockResponse response)
            where TMocker : IResponseMocker<TMocker>
        {

            return mocker.WithResponse(predicate, ctx => response);
        }

        public static TMocker WithResponse<TMocker>(this IResponseMocker<TMocker> mock, Predicate<IMockRequestContext> predicate, HttpStatusCode statusCode)
            where TMocker : IResponseMocker<TMocker>
        {
            return mock.WithResponse(predicate, new MockHttpResponseMessage(statusCode));
        }

        public static TMocker WithResponse<TMocker> (this IResponseMocker<TMocker>  mock, Predicate<IMockRequestContext> predicate, HttpStatusCode statusCode, string content)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithResponse(predicate, new MockHttpResponseMessage(statusCode).WithContent(content));
        }

        public static TMocker WithResponseOk<TMocker> (this IResponseMocker<TMocker>  mock, Predicate<IMockRequestContext> predicate) 
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithResponse(predicate, HttpStatusCode.OK);
        }

        public static TMocker WithResponseOk<TMocker> (this IResponseMocker<TMocker>  mock, Predicate<IMockRequestContext> predicate, string content)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithResponse(predicate, HttpStatusCode.OK, content);
        }


        #region WithNextResponse

        public static TMocker WithNextResponse<TMocker>(this IResponseMocker<TMocker> mocker, IMockResponse response)
             where TMocker : IResponseMocker<TMocker>
        {
            return mocker.WithResponse(r => true, response.AsTransient());
        }

        public static TMocker WithNextResponse<TMocker>(this IResponseMocker<TMocker> mock, HttpStatusCode statusCode)
            where TMocker : IResponseMocker<TMocker>
        {
            return mock.WithNextResponse(new MockHttpResponseMessage(statusCode));
        }

        public static TMocker WithNextResponse<TMocker> (this IResponseMocker<TMocker>  mock, HttpStatusCode statusCode, string content)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithNextResponse(new MockHttpResponseMessage(statusCode).WithContent(content));
        }

        public static TMocker WithNextResponseOk<TMocker> (this IResponseMocker<TMocker>  mock)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithNextResponse(HttpStatusCode.OK);
        }

        public static TMocker WithNextResponseOk<TMocker> (this IResponseMocker<TMocker>  mock, string content)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithNextResponse(HttpStatusCode.OK, content);
        }

        #endregion

        #region WithAllResponses

        public static TMocker WithAllResponses<TMocker>(this IResponseMocker<TMocker> mocker, Func<IMockRequestContext, IMockResponse> responseFactory)
             where TMocker : IResponseMocker<TMocker>
        {
            return mocker.WithResponse(r => true, responseFactory);
        }

        public static TMocker WithAllResponses<TMocker>(this IResponseMocker<TMocker> mocker, IMockResponse response)
             where TMocker : IResponseMocker<TMocker>
        {
            return mocker.WithAllResponses(ctx => response);
        }

        public static TMocker WithAllResponses<TMocker>(this IResponseMocker<TMocker> mock, HttpStatusCode statusCode)
            where TMocker : IResponseMocker<TMocker>
        {
            return mock.WithAllResponses(new MockHttpResponseMessage(statusCode));
        }

        public static TMocker WithAllResponses<TMocker> (this IResponseMocker<TMocker>  mock, HttpStatusCode statusCode, string content)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithAllResponses(new MockHttpResponseMessage(statusCode).WithContent(content));
        }

        public static TMocker WithAllResponsesOk<TMocker> (this IResponseMocker<TMocker>  mock)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithAllResponses(HttpStatusCode.OK);
        }

        public static TMocker WithAllResponsesOk<TMocker> (this IResponseMocker<TMocker>  mock, string content)
            where TMocker : IResponseMocker<TMocker> 
        {
            return mock.WithAllResponses(HttpStatusCode.OK, content);
        }

        #endregion

    }
}