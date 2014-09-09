using System;
using System.Net.Http;
using System.Web;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpExceptionContext : TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker<bool> _exceptionHandled;

        public TypedHttpExceptionContext(TypedHttpCallContext context, HttpResponseMessage response, Exception exception)
            : base(context, response != null ? response.RequestMessage : null)
        {
            Exception = exception;
            Response = response;
            _exceptionHandled = new ModifyTracker<bool>(false);
        }

        protected TypedHttpExceptionContext(TypedHttpExceptionContext context)
            : base(context)
        {
            Exception = context.Exception;
            Response = context.Response;
            _exceptionHandled = context._exceptionHandled;
        }

        public Exception Exception { get; private set; }
        public HttpResponseMessage Response { get; set; }

        public bool ExceptionHandled
        {
            get { return _exceptionHandled.Value; }
            set { _exceptionHandled.Value = value; }
        }

        public override ModifyTracker GetHandlerResult()
        {
            return _exceptionHandled.ToResult();
        }
    }
}