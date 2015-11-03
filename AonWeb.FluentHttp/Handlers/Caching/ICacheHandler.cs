using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheHandler: IHandler<CacheHandlerType>
    {
        Task OnHit(CacheHitContext context);
        Task OnMiss(CacheMissContext context);
        Task OnStore(CacheStoreContext context);
        Task OnExpiring(CacheExpiringContext context);
        Task OnExpired(CacheExpiredContext context);
    }

    public interface ITypedCacheHandler : ICacheHandler { }

    public interface IHttpCacheHandler : ICacheHandler { }
}