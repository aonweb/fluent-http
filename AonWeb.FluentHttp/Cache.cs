using System;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp
{
    public static class Cache
    {
        private static Lazy<ICacheProvider> _cacheStore;

        static Cache()
        {
            SetProvider(() => new InMemoryCacheProvider(new InMemoryVaryByProvider()));
        }

        public static void Clear()
        {
            _cacheStore.Value.Clear();
        }

        public static void Remove(string uri)
        {
           Remove(new Uri(uri));
        }

        public static void Remove(Uri uri)
        {
            _cacheStore.Value.Remove(uri);
        }

        public static void SetProvider(Func<ICacheProvider> cacheFactory)
        {
            _cacheStore = new Lazy<ICacheProvider>(cacheFactory, true);
        }

        internal static ICacheProvider Current => _cacheStore.Value;
    }
}