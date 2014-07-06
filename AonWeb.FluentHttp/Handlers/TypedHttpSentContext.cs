using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class TypedHttpSentContext : TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker _result;

        protected TypedHttpSentContext(TypedHttpCallContext context, HttpResponseMessage response)
            : base(context)
        {
            Response = response;
            _result = new ModifyTracker();
        }

        protected TypedHttpSentContext(TypedHttpSentContext context)
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

        public override ModifyTracker GetHandlerResult()
        {
            return _result.ToResult();
        }
    }

    public class TypedHttpSentContext<TResult> : TypedHttpSentContext
    {
        public TypedHttpSentContext(TypedHttpCallContext context, HttpResponseMessage response)
            : base(context, response) { }

        internal TypedHttpSentContext(TypedHttpSentContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }
    }
}