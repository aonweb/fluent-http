using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public abstract class CacheHitContext : CacheHandlerContext
    {
        private readonly Modifiable<bool> _ignore;

        protected CacheHitContext(ICacheContext context, object result)
            : base(context)
        {
            _ignore = new Modifiable<bool>();
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

        public override Modifiable GetHandlerResult()
        {
            return _ignore;
        }
    }

    public class CacheHitContext<TResult> : CacheHitContext
    {
        public CacheHitContext(ICacheContext context, TResult result)
            : base(context,result) { }

        internal CacheHitContext(CacheHitContext context)
            : base(context) { }

        public TResult Result
        {
            get { return ObjectHelpers.CheckType<TResult>(ResultInternal, SuppressTypeMismatchExceptions); }
            set { ResultInternal = value; }
        }
    }
}