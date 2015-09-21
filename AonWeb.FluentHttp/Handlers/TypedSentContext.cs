using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedSentContext<TResult> : TypedSentContext, IHandlerContextWithResult<TResult>
    {
        public TypedSentContext(ITypedBuilderContext context, HttpResponseMessage response)
            : base(context, response)
        { }

        internal TypedSentContext(TypedSentContext context)
            : base(context)
        { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }
    }

    public abstract class TypedSentContext : TypedHandlerContext, IHandlerContextWithResult
    {
        private readonly Modifiable _result;

        protected TypedSentContext(ITypedBuilderContext context, HttpResponseMessage response)
            : base(context, response.RequestMessage)
        {
            Response = response;
            _result = new Modifiable();
        }

        protected TypedSentContext(TypedSentContext context)
            : base(context)
        {
            Response = context.Response;
            _result = context._result;
        }

        public HttpResponseMessage Response { get; set; }

        protected object ResultInternal
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public override Modifiable GetHandlerResult()
        {
            return _result.ToResult();
        }

        object IHandlerContextWithResult.Result { set { ResultInternal = value; } }
    }
}