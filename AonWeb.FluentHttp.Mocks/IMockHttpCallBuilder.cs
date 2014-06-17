using System;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IHttpMocker<out T>
        where T : IHttpMocker<T>
    {
        T WithResponse(HttpResponseMessage response);
        T WithResponse(ResponseInfo response);
        T WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory);
    }

    public interface IHttpTypedMocker<out T>
        where T : IHttpTypedMocker<T>
    {
        T WithResult<TResult>(TResult result);
        T WithError<TError>(TError error);
        T WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory);
        T WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory);
    }

    public interface IMockBuilder<out T> : IHttpMocker<T>
        where T : IMockBuilder<T> { }

    public interface IMockTypedBuilder<out T> :
        IHttpTypedMocker<T>,
        IHttpMocker<T>
        where T : IMockTypedBuilder<T>

    {
        T WithResult<TResult>(TResult result, HttpStatusCode statusCode);
        T WithError<TError>(TError error, HttpStatusCode statusCode);
    }
}