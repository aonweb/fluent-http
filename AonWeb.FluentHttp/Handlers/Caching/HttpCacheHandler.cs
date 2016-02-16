using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public abstract class HttpCacheHandler : IHttpCacheHandler
    {
        public virtual bool Enabled { get; protected set; } = true;

        public virtual HandlerPriority GetPriority(CacheHandlerType type)
        {
            return HandlerPriority.Default;
        }

        public virtual Task OnHit(CacheHitContext context)
        {
            return Task.FromResult(true);
        }

        public virtual Task OnMiss(CacheMissContext context)
        {
            return Task.FromResult(true);
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