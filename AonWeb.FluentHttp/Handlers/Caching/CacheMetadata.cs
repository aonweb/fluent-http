using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheMetadata : ICacheMetadata
    {
        public CacheMetadata()
        {
            Enabled = true;
            DependentUris = new HashSet<Uri>();
            CacheDuration = null;
            CacheableHttpMethods = new HashSet<HttpMethod>();
            CacheableHttpStatusCodes = new HashSet<HttpStatusCode>();
            DefaultVaryByHeaders = new HashSet<string>();
            SuppressTypeMismatchExceptions = false;
            DefaultDurationForCacheableResults = null;
            MustRevalidate = true;
        }

        public CacheMetadata(ICacheMetadata context)
            : this()
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Enabled = context.Enabled;
            DependentUris.Merge(context.DependentUris);
            CacheDuration = context.CacheDuration;
            CacheableHttpMethods.Merge(context.CacheableHttpMethods);
            CacheableHttpStatusCodes.Merge(context.CacheableHttpStatusCodes);
            DefaultVaryByHeaders.Merge(context.DefaultVaryByHeaders);
            SuppressTypeMismatchExceptions = context.SuppressTypeMismatchExceptions;
            DefaultDurationForCacheableResults = context.DefaultDurationForCacheableResults;
            MustRevalidate = context.MustRevalidate;
        }

        public bool Enabled { get; }
        public ISet<Uri> DependentUris { get; }
        public TimeSpan? CacheDuration { get; }
        public ISet<HttpMethod> CacheableHttpMethods { get; }
        public ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        public ISet<string> DefaultVaryByHeaders { get; }
        public bool SuppressTypeMismatchExceptions { get; }
        public TimeSpan? DefaultDurationForCacheableResults { get; }
        public bool MustRevalidate { get; }
    }
}