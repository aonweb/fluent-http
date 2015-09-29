using System;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockTypedBuilder : IChildTypedBuilder, ITypedResultWithResponseMocker<IMockTypedBuilder>
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
}