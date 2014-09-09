using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpResultContext: TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker _result;

        protected TypedHttpResultContext(TypedHttpCallContext context, HttpResponseMessage response, object result)
            : base(context, response.RequestMessage)
        {
            Response = response;
            _result = new ModifyTracker(result);
        }

        protected TypedHttpResultContext(TypedHttpResultContext context)
            : base(context)
        {
            Response = context.Response;
            _result = context._result;
        }

        public HttpResponseMessage Response { get; private set; }

        protected object ResultInternal
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public override ModifyTracker GetHandlerResult()
        {
            return _result.ToResult();
        }
    }

    public class TypedHttpResultContext<TResult> : TypedHttpResultContext
    {
        public TypedHttpResultContext(TypedHttpCallContext context, HttpResponseMessage response, TResult result)
            : base(context, response, result) { }

        internal TypedHttpResultContext(TypedHttpResultContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }
    }
}