using System;
using System.Net.Http;

namespace AonWeb.Fluent.Http.Handlers
{
    public interface IRedirectHandler : IHttpCallHandler
    {
        IRedirectHandler WithAutoRedirect();
        IRedirectHandler WithAutoRedirect(int maxAutoRedirects);
        IRedirectHandler WithHandler(Action<HttpRedirectContext> handler);
    }
}