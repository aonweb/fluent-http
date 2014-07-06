using System;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpCallExceptionContext : TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker<bool> _exceptionHandled;

        public TypedHttpCallExceptionContext(TypedHttpCallContext context, Exception exception)
            : base(context)
        {
            Exception = exception;
            _exceptionHandled = new ModifyTracker<bool>(false);
        }

        public TypedHttpCallExceptionContext(TypedHttpCallExceptionContext context)
            : base(context)
        {
            Exception = context.Exception;
            _exceptionHandled = context._exceptionHandled;
        }

        public Exception Exception { get; private set; }

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