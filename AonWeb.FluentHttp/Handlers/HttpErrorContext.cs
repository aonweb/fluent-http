using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpErrorContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        public HttpErrorContext(HttpCallContext<TResult, TContent, TError> context, TError error, HttpResponseMessage response)
            : base(context)
        {
            Error = error;
            Response = response;
        }

        public HttpResponseMessage Response { get; private set; }
        public HttpStatusCode StatusCode { get { return Response.StatusCode; } }
        public bool ErrorHandled { get; set; }
        public TError Error { get; private set; }
    }
}