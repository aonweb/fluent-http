using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheMissContext<TResult> : CacheHandlerContext
    {
        private readonly ModifyTracker<TResult> _result;

        public CacheMissContext(CacheContext context)
            : base(context)
        {
            _result = new ModifyTracker<TResult>();
        }

        public TResult Result
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public override ModifyTracker GetHandlerResult()
        {
            return _result;
        }
    }
}