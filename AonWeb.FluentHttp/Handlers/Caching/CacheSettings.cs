using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheSettings : ICacheSettings
    {
        public CacheSettings()
        {
            Enabled = Defaults.Current.GetCachingDefaults().Enabled;
            CacheableHttpMethods = new HashSet<HttpMethod>(Defaults.Current.GetCachingDefaults().CacheableHttpMethods);
            CacheableHttpStatusCodes = new HashSet<HttpStatusCode>(Defaults.Current.GetCachingDefaults().CacheableHttpStatusCodes);
            DefaultVaryByHeaders = new HashSet<string>(Defaults.Current.GetCachingDefaults().VaryByHeaders);
            DependentUris = new HashSet<Uri>();
            DefaultDurationForCacheableResults = Defaults.Current.GetCachingDefaults().DefaultDurationForCacheableResults;
            Handler = new CacheHandlerRegister();
            CacheValidator = Defaults.Current.GetCachingDefaults().CacheValidator;
            RevalidateValidator = Defaults.Current.GetCachingDefaults().RevalidateValidator;
            ResponseValidator = Defaults.Current.GetCachingDefaults().ResponseValidator;
            AllowStaleResultValidator = Defaults.Current.GetCachingDefaults().AllowStaleResultValidator;
            SuppressTypeMismatchExceptions = Defaults.Current.GetTypedBuilderDefaults().SuppressTypeMismatchExceptions;
            CacheKeyBuilder = Defaults.Current.GetCachingDefaults().CacheKeyBuilderFactory?.Invoke() ?? new CacheKeyBuilder();
            MustRevalidate = Defaults.Current.GetCachingDefaults().MustRevalidate;
        }

        public ISet<HttpMethod> CacheableHttpMethods { get; }
        public ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        public ISet<string> DefaultVaryByHeaders { get; }
        public ISet<Uri> DependentUris { get; }
        public CacheHandlerRegister Handler { get; }

        public bool SuppressTypeMismatchExceptions { get; }

        public bool Enabled { get; set; }
        public Action<CacheResult> ResultInspector { get; set; }
        public Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; set; }
        public Func<ICacheContext, bool> CacheValidator { get; set; }
        public Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; set; }
        public Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; set; }
        public ICacheKeyBuilder CacheKeyBuilder { get; set; }
        public TimeSpan? DefaultDurationForCacheableResults { get; set; }
        public bool MustRevalidate { get; set; }
        public TimeSpan? CacheDuration { get; set; }

        public ISet<string> GetVaryByHeaders(Uri uri)
        {
            return CollectionHelpers.MergeSet(DefaultVaryByHeaders, Cache.CurrentVaryByStore.Get(uri));
        }
    }
}