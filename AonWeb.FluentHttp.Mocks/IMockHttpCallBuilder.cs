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

    public interface IHttpTypedMocker<out T, TResult, TContent, TError>
        where T : IHttpTypedMocker<T, TResult, TContent, TError>
    {
        T WithResult(TResult result);
        T WithError(TError error);
        T WithResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory);
        T WithError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory);
    }

    public interface IMockBuilder : IHttpMocker<IMockBuilder> { }

    public interface IMockBuilder<TResult, TContent, TError> :
        IHttpTypedMocker<IMockBuilder<TResult, TContent, TError>, TResult, TContent, TError>,
        IHttpMocker<IMockBuilder<TResult, TContent, TError>>
    {
        IMockBuilder<TResult, TContent, TError> WithResult(TResult result, HttpStatusCode statusCode);
        IMockBuilder<TResult, TContent, TError> WithError(TError error, HttpStatusCode statusCode);
    }
}