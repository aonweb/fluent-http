using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockHttpCallBuilderBase<out T>
        where T : IMockHttpCallBuilderBase<T>
    {
        T WithResponse(HttpResponseMessage response);
        T WithResponse(ResponseInfo response);
        T ConfigureResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory);
    }

    public interface IMockHttpCallBuilder : IMockHttpCallBuilderBase<IMockHttpCallBuilder>, IAdvancedHttpCallBuilder { }

    public interface IMockTypedCallBuilder<out T, TResult, TContent, TError> : IMockHttpCallBuilderBase<T>
        where T : IMockTypedCallBuilder<T, TResult, TContent, TError>
    {
        T ConfigureResult(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TResult> resultFactory);
        T WithResult(TResult result);
        T WithResult(TResult result, HttpStatusCode statusCode);
        T ConfigureError(Func<HttpResponseMessage, HttpCallContext<TResult, TContent, TError>, TError> errorFactory);
        T WithError(TError error);
        T WithError(TError error, HttpStatusCode statusCode);
    }

    public interface IMockHttpCallBuilder<TResult, TContent, TError> : IMockTypedCallBuilder<IMockHttpCallBuilder<TResult, TContent, TError>, TResult, TContent, TError>, IAdvancedHttpCallBuilder<TResult, TContent, TError> { }

    public interface IMockHalCallBuilder<TResult, TContent, TError> : IMockTypedCallBuilder<IMockHalCallBuilder<TResult, TContent, TError>, TResult, TContent, TError>, IAdvancedHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource 
        where TContent : IHalRequest { }
}