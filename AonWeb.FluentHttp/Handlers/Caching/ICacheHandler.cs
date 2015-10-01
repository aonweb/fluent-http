using System.Net.Http;
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

    public interface ITypedCacheHandler : ICacheHandler
    {
        Task OnHit<TResult>(CacheHitContext<TResult> context);
        Task OnMiss<TResult>(CacheMissContext<TResult> context);
        Task OnStore<TResult>(CacheStoreContext<TResult> context);
    }

    public interface IHttpCacheHandler : ICacheHandler
    {
        Task OnHit(CacheHitContext<HttpResponseMessage> context);
        Task OnMiss(CacheMissContext<HttpResponseMessage> context);
        Task OnStore(CacheStoreContext<HttpResponseMessage> context);
    }
}