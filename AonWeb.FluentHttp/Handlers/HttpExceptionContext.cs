using System;

namespace AonWeb.Fluent.Http.Handlers
{
    public class HttpExceptionContext
    {
        public Exception Exception { get; set; }
        public bool ExceptionHandled { get; set; }
    }
}