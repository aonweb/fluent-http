using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks {
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

    public interface IMockHttpCallBuilder<out T> : IAdvancedHttpCallBuilder, IMockBuilder<T>
        where T : IMockBuilder<T> 
    {
        T VerifyOnSending(Action<HttpSendingContext> handler);
        T VerifyOnSent(Action<HttpSentContext> handler);
        T VerifyOnException(Action<HttpExceptionContext> handler);
        T WithAssertFailure(Action failureAction);
    }

    public interface IMockTypedHttpCallBuilder<out T> : IAdvancedTypedHttpCallBuilder, IMockTypedBuilder<T>
        where T : IMockTypedBuilder<T> 
    {
        T VerifyOnSending(Action<TypedHttpSendingContext<object, object>> handler);
        T VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler);
        T VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler);
        T VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler);
        T VerifyOnSent(Action<TypedHttpSentContext<object>> handler);
        T VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler);
        T VerifyOnResult(Action<TypedHttpResultContext<object>> handler);
        T VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler);
        T VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler);
        T VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler);
        T VerifyOnException(Action<TypedHttpCallExceptionContext> handler);
        T WithAssertFailure(Action failureAction);
    }

    public interface IMockHalCallBuilder<out T> : IAdvancedHalCallBuilder, IMockTypedBuilder<T>
        where T : IMockTypedBuilder<T> 
    {
        T VerifyOnSending(Action<TypedHttpSendingContext<IHalResource, IHalRequest>> handler);
        T VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        T VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<IHalResource, TContent>> handler)
            where TContent : IHalRequest;
        T VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, IHalRequest>> handler)
            where TResult : IHalResource;
        T VerifyOnSent(Action<TypedHttpSentContext<IHalResource>> handler);
        T VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource;
        T VerifyOnResult(Action<TypedHttpResultContext<IHalResource>> handler);
        T VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource;
        T VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler);
        T VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler);
        T VerifyOnException(Action<TypedHttpCallExceptionContext> handler);
        T WithAssertFailure(Action failureAction);
    }


    public interface IMockHttpCallBuilder : IMockHttpCallBuilder<IMockHttpCallBuilder> { }

    public interface IMockTypedHttpCallBuilder : IMockTypedHttpCallBuilder<IMockTypedHttpCallBuilder> { }

    public interface IMockHalCallBuilder : IMockHalCallBuilder<IMockHalCallBuilder> { }
}