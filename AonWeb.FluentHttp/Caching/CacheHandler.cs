using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public abstract class CacheHandler : ICacheHandler
    {
        private bool _enabled = true;

        public virtual bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public virtual HttpCallHandlerPriority GetPriority(CacheHandlerType type)
        {
            return HttpCallHandlerPriority.Default;
        }

        public virtual Task OnHit<TResult>(CacheHitContext<TResult> context) { return Task.Delay(0); }
        public virtual Task OnMiss<TResult>(CacheMissContext<TResult> context) { return Task.Delay(0); }
        public virtual Task OnStore<TResult>(CacheStoreContext<TResult> context) { return Task.Delay(0); }
        public virtual Task OnExpiring(CacheExpiringContext context) { return Task.Delay(0); }
        public virtual Task OnExpired(CacheExpiredContext context) { return Task.Delay(0); }
    }
}