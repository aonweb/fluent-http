using System;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp
{
    public static class Cache
    {
        private static readonly Lazy<IHttpCacheStore> CacheStore = new Lazy<IHttpCacheStore>(() => Defaults.Current.GetCachingDefaults().CacheStoreFactory(), true);
        private static readonly Lazy<IVaryByStore> VaryStore = new Lazy<IVaryByStore>(() => Defaults.Current.GetCachingDefaults().VaryByStoreFactory(), true);
       
        public static void Clear()
        {
            CurrentCacheStore.Clear();
        }

        public static void Remove(string uri)
        {
           Remove(new Uri(uri));
        }

        public static void Remove(Uri uri)
        {
            CurrentCacheStore.RemoveItem(uri);
        }

        internal static IHttpCacheStore CurrentCacheStore => CacheStore.Value;

        internal static IVaryByStore CurrentVaryByStore => VaryStore.Value;
    }
}