using System;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpCallExceptionContext : TypedHttpCallHandlerContext
    {
        private readonly HttpCallHandlerResult<bool> _exceptionHandled;

        public TypedHttpCallExceptionContext(TypedHttpCallContext context, Exception exception)
            : base(context)
        {
            Exception = exception;
            _exceptionHandled = new HttpCallHandlerResult<bool>(false);
        }

        public Exception Exception { get; private set; }


        public bool ExceptionHandled
        {
            get
            {
                return _exceptionHandled.Value;
            }
            internal set
            {
                _exceptionHandled.Value = value;
            }
        }

        public override HttpCallHandlerResult GetHandlerResult()
        {
            return _exceptionHandled.ToResult();
        }
    }
}