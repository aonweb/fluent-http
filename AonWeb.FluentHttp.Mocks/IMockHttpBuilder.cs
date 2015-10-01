using System;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks {
    public interface IMockHttpBuilder : IChildHttpBuilder, IResponseMocker<IMockHttpBuilder>
    {
        IMockHttpBuilder VerifyOnSending(Action<HttpSendingContext> handler);
        IMockHttpBuilder VerifyOnSent(Action<HttpSentContext> handler);
        IMockHttpBuilder VerifyOnException(Action<HttpExceptionContext> handler);
        IMockHttpBuilder WithAssertFailure(Action failureAction);
    }
}