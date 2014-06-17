using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Mocks
{
    internal static class Extensions
    {
        internal static  T ConfigureMock<T>(this T builder)
            where T : IMockBuilder<T>, IHttpCallBuilder
        {
            return (T)builder.Advanced
                .OnSending(HttpCallHandlerPriority.Last, context => context.Items["MockRequest"] = context.Request)
                .OnSent(HttpCallHandlerPriority.First, context => context.Response.RequestMessage = context.Items["MockRequest"] as HttpRequestMessage);
        }
    }
}