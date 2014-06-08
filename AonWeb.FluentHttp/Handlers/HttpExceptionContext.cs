using System;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpExceptionContext : HttpCallContext
    {
        public HttpExceptionContext(HttpCallContext context, Exception exception)
            : base(context)
        {
            Exception = exception;
        }

        public Exception Exception { get; private set; }

        public bool ExceptionHandled { get; set; }
    }
}