using System;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks {
    public interface IMockHttpBuilder : IChildHttpBuilder, IResponseMocker<IMockHttpBuilder>
    {
        IMockHttpBuilder VerifyOnSending(Action<SendingContext> handler);
        IMockHttpBuilder VerifyOnSent(Action<SentContext> handler);
        IMockHttpBuilder VerifyOnException(Action<ExceptionContext> handler);
        IMockHttpBuilder WithAssertFailure(Action failureAction);
    }
}