using System;
using System.Net;

namespace AonWeb.FluentHttp.Handlers
{

    public class HttpErrorContext : HttpCallContext
    {
        public HttpErrorContext(HttpCallContext context)
            : base(context) { }

        public HttpStatusCode StatusCode { get; private set; }
    }

    public class HttpErrorContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        public HttpErrorContext(HttpCallContext<TResult, TContent, TError> context, TError error)
            : base(context)
        {
            Error = error;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public TError Error { get; private set; }
        public bool ErrorHandled { get; set; }
    }
}