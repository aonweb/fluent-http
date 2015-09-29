using System;

namespace AonWeb.FluentHttp.Mocks
{
    public interface ITypedResultMocker<out TMocker>
        where TMocker : ITypedResultMocker<TMocker>
    {
        TMocker WithResult<TResult>(
            Predicate<IMockTypedRequestContext> predicate,
            Func<IMockTypedRequestContext, IMockResult<TResult>> resultFactory);

        TMocker WithError<TError>(
            Predicate<IMockTypedRequestContext> predicate,
            Func<IMockTypedRequestContext, IMockResult<TError>> errorFactory);
    }
}