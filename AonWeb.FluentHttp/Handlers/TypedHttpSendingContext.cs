using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class TypedHttpSendingContext : TypedHttpCallHandlerContext
    {
        private readonly ModifyTracker _result;

        protected TypedHttpSendingContext(TypedHttpCallContext context, HttpRequestMessage request, object content, bool hasContent)
            : base(context)
        {
            Request = request;
            _result = new ModifyTracker();
            ContentInternal = content;
            HasContent = hasContent;
        }

        protected TypedHttpSendingContext(TypedHttpSendingContext context)
            : base(context)
        {
            Request = context.Request;
            _result = context._result;
            ContentInternal = context.ContentInternal;
            HasContent = context.HasContent;
        }

        public HttpRequestMessage Request { get; private set; }

        protected object ResultInternal
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public object ContentInternal { get; private set; }
        public bool HasContent { get; private set; }

        public override ModifyTracker GetHandlerResult()
        {
            return _result.ToResult();
        }
    }

    public class TypedHttpSendingContext<TResult, TContent> : TypedHttpSendingContext
    {
        public TypedHttpSendingContext(TypedHttpCallContext context, HttpRequestMessage request, TContent content, bool hasContent)
            : base(context, request, content, hasContent) { }

        internal TypedHttpSendingContext(TypedHttpSendingContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }

        public TContent Content { get { return (TContent)ContentInternal; } }
    }
}