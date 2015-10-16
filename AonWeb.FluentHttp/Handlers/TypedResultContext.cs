using System.Net.Http;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedResultContext<TResult> : TypedResultContext, IHandlerContextWithResult<TResult>
    {
        public TypedResultContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, TResult result)
            : base(context, request, response, result)
        { }

        internal TypedResultContext(TypedResultContext context)
            : base(context)
        { }

        public new TResult Result
        {
            get { return TypeHelpers.CheckType<TResult>(base.Result, SuppressTypeMismatchExceptions); }
            set { base.Result = value; }
        }
    }

    public class TypedResultContext: TypedHandlerContext, IHandlerContextWithResult
    {
        private readonly Modifiable _result;

        protected TypedResultContext(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, object result)
            : base(context, request)
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