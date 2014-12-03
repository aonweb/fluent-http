using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public abstract class CacheMissContext : CacheHandlerContext
    {
        private readonly ModifyTracker _result;

        protected CacheMissContext(CacheContext context)
            : base(context)
        {
            _result = new ModifyTracker();
        }

        protected CacheMissContext(CacheMissContext context)
            : base(context)
        {
            _result = context._result;
        }

        protected object ResultInternal
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public override ModifyTracker GetHandlerResult()
        {
            return _result;
        }
    }

    public class CacheMissContext<TResult> : CacheMissContext
    {
        public CacheMissContext(CacheContext context)
            : base(context) { }

        internal CacheMissContext(CacheMissContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
            set { ResultInternal = value; }
        }
    }
}