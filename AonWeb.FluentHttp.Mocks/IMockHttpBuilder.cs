using System;
using System.Net.Http;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks {
    public interface IResponseMocker<out T>
        where T : IResponseMocker<T> 
    {
        T WithResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory);
    }

    public interface ITypedResultMocker<out T>
        where T : ITypedResultMocker<T>
    {
        T WithResult<TResult>(Func<HttpResponseMessage, ITypedBuilderContext, TResult> resultFactory);
        T WithError<TError>(Func<HttpResponseMessage, ITypedBuilderContext, TError> errorFactory);
    }

    public interface IMockTypedBuilder<out T> :
        ITypedResultMocker<T>,
        IResponseMocker<T>
        where T : IMockTypedBuilder<T>
    {
        T WithResult<TResult>(Func<HttpResponseMessage, ITypedBuilderContext, TResult> resultFactory, ILocalResponse response);
        T WithError<TError>(Func<HttpResponseMessage, ITypedBuilderContext, TError> errorFactory, ILocalResponse response);
    }

    public interface IMockHttpClientBuilder : IResponseMocker<IMockHttpClientBuilder>, IHttpClientBuilder
    {
        
    }

    public interface IMockHttpBuilder : IChildHttpBuilder, IResponseMocker<IMockHttpBuilder>
    {
        IMockHttpBuilder VerifyOnSending(Action<SendingContext> handler);
        IMockHttpBuilder VerifyOnSent(Action<SentContext> handler);
        IMockHttpBuilder VerifyOnException(Action<ExceptionContext> handler);
        IMockHttpBuilder WithAssertFailure(Action failureAction);
    }

    public interface IMockTypedBuilder : IChildTypedBuilder, IMockTypedBuilder<IMockTypedBuilder>
    {
        IMockTypedBuilder VerifyOnSending(Action<TypedSendingContext<object, object>> handler);
        IMockTypedBuilder VerifyOnSending<TResult, TContent>(Action<TypedSendingContext<TResult, TContent>> handler);
        IMockTypedBuilder VerifyOnSendingWithContent<TContent>(Action<TypedSendingContext<object, TContent>> handler);
        IMockTypedBuilder VerifyOnSendingWithResult<TResult>(Action<TypedSendingContext<TResult, object>> handler);
        IMockTypedBuilder VerifyOnSent(Action<TypedSentContext<object>> handler);
        IMockTypedBuilder VerifyOnSent<TResult>(Action<TypedSentContext<TResult>> handler);
        IMockTypedBuilder VerifyOnResult(Action<TypedResultContext<object>> handler);
        IMockTypedBuilder VerifyOnResult<TResult>(Action<TypedResultContext<TResult>> handler);
        IMockTypedBuilder VerifyOnError(Action<TypedErrorContext<object>> handler);
        IMockTypedBuilder VerifyOnError<TError>(Action<TypedErrorContext<TError>> handler);
        IMockTypedBuilder VerifyOnException(Action<TypedExceptionContext> handler);
        IMockTypedBuilder WithAssertFailure(Action failureAction);
    }

    public interface IMockHalBuilder : IAdvancedHalBuilder, IMockTypedBuilder<IMockHalBuilder>
    {
        IMockHalBuilder VerifyOnSending(Action<TypedSendingContext<IHalResource, IHalRequest>> handler);
        IMockHalBuilder VerifyOnSending<TResult, TContent>(Action<TypedSendingContext<TResult, TContent>> handler)
            where TResult : IHalResource
            where TContent : IHalRequest;
        IMockHalBuilder VerifyOnSendingWithContent<TContent>(Action<TypedSendingContext<IHalResource, TContent>> handler)
            where TContent : IHalRequest;
        IMockHalBuilder VerifyOnSendingWithResult<TResult>(Action<TypedSendingContext<TResult, IHalRequest>> handler)
            where TResult : IHalResource;
        IMockHalBuilder VerifyOnSent(Action<TypedSentContext<IHalResource>> handler);
        IMockHalBuilder VerifyOnSent<TResult>(Action<TypedSentContext<TResult>> handler)
            where TResult : IHalResource;
        IMockHalBuilder VerifyOnResult(Action<TypedResultContext<IHalResource>> handler);
        IMockHalBuilder VerifyOnResult<TResult>(Action<TypedResultContext<TResult>> handler)
            where TResult : IHalResource;
        IMockHalBuilder VerifyOnError(Action<TypedErrorContext<object>> handler);
        IMockHalBuilder VerifyOnError<TError>(Action<TypedErrorContext<TError>> handler);
        IMockHalBuilder VerifyOnException(Action<TypedExceptionContext> handler);
        IMockHalBuilder WithAssertFailure(Action failureAction);
    }
}