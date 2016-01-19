using System;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp
{
    public static class Cache
    {
        private static Lazy<ICacheManager> _cacheManagerStore;

        static Cache()
        {
            SetManager(() => new CacheManager(
                new CacheProvider(), 
                new VaryByProvider(new CacheProvider()),
                new UriInfoProvider(new CacheProvider()),
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