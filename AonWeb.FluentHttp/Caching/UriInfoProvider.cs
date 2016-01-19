using System;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class UriInfoProvider : IUriInfoProvider
    {
        private readonly ICacheProvider _cache;

        public UriInfoProvider(ICacheProvider cache)
        {
            _cache = cache;
        }

        public async Task<UriCacheInfo> Get(Uri uri)
        {
            if (uri == null)
                return new UriCacheInfo();

            var key = GetKey(uri);

            return await _cache.Get<UriCacheInfo>(key);
        }

        public async Task<bool> Put(Uri uri, CacheKey cacheKey)
        {
            if (uri == null || cacheKey == CacheKey.Empty)
                return false;

            var key = GetKey(uri);

            var cacheInfo = await _cache.Get<UriCacheInfo>(key) ?? new UriCacheInfo();

            lock (cacheInfo)
            {
                if (!cacheInfo.CacheKeys.Contains(cacheKey))
                    cacheInfo.CacheKeys.Add(cacheKey);
            }

            return await _cache.Put(key, cacheInfo).ConfigureAwait(false);
        }

        public async Task<bool> DeleteKey(Uri uri, CacheKey cacheKey)
        {
            if (uri == null || cacheKey == CacheKey.Empty)
                return false;

            var key = GetKey(uri);

            var cacheInfo = await _cache.Get<UriCacheInfo>(key);

            if (cacheInfo == null)
                return false;

            lock (cacheInfo)
            {
                if (!cacheInfo.CacheKeys.Contains(cacheKey))
                    return false;

                cacheInfo.CacheKeys.Remove(cacheKey);
            }

            return await _cache.Put(key, cacheInfo).ConfigureAwait(false); ;
        }

        public async Task<bool> Delete(Uri uri)
        {
            if (uri == null)
                return false;

            var key = GetKey(uri);

            return await _cache.Delete(key).ConfigureAwait(false);
        }

        public Task DeleteAll()
        {
            return _cache.DeleteAll();
        }

        private static string GetKey(Uri uri)
        {
            var hash = DigestHelpers.Sha256Hash(Encoding.UTF8.GetBytes(uri.Normalize().ToString()));

            return "UriInfo:" + Convert.ToBase64String(hash);
        }
    }
}