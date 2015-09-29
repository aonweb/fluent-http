using System;

namespace AonWeb.FluentHttp.Mocks
{
    public interface ITypedResultWithResponseMocker<out TMocker> : IResponseMocker<TMocker>
        where TMocker : ITypedResultWithResponseMocker<TMocker>
    {
        TMocker WithResult<TResult>(
            Predicate<IMockTypedRequestContext> resultPredicate,
            Func<IMockTypedRequestContext, IMockResult<TResult>> resultFactory,
            Predicate<IMockRequestContext> responsePredicate,
            Func<IMockRequestContext, IMockResponse> responseFactory);

        TMocker WithError<TError>(
            Predicate<IMockTypedRequestContext> errorPredicate,
            Func<IMockTypedRequestContext, IMockResult<TError>> errorFactory,
            Predicate<IMockRequestContext> responsePredicate,
            Func<IMockRequestContext, IMockResponse> responseFactory);
    }
}