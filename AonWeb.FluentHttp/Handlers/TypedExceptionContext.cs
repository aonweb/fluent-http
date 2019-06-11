using System;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedExceptionContext : TypedHandlerContext
    {
        private readonly Modifiable<bool> _exceptionHandled;

        public TypedExceptionContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, Exception exception)
            : base(context, request)
        {
            Exception = exception;
            Response = response;
            
            _exceptionHandled = new Modifiable<bool>(false);
        }

        public TypedExceptionContext(TypedExceptionContext context)
            : base(context)
        {
            Exception = context.Exception;
            Response = context.Response;
            _exceptionHandled = context._exceptionHandled;
        }

        public Exception Exception { get; }
        public HttpResponseMessage Response { get; set; }

        public bool ExceptionHandled
        {
            get => _exceptionHandled.Value;
            set => _exceptionHandled.Value = value;
        }

        public override Modifiable GetHandlerResult()
        {
            return _exceptionHandled.ToResult();
        }
    }
}