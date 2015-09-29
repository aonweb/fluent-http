using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class ErrorContext : TypedErrorContext<object>
    {
        public ErrorContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, object error)
            : base(context, request, response, error) { }
    }
}