using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpSentContext<TResult> : TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker<TResult> _result;

        public TypedHttpSentContext(TypedHttpCallContext context, HttpResponseMessage response)
            : base(context)
        {
            Response = response;

            _result = new ModifyTracker<TResult>();
        }

        public HttpResponseMessage Response { get; set; }

        public TResult Result
        {
            get
            {
                return _result.Value;
            }
            internal set
            {
                _result.Value = value;
            }
        }

        public override ModifyTracker GetHandlerResult()
        {
            return _result.ToResult();
        }
    }
}