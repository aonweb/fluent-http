namespace AonWeb.FluentHttp.Handlers.Caching
{
    public abstract class CacheStoreContext : CacheHandlerContext
    {
        protected CacheStoreContext(ICacheContext context, object result)
            : base(context)
        {
            ResultInternal = result;
        }

        protected CacheStoreContext(CacheStoreContext context)
            : base(context)
        {
            ResultInternal = context.ResultInternal;
        }

        protected object ResultInternal { get; }

        public override Modifiable GetHandlerResult()
        {
            return new Modifiable();
        }
    }

    public class CacheStoreContext<TResult> : CacheStoreContext
    {
        public CacheStoreContext(ICacheContext context, TResult result)
            : base(context,result) { }

        internal CacheStoreContext(CacheStoreContext context)
            : base(context) { }

        public TResult Result => (TResult)ResultInternal;
    }
}