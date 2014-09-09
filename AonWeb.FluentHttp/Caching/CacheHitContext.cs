using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheHitContext<TResult> : CacheHandlerContext
    {
        private readonly ModifyTracker<bool> _ignore;

        public CacheHitContext(CacheContext context, TResult result)
            : base(context)
        {
            _ignore = new ModifyTracker<bool>();
            Result = result;
        }

        public bool Ignore
        {
            get { return _ignore.Value; }
            set { _ignore.Value = value; }
        }

        public TResult Result { get; private set; }

        public override ModifyTracker GetHandlerResult()
        {
            return _ignore;
        }
    }
}