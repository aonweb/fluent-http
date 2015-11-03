using System;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheContext : ICacheValidator, IContext
    {
        HttpRequestMessage Request { get; }
        Uri Uri { get; }
        IHandlerContext GetHandlerContext();
    }
}