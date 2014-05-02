using System;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpExceptionContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        public HttpExceptionContext(HttpCallContext<TResult, TContent, TError> context, Exception exception)
            : base(context)
        {
            Exception = exception;
        }

        public Exception Exception { get; private set; }

        public bool ExceptionHandled { get; set; }
    }

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