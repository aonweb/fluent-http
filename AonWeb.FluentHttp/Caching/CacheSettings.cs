using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheSettings<TResult>
    {
        public CacheSettings()
            : this(HttpCallBuilderDefaults.DefaultCacheStoreFactory(), HttpCallBuilderDefaults.DefaultVaryByStoreFactory()) { }

        public CacheSettings(IHttpCacheStore cacheStore, IVaryByStore varyByStore)
        {
            Enabled = HttpCallBuilderDefaults.CachingEnabled;
            CacheStore = cacheStore;
            VaryByStore = varyByStore;
            CacheableMethods = new HashSet<HttpMethod>(HttpCallBuilderDefaults.DefaultCacheableMethods);
            CacheableStatusCodes = new HashSet<HttpStatusCode>(HttpCallBuilderDefaults.DefaultCacheableStatusCodes);
            DefaultVaryByHeaders = new HashSet<string>(HttpCallBuilderDefaults.DefaultVaryByHeaders);
            DependentUris = new HashSet<string>();
            DefaultExpiration = HttpCallBuilderDefaults.DefaultCacheExpiration;
        }

        public IHttpCacheStore CacheStore { get; private set; }
        public IVaryByStore VaryByStore { get; private set; }
        public ISet<HttpMethod> CacheableMethods { get; private set; }
        public ISet<HttpStatusCode> CacheableStatusCodes { get; private set; }
        public ISet<string> DefaultVaryByHeaders { get; private set; }
        public ISet<string> DependentUris { get; private set; }
        public bool Enabled { get; set; }

        public Action<CacheResult<TResult>> ResultInspector { get; set; }

        public TimeSpan DefaultExpiration { get; set; }

        public bool MustRevalidateByDefault { get; set; }

        public ISet<string> GetVaryByHeaders(Uri uri)
        {
            return Helper.MergeSet(DefaultVaryByHeaders, VaryByStore.Get(uri));
        }
    }
}