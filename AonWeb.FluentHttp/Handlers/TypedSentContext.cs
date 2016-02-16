using System.Net.Http;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedSentContext<TResult> : TypedSentContext, IHandlerContextWithResult<TResult>
    {
        public TypedSentContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response)
            : base(context, request, response)
        { }

        internal TypedSentContext(TypedSentContext context)
            : base(context)
        { }

        public new TResult Result
        {
            get { return TypeHelpers.CheckType<TResult>(base.Result, SuppressTypeMismatchExceptions); }
            set { base.Result = value; }
        }
    }

    public abstract class TypedSentContext : TypedHandlerContext, IHandlerContextWithResult
    {
        private readonly Modifiable _result;

        protected TypedSentContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response)
            : base(context, request)
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

        public object Result
        {
            get { return _result.Value; }
            protected set { _result.Value = value; }
        }

        public override Modifiable GetHandlerResult()
        {
            return _result.ToResult();
        }

        object IHandlerContextWithResult.Result { set { Result = value; } }
    }
}