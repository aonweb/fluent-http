using System;
using System.Net.Http;

namespace AonWeb.Fluent.Http.Handlers
{
    public interface IRetryHandler: IHttpCallHandler
    {
        IRetryHandler WithAutoRetry();
        IRetryHandler WithAutoRetry(int maxAutoRetries, int retryAfter);
        IRetryHandler WithHandler(Action<HttpRetryContext> handler);
    }
}