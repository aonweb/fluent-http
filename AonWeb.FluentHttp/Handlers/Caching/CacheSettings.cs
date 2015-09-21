using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheContext : IContextWithSettings<ICacheSettings>, IContextWithResult<CacheResult>
    {
        bool Enabled { get; }
        ISet<Uri> DependentUris { get; }
        HttpRequestMessage Request { get; }
        Uri Uri { get; }
        new CacheResult Result { get; set; }
        bool MustRevalidateByDefault { get; }
        TimeSpan DefaultExpiration { get; }
        ISet<string> DefaultVaryByHeaders { get; }
        Action<CacheResult> ResultInspector { get; }
        Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator { get; }
        Func<ICacheContext, bool> CacheValidator { get; }
        Func<ICacheContext, ResponseInfo, bool> RevalidateValidator { get; }
        Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator { get; }
        CacheHandlerRegister Handler { get; }
        ISet<HttpMethod> CacheableHttpMethods { get; }
        ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        ResponseValidationResult ValidationResult { get; set; }
        IHandlerContext GetHandlerContext();
    }

    public interface ICacheSettings
    {
        bool Enabled { get; }
        ISet<Uri> DependentUris { get; }
        CacheHandlerRegister Handler { get; }
        ISet<HttpMethod> CacheableHttpMethods { get; }
        ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        ISet<string> DefaultVaryByHeaders { get; }
        TimeSpan DefaultExpiration { get; set; }
        bool MustRevalidateByDefault { get; set; }
        Action<CacheResult> ResultInspector { get; set; }
        Func<ICacheContext, bool> CacheValidator { get; set; }
        Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator { get; set; }
        Func<ICacheContext, ResponseInfo, bool> RevalidateValidator { get; set; }
        Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator { get; set; }
    }


    public class CacheSettings : ICacheSettings
    {
        public CacheSettings()
        {
            Enabled = Defaults.Caching.Enabled;
            CacheableHttpMethods = new HashSet<HttpMethod>(Defaults.Caching.CacheableHttpMethods);
            CacheableHttpStatusCodes = new HashSet<HttpStatusCode>(Defaults.Caching.CacheableHttpStatusCodes);
            DefaultVaryByHeaders = new HashSet<string>(Defaults.Caching.VaryByHeaders);
            DependentUris = new HashSet<Uri>();
            DefaultExpiration = Defaults.Caching.DefaultCacheExpiration;
            Handler = new CacheHandlerRegister();
            CacheValidator = Defaults.Caching.CacheValidator;
            RevalidateValidator = Defaults.Caching.RevalidateValidator;
            ResponseValidator = Defaults.Caching.ResponseValidator;
            AllowStaleResultValidator = Defaults.Caching.AllowStaleResultValidator;
        }

        public ISet<HttpMethod> CacheableHttpMethods { get; }
        public ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        public ISet<string> DefaultVaryByHeaders { get; }
        public ISet<Uri> DependentUris { get; }
        public CacheHandlerRegister Handler { get; }



        public bool Enabled { get; set; }
        public Action<CacheResult> ResultInspector { get; set; }
        public Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator { get; set; }
        public Func<ICacheContext, bool> CacheValidator { get; set; }
        public Func<ICacheContext, ResponseInfo, bool> RevalidateValidator { get; set; }
        public Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator { get; set; }
        public TimeSpan DefaultExpiration { get; set; }
        public bool MustRevalidateByDefault { get; set; }

        public ISet<string> GetVaryByHeaders(Uri uri)
        {
            return CollectionHelpers.MergeSet(DefaultVaryByHeaders, Cache.CurrentVaryByStore.Get(uri));
        }
    }
}