using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedResultContext<TResult> : TypedResultContext, IHandlerContextWithResult<TResult>
    {
        public TypedResultContext(ITypedBuilderContext context, HttpResponseMessage response, TResult result)
            : base(context, response, result)
        { }

        internal TypedResultContext(TypedResultContext context)
            : base(context)
        { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }
    }

    public class TypedResultContext: TypedHandlerContext, IHandlerContextWithResult
    {
        private readonly Modifiable _result;

        protected TypedResultContext(ITypedBuilderContext context, HttpResponseMessage response, object result)
            : base(context, response.RequestMessage)
        {
            Response = response;
            _result = new Modifiable(result);
        }

        protected TypedResultContext(TypedResultContext context)
            : base(context)
        {
            Response = context.Response;
            _result = context._result;
        }

        public HttpResponseMessage Response { get; }

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