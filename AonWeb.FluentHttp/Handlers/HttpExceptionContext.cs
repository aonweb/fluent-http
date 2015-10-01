using System;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpExceptionContext : HttpHandlerContext
    {
        private readonly Modifiable<bool> _exceptionHandled;

        public HttpExceptionContext(IHttpBuilderContext context, HttpResponseMessage response, Exception exception)
            : base(context, response?.RequestMessage)
        {
            Exception = exception;
            Response = response;
            _exceptionHandled = new Modifiable<bool>(false);
        }

        protected HttpExceptionContext(HttpExceptionContext context)
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
            get { return _exceptionHandled.Value; }
            set { _exceptionHandled.Value = value; }
        }

        public override Modifiable GetHandlerResult()
        {
            return _exceptionHandled.ToResult();
        }
    }
}