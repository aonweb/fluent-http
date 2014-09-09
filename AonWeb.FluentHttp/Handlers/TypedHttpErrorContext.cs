using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class TypedHttpErrorContext : TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker<bool> _errorHandled;

        protected TypedHttpErrorContext(TypedHttpCallContext context, HttpResponseMessage response, object error)
            : base(context, response.RequestMessage)
        {
            Response = response;
            ErrorInternal = error;
            _errorHandled = new ModifyTracker<bool>(false);
        }

        protected TypedHttpErrorContext(TypedHttpErrorContext context)
            : base(context)
        {
            Response = context.Response;
            ErrorInternal = context.ErrorInternal;
            _errorHandled = context._errorHandled;
        }

        public HttpResponseMessage Response { get; private set; }
        public HttpStatusCode StatusCode { get { return Response.StatusCode; } }

        public bool ErrorHandled
        {
            get { return _errorHandled.Value; }
            set {  _errorHandled.Value = value; }
        }

        protected object ErrorInternal { get; private set; }

        public override ModifyTracker GetHandlerResult()
        {
            return _errorHandled.ToResult();
        }
    }

    public class TypedHttpErrorContext<TError> : TypedHttpErrorContext
    {
        public TypedHttpErrorContext(TypedHttpCallContext context, HttpResponseMessage response, TError error)
            : base(context, response,  error) { }

        public TypedHttpErrorContext(TypedHttpErrorContext context)
            : base(context) { }

        public TError Error
        {
            get { return (TError)ErrorInternal; }
        }
    }
}