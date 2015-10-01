using System.Net.Http;
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

        public Task OnHit(CacheHitContext<HttpResponseMessage> context)
        {
            return OnHit((CacheHitContext)context);
        }

        public Task OnHit(CacheHitContext context)
        {
            return Task.FromResult(true);
        }

        public Task OnMiss(CacheMissContext<HttpResponseMessage> context)
        {
            return OnMiss((CacheMissContext)context);
        }

        public Task OnMiss(CacheMissContext context)
        {
            return Task.FromResult(true);
        }

        public Task OnStore(CacheStoreContext<HttpResponseMessage> context)
        {
            return OnStore((CacheStoreContext)context);
        }

        public Task OnStore(CacheStoreContext context)
        {
            return Task.FromResult(true);
        }

        public Task OnExpiring(CacheExpiringContext context)
        {
            return Task.FromResult(true);
        }

        public Task OnExpired(CacheExpiredContext context)
        {
            return Task.FromResult(true);
        }

        

        

        
    }
}