using System;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace AonWeb.FluentHttp
{
    public static class Cache
    {
        private static Lazy<ICacheManager> _cacheManagerStore;

        static Cache()
        {
            // TODO: MemoryCacheOptions should be configurable
            SetManager(() => new CacheManager(
                new CacheProvider(new MemoryCacheOptions()), 
                new VaryByProvider(new CacheProvider(new MemoryCacheOptions())),
                new UriInfoProvider(new CacheProvider(new MemoryCacheOptions())),
                new ResponseSerializer()));
        }

        public static Task DeleteAll()
        {
           return _cacheManagerStore.Value.DeleteAll();
        }

        public static Task Delete(string uri)
        {
           return Delete(new Uri(uri));
        }

        public static Task Delete(Uri uri)
        {
           return _cacheManagerStore.Value.Delete(uri);
        }

        public static void SetManager(Func<ICacheManager> cacheManagerFactory)
        {
            _cacheManagerStore = new Lazy<ICacheManager>(cacheManagerFactory);
        }

        internal static ICacheManager Current => _cacheManagerStore.Value;
    }
}