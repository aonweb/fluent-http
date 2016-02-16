using System;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class ExceptionCreationContext : TypedErrorContext<object>
    {
        public ExceptionCreationContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response,
            object error, Exception innerException)
            : base(context, request, response, error)
        {
            InnerException = innerException;
        }

        public Exception InnerException { get; }
    }
}