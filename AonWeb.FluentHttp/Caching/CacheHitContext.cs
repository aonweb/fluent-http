using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public abstract class CacheHitContext : CacheHandlerContext
    {
        private readonly ModifyTracker<bool> _ignore;

        protected CacheHitContext(CacheContext context, object result)
            : base(context)
        {
            _ignore = new ModifyTracker<bool>();
            ResultInternal = result;
        }

        protected CacheHitContext(CacheHitContext context)
            : base(context)
        {
            _ignore = context._ignore;
            ResultInternal = context.ResultInternal;
        }

        public bool Ignore
        {
            get { return _ignore.Value; }
            set { _ignore.Value = value; }
        }

        protected object ResultInternal { get; set; }

        public override ModifyTracker GetHandlerResult()
        {
            return _ignore;
        }
    }

    public class CacheHitContext<TResult> : CacheHitContext
    {
        public CacheHitContext(CacheContext context, TResult result)
            : base(context,result) { }

        internal CacheHitContext(CacheHitContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }
    }
}