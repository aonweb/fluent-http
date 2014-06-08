using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedHttpResultContext<TResult> : TypedHttpCallHandlerContext
    {
        public TypedHttpResultContext(TypedHttpCallContext context, HttpResponseMessage response, TResult result)
            : base(context)
        {
            Response = response;

            _result = new HttpCallHandlerResult<TResult>(result);
        }

        public HttpResponseMessage Response { get; private set; }

        private readonly HttpCallHandlerResult<TResult> _result;

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

        public override HttpCallHandlerResult GetHandlerResult()
        {
            return _result.ToResult();
        }
    }
}