using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedErrorContext<TError> : TypedErrorContext
    {
        public TypedErrorContext(ITypedBuilderContext context, HttpResponseMessage response, TError error)
            : base(context, response, error)
        { }

        public TypedErrorContext(TypedErrorContext context)
            : base(context)
        { }

        public TError Error => (TError)ErrorInternal;
    }

    public abstract class TypedErrorContext : TypedHandlerContext
    {
        private readonly Modifiable<bool> _errorHandled;

        protected TypedErrorContext(ITypedBuilderContext context, HttpResponseMessage response, object error)
            : base(context, response.RequestMessage)
        {
            Response = response;
            ErrorInternal = error;
            _errorHandled = new Modifiable<bool>(false);
        }

        protected TypedErrorContext(TypedErrorContext context)
            : base(context)
        {
            Response = context.Response;
            ErrorInternal = context.ErrorInternal;
            _errorHandled = context._errorHandled;
        }

        public HttpResponseMessage Response { get; }
        public HttpStatusCode StatusCode => Response.StatusCode;

        public bool ErrorHandled
        {
            get { return _errorHandled.Value; }
            set {  _errorHandled.Value = value; }
        }

        protected object ErrorInternal { get; }

        public override Modifiable GetHandlerResult()
        {
            return _errorHandled.ToResult();
        }
    }
}