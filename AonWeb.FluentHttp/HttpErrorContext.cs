using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public class HttpErrorContext : TypedHttpErrorContext<object>
    {
        public HttpErrorContext(TypedHttpCallContext context, HttpResponseMessage response, object error)
            : base(context, response, error) { }
    }
}