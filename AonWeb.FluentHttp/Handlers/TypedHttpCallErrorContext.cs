using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpCallErrorContext<TError> : TypedHttpCallHandlerContext
    {
        private readonly HttpCallHandlerResult<bool> _errorHandled;

        public TypedHttpCallErrorContext(TypedHttpCallContext context, HttpResponseMessage response, TError error)
            : base(context)
        {
            Response = response;
            Error = error;
            _errorHandled = new HttpCallHandlerResult<bool>(false);
        }

        public HttpResponseMessage Response { get; private set; }
        public HttpStatusCode StatusCode { get { return Response.StatusCode; } }

        public bool ErrorHandled
        {
            get
            {
                return _errorHandled.Value;
            }
            internal set
            {
                _errorHandled.Value = value;
            }
        }

        public TError Error { get; private set; }

        public override HttpCallHandlerResult GetHandlerResult()
        {
            return _errorHandled.ToResult();
        }
    }
}