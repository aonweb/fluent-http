using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpCallErrorContext : TypedHttpCallErrorContext<object>
    {
        public HttpCallErrorContext(TypedHttpCallContext context, HttpResponseMessage response, object error)
            : base(context, response, error) { }
    }
}