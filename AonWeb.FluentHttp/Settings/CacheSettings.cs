using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Settings
{
    public class CacheSettings : ICacheSettings
    {
        public CacheSettings()
        {
            HandlerRegister = new CacheHandlerRegister();
            Enabled = true;
            MustRevalidate = false;
            SuppressTypeMismatchExceptions = false;
            CacheableHttpMethods = new HashSet<HttpMethod> { HttpMethod.Get };
            CacheableHttpStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.OK };
            DefaultVaryByHeaders = new HashSet<string> { "Accept", "Accept-Encoding" };
            DefaultDurationForCacheableResults = TimeSpan.FromMinutes(15);
            ResponseValidator = (ctx, res) => CachingHelpers.ValidateResponse(res, ctx.CacheableHttpStatusCodes);
            RequestValidator = CachingHelpers.CanCacheRequest;
            RevalidateValidator = (ctx, res) => CachingHelpers.ShouldRevalidate(ctx, ctx.Request, res);
            AllowStaleResultValidator = (ctx, res) => CachingHelpers.AllowStale(ctx.Request, res);
            DependentUris = new HashSet<Uri>();
        }

        public ISet<HttpMethod> CacheableHttpMethods { get; }
        public ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        public ISet<string> DefaultVaryByHeaders { get; }
        public ISet<Uri> DependentUris { get; }
        public CacheHandlerRegister HandlerRegister { get; }
        public bool SuppressTypeMismatchExceptions { get; }
        public bool Enabled { get; set; }
        public Action<CacheEntry> ResultInspector { get; set; }
        public Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; set; }
        public Func<ICacheContext, bool> RequestValidator { get; set; }
        public Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; set; }
        public Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; set; }
        public TimeSpan? DefaultDurationForCacheableResults { get; set; }
        public bool MustRevalidate { get; set; }
        public TimeSpan? CacheDuration { get; set; }
    }
}