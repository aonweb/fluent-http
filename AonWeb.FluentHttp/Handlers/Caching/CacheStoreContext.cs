using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public abstract class CacheStoreContext : CacheHandlerContext
    {
        protected CacheStoreContext(ICacheContext context, object result)
            : base(context)
        {
            Result = result;
        }

        protected CacheStoreContext(CacheStoreContext context)
            : base(context)
        {
            Result = context.Result;
        }

        public object Result { get; }

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

        public new TResult Result => ObjectHelpers.CheckType<TResult>(base.Result, SuppressTypeMismatchExceptions);
        
    }
}