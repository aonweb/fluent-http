using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheSettings<TResult> : CacheSettings
    {
        public CacheSettings()
            : base(typeof(TResult),
                HttpCallBuilderDefaults.DefaultCacheStoreFactory(),
                HttpCallBuilderDefaults.DefaultVaryByStoreFactory())
        {
            CacheValidator = CacheHandlerDefaults.CacheValidator;
            RevalidateValidator = CacheHandlerDefaults.RevalidateValidator;
            ResponseValidator = CacheHandlerDefaults.ResponseValidator;
            AllowStaleResultValidator = CacheHandlerDefaults.AllowStaleResultValidator;
        }

        public Action<CacheResult<TResult>> CacheResultConfiguration { get; set; }
        public Func<CacheContext<TResult>, ResponseValidationResult> ResponseValidator { get; set; }
        public Func<CacheContext<TResult>, bool> CacheValidator { get; set; }
        public Func<CacheContext<TResult>, bool> RevalidateValidator { get; set; }
        public Func<CacheContext<TResult>, bool> AllowStaleResultValidator { get; set; }
    }

    public abstract class CacheSettings
    {
        protected CacheSettings(Type resultType, IHttpCacheStore cacheStore, IVaryByStore varyByStore)
        {
            ResultType = resultType;
            CacheStore = cacheStore;
            VaryByStore = varyByStore;
            Enabled = HttpCallBuilderDefaults.CachingEnabled;
            CacheableMethods = new HashSet<HttpMethod>(HttpCallBuilderDefaults.DefaultCacheableMethods);
            CacheableStatusCodes = new HashSet<HttpStatusCode>(HttpCallBuilderDefaults.DefaultCacheableStatusCodes);
            DefaultVaryByHeaders = new HashSet<string>(HttpCallBuilderDefaults.DefaultVaryByHeaders);
            DependentUris = new HashSet<Uri>();
            DefaultExpiration = HttpCallBuilderDefaults.DefaultCacheExpiration;
        }

        public bool Enabled { get; set; }
        public Type ResultType { get; private set; }
        public IHttpCacheStore CacheStore { get; private set; }
        public IVaryByStore VaryByStore { get; private set; }
        public ISet<HttpMethod> CacheableMethods { get; private set; }
        public ISet<HttpStatusCode> CacheableStatusCodes { get; private set; }
        public ISet<string> DefaultVaryByHeaders { get; private set; }
        public ISet<Uri> DependentUris { get; private set; }
        public TimeSpan DefaultExpiration { get; set; }
        public bool MustRevalidateByDefault { get; set; }

        public ISet<string> GetVaryByHeaders(Uri uri)
        {
            return Helper.MergeSet(DefaultVaryByHeaders, VaryByStore.Get(uri));
        }
    }
}