using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheHitContext<TResult> : CacheHitContext
    {
        public CacheHitContext(ICacheContext context, TResult result)
            : base(context, result)
        { }

        internal CacheHitContext(CacheHitContext context)
            : base(context)
        { }

        public new TResult Result
        {
            get { return TypeHelpers.CheckType<TResult>(base.Result, SuppressTypeMismatchExceptions); }
            set { base.Result = value; }
        }
    }

    public abstract class CacheHitContext : CacheHandlerContext
    {
        private readonly Modifiable<bool> _ignore;

        protected CacheHitContext(ICacheContext context, object result)
            : base(context)
        {
            _ignore = new Modifiable<bool>(false);
            Result = result;
        }

        protected CacheHitContext(CacheHitContext context)
            : base(context)
        {
            _ignore = context._ignore;
            Result = context.Result;
        }

        public bool Ignore
        {
            get { return _ignore.Value; }
            set { _ignore.Value = value; }
        }

        public object Result { get; set; }

        public override Modifiable GetHandlerResult()
        {
            return _ignore;
        }
    } 
}