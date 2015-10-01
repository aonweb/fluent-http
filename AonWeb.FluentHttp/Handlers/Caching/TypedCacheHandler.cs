using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public abstract class TypedCacheHandler : ITypedCacheHandler
    {
        public virtual bool Enabled { get; protected set; } = true;

        public virtual HandlerPriority GetPriority(CacheHandlerType type)
        {
           return HandlerPriority.Default;
        }

        public virtual Task OnHit<TResult>(CacheHitContext<TResult> context)
        {
            return OnHit((CacheHitContext)context);
        }

        public virtual Task OnHit(CacheHitContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnMiss<TResult>(CacheMissContext<TResult> context)
        {
            return OnMiss((CacheMissContext)context);
        }

        public virtual Task OnMiss(CacheMissContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnStore<TResult>(CacheStoreContext<TResult> context)
        {
            return OnStore((CacheStoreContext)context);
        }

        public virtual Task OnStore(CacheStoreContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnExpiring(CacheExpiringContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnExpired(CacheExpiredContext context)
        {
            return Task.FromResult(true);
        }
    }
}