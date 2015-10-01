using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class ResponseInfo
    {
        public ResponseInfo(object result, HttpRequestMessage request, HttpResponseMessage response, ICacheContext context)
        {
            Uri = request.RequestUri;
            StatusCode = response.StatusCode;
            Date = response.Headers.Date ?? DateTimeOffset.UtcNow;
            LastModified = response.Content?.Headers?.LastModified;
            HasContent = response.Content != null;
            ETag = response.Headers.ETag;

            if (response.Headers.CacheControl != null)
            {
                NoStore = response.Headers.CacheControl.NoStore;
                NoCache = response.Headers.CacheControl.NoCache;
                ShouldRevalidate = context.MustRevalidateByDefault || response.Headers.CacheControl.MustRevalidate || NoCache;
            }

            CacheDuration = GetCacheDuration(result, context);
            Expiration = GetExpiration(LastModified ?? Date, response, CacheDuration);
            VaryHeaders = new HashSet<string>(response.Headers.Vary.Concat(context.DefaultVaryByHeaders).Distinct(StringComparer.OrdinalIgnoreCase));
            DependentUris = GetDependentUris(result, context.DependentUris);
        }

        public Uri Uri { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool HasContent { get; set; }
        public DateTimeOffset Date { get; set; }
        public EntityTagHeaderValue ETag { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public bool NoStore { get; set; }
        public bool NoCache { get; set; }
        public bool ShouldRevalidate { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        public ISet<string> VaryHeaders { get; }
        public ISet<Uri> DependentUris { get; }
        public TimeSpan? CacheDuration { get; }

        private static TimeSpan? GetCacheDuration(object result, ICacheContext context)
        {
            var cacheableResult = result as ICacheableHttpResult;

            return context.CacheDuration ?? (cacheableResult != null ? cacheableResult.Duration ?? context.DefaultDurationForCacheableResults : null);
        }

        private static DateTimeOffset? GetExpiration(DateTimeOffset lastModified, HttpResponseMessage response, TimeSpan? duration)
        {
            DateTimeOffset updatedLastModified;

            if (TryUpdateLastModified(duration, lastModified, out updatedLastModified))
                return updatedLastModified;

            if (response.Headers.CacheControl != null)
            {
                if (TryUpdateLastModified(response.Headers.CacheControl.MaxAge, lastModified, out updatedLastModified))
                    return updatedLastModified;

                if (TryUpdateLastModified(response.Headers.CacheControl.SharedMaxAge, lastModified, out updatedLastModified))
                    return updatedLastModified;
            }

            return response.Content?.Headers.Expires;
        }

        private static bool TryUpdateLastModified(TimeSpan? duration, DateTimeOffset originalLastModified, out DateTimeOffset lastModified)
        {
            lastModified = originalLastModified;

            if (!duration.HasValue || duration.Value <= TimeSpan.Zero)
                return false;

            lastModified = originalLastModified.Add(duration.Value);

            return true;
        }

        private static ISet<Uri> GetDependentUris(object result, IEnumerable<Uri> dependentUris)
        {
            var uris = dependentUris;

            var cacheableResult = result as ICacheableHttpResult;
            if (cacheableResult != null)
            {
                uris = uris.Concat(cacheableResult.DependentUris);
            }

            return new HashSet<Uri>(uris.NormalizeUris());
        }

        public void UpdateExpiration(object result, HttpResponseMessage response, ICacheContext context)
        {
            var lastModified = response.Headers.Date ?? DateTimeOffset.UtcNow;

            if (response.Content?.Headers.LastModified != null)
                lastModified = response.Content.Headers.LastModified.Value;

            var newExpiration = GetExpiration(lastModified, response, CacheDuration);

            if (newExpiration > Expiration) 
                Expiration = newExpiration;
        }

        public void Merge(ResponseInfo responseInfo)
        {
            if (responseInfo.Date > Date) 
                Date = responseInfo.Date;

            if (responseInfo.LastModified > LastModified)
                LastModified = responseInfo.LastModified;

            if (responseInfo.ShouldRevalidate) 
                ShouldRevalidate = responseInfo.ShouldRevalidate;

           if (responseInfo.ETag != null)
               ETag = responseInfo.ETag;

           if (responseInfo.Expiration > Expiration)
               Expiration = responseInfo.Expiration;

            foreach (var header in responseInfo.VaryHeaders.Where(header => !VaryHeaders.Contains(header)))
            {
                VaryHeaders.Add(header);
            }

            foreach (var uri in responseInfo.DependentUris.Where(uri => !DependentUris.Contains(uri)))
            {
                DependentUris.Add(uri);
            }
        }
    }
}