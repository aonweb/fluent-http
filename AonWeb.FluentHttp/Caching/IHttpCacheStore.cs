using System;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public interface IHttpCacheStore
    {
        Task<CacheResult> GetCachedResult(CacheContext context);

        Task AddOrUpdate(CacheContext context);

        bool TryRemove(CacheContext context);

        void Clear();

        void RemoveItem(Uri uri);
    }
}