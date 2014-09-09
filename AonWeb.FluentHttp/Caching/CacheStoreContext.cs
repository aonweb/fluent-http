using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheStoreContext<TResult> : CacheHandlerContext
    {
        public CacheStoreContext(CacheContext context, TResult result)
            : base(context)
        {
            Result = result;
        }

        public TResult Result { get; private set; }

        public override ModifyTracker GetHandlerResult()
        {
            return new ModifyTracker();
        }
    }
}