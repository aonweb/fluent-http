using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheHandler : IHandler<CacheHandlerType>
    {
        Task OnHit<TResult>(CacheHitContext<TResult> context);
        Task OnMiss<TResult>(CacheMissContext<TResult> context);
        Task OnStore<TResult>(CacheStoreContext<TResult> context);
        Task OnExpiring(CacheExpiringContext context);
        Task OnExpired(CacheExpiredContext context);
    }
}