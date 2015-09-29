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

        public TResult Result
        {
            get { return ObjectHelpers.CheckType<TResult>(ResultInternal, SuppressTypeMismatchExceptions); }
            set { ResultInternal = value; }
        }

        public TContent Content => ObjectHelpers.CheckType<TContent>(ContentInternal, SuppressTypeMismatchExceptions);
    }

    public abstract class TypedSendingContext : TypedHandlerContext, IHandlerContextWithResult
    {
        private readonly Modifiable _result;

        protected TypedSendingContext(ITypedBuilderContext context, HttpRequestMessage request, object content, bool hasContent)
            : base(context, request)
        {
            _result = new Modifiable();
            ContentInternal = content;
            HasContent = hasContent;
        }

        protected TypedSendingContext(TypedSendingContext context)
            : base(context)
        {
            _result = context._result;
            ContentInternal = context.ContentInternal;
            HasContent = context.HasContent;
        }

        protected object ResultInternal
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public object ContentInternal { get; private set; }
        public bool HasContent { get; private set; }

        public override Modifiable GetHandlerResult()
        {
            return _result.ToResult();
        }

        object IHandlerContextWithResult.Result { set { ResultInternal = value; } }
    }
}