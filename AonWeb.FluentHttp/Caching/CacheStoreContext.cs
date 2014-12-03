using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public abstract class CacheStoreContext : CacheHandlerContext
    {
        protected CacheStoreContext(CacheContext context, object result)
            : base(context)
        {
            ResultInternal = result;
        }

        protected CacheStoreContext(CacheStoreContext context)
            : base(context)
        {
            ResultInternal = context.ResultInternal;
        }

        protected object ResultInternal { get; private set; }

        public override ModifyTracker GetHandlerResult()
        {
            return new ModifyTracker();
        }
    }

    public class CacheStoreContext<TResult> : CacheStoreContext
    {
        public CacheStoreContext(CacheContext context, TResult result)
            : base(context,result) { }

        internal CacheStoreContext(CacheStoreContext context)
            : base(context) { }

        public TResult Result
        {
            get { return (TResult)ResultInternal; }
        }
    }
}