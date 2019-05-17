using System;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public interface IUriInfoProvider
    {
        Task<UriCacheInfo> Get(Uri uri);
        Task<bool> Put(Uri uri, CacheKey cacheKey, TimeSpan? expiration);
        Task<bool> DeleteKey(Uri uri, CacheKey cacheKey);
        Task<bool> Delete(Uri uri);
        Task DeleteAll();
    }
}