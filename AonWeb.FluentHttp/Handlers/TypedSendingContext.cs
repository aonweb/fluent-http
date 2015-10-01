using System.Net.Http;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class TypedSendingContext<TResult, TContent> : TypedSendingContext, IHandlerContextWithResult<TResult>
    {
        public TypedSendingContext(ITypedBuilderContext context, HttpRequestMessage request, TContent content, bool hasContent)
            : base(context, request, content, hasContent)
        { }

        internal TypedSendingContext(TypedSendingContext context)
            : base(context)
        { }

        public new TResult Result
        {
            get { return ObjectHelpers.CheckType<TResult>(base.Result, SuppressTypeMismatchExceptions); }
            set { base.Result = value; }
        }

        public new TContent Content => ObjectHelpers.CheckType<TContent>(base.Content, SuppressTypeMismatchExceptions);
    }

    public abstract class TypedSendingContext : TypedHandlerContext, IHandlerContextWithResult
    {
        private readonly Modifiable _result;

        protected TypedSendingContext(ITypedBuilderContext context, HttpRequestMessage request, object content, bool hasContent)
            : base(context, request)
        {
            _result = new Modifiable();
            Content = content;
            HasContent = hasContent;
        }

        protected TypedSendingContext(TypedSendingContext context)
            : base(context)
        {
            _result = context._result;
            Content = context.Content;
            HasContent = context.HasContent;
        }

        public object Result
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public object Content { get; }
        public bool HasContent { get; }

        public override Modifiable GetHandlerResult()
        {
            return _result.ToResult();
        }

        object IHandlerContextWithResult.Result { set { Result = value; } }
    }
}