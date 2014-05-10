using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public interface IHttpCacheStore
    {
        Task<CacheResult<T>> GetCachedResult<T>(CacheContext<T> context);

        Task AddOrUpdate<T>(CacheContext<T> context);

        bool TryRemove<T>(CacheContext<T> context);

        void Clear();

        void RemoveItem(Uri uri);
    }
}