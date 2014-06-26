using System;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Client;
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

    public interface IMockTypedBuilder<out T> :
        IHttpTypedMocker<T>,
        IHttpMocker<T>
        where T : IMockTypedBuilder<T> 
    {
        T WithResult<TResult>(TResult result, HttpStatusCode statusCode);
        T WithResult<TResult>(Func<HttpResponseMessage, TypedHttpCallContext, TResult> resultFactory, ResponseInfo response);
        T WithError<TError>(TError error, HttpStatusCode statusCode);
        T WithError<TError>(Func<HttpResponseMessage, TypedHttpCallContext, TError> errorFactory, ResponseInfo response);
    }

    public interface IMockFormatter : IHttpTypedMocker<IMockFormatter>, IHttpCallFormatter { }

    public interface IMockHttpClientBuilder : IHttpMocker<IMockHttpClientBuilder>, IHttpClientBuilder { }

    public interface IMockHttpCallBuilder : IChildHttpCallBuilder, IHttpMocker<IMockHttpCallBuilder>
    {
        IMockHttpCallBuilder VerifyOnSending(Action<HttpSendingContext> handler);
        IMockHttpCallBuilder VerifyOnSent(Action<HttpSentContext> handler);
        IMockHttpCallBuilder VerifyOnException(Action<HttpExceptionContext> handler);
        IMockHttpCallBuilder WithAssertFailure(Action failureAction);
    }

    public interface IMockTypedHttpCallBuilder : IAdvancedTypedHttpCallBuilder, IMockTypedBuilder<IMockTypedHttpCallBuilder>
    {
        IMockTypedHttpCallBuilder VerifyOnSending(Action<TypedHttpSendingContext<object, object>> handler);
        IMockTypedHttpCallBuilder VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler);
        IMockTypedHttpCallBuilder VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<object, TContent>> handler);
        IMockTypedHttpCallBuilder VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, object>> handler);
        IMockTypedHttpCallBuilder VerifyOnSent(Action<TypedHttpSentContext<object>> handler);
        IMockTypedHttpCallBuilder VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler);
        IMockTypedHttpCallBuilder VerifyOnResult(Action<TypedHttpResultContext<object>> handler);
        IMockTypedHttpCallBuilder VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler);
        IMockTypedHttpCallBuilder VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler);
        IMockTypedHttpCallBuilder VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler);
        IMockTypedHttpCallBuilder VerifyOnException(Action<TypedHttpCallExceptionContext> handler);
        IMockTypedHttpCallBuilder WithAssertFailure(Action failureAction);
    }

    public interface IMockHalCallBuilder : IAdvancedHalCallBuilder, IMockTypedBuilder<IMockHalCallBuilder>
    {
        IMockHalCallBuilder VerifyOnSending(Action<TypedHttpSendingContext<IHalResource, IHalRequest>> handler);
        IMockHalCallBuilder VerifyOnSending<TResult, TContent>(Action<TypedHttpSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IMockHalCallBuilder VerifyOnSendingWithContent<TContent>(Action<TypedHttpSendingContext<IHalResource, TContent>> handler)
            where TContent : IHalRequest;
        IMockHalCallBuilder VerifyOnSendingWithResult<TResult>(Action<TypedHttpSendingContext<TResult, IHalRequest>> handler)
            where TResult : IHalResource;
        IMockHalCallBuilder VerifyOnSent(Action<TypedHttpSentContext<IHalResource>> handler);
        IMockHalCallBuilder VerifyOnSent<TResult>(Action<TypedHttpSentContext<TResult>> handler)
            where TResult : IHalResource;
        IMockHalCallBuilder VerifyOnResult(Action<TypedHttpResultContext<IHalResource>> handler);
        IMockHalCallBuilder VerifyOnResult<TResult>(Action<TypedHttpResultContext<TResult>> handler)
            where TResult : IHalResource;
        IMockHalCallBuilder VerifyOnError(Action<TypedHttpCallErrorContext<object>> handler);
        IMockHalCallBuilder VerifyOnError<TError>(Action<TypedHttpCallErrorContext<TError>> handler);
        IMockHalCallBuilder VerifyOnException(Action<TypedHttpCallExceptionContext> handler);
        IMockHalCallBuilder WithAssertFailure(Action failureAction);
    }
}