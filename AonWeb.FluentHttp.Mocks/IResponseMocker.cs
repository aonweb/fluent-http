using System;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IResponseMocker<out TMocker>
        where TMocker : IResponseMocker<TMocker>
    {
        TMocker WithResponse(Predicate<IMockRequestContext> predicate, Func<IMockRequestContext, IMockResponse> responseFactory);
    }
}