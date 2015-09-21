using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class ErrorContext : TypedErrorContext<object>
    {
        public ErrorContext(ITypedBuilderContext context, HttpResponseMessage response, object error)
            : base(context, response, error) { }
    }
}