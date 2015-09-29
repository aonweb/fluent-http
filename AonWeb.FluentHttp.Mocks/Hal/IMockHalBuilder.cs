using System;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockHalBuilder : IAdvancedHalBuilder, ITypedResultWithResponseMocker<IMockHalBuilder>
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