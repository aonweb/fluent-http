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
        public ResponseInfo(object result, HttpResponseMessage response, ICacheContext context)
        {
            Uri = response.RequestMessage.RequestUri;
            StatusCode = response.StatusCode;
            Date = response.Headers.Date ?? DateTimeOffset.UtcNow;

            if (response.Content != null)
            {
                HasContent = true;
                LastModified = response.Content.Headers.LastModified;
            }

            if (response.Headers.CacheControl != null)
            {
                NoStore = response.Headers.CacheControl.NoStore;
                NoCache = response.Headers.CacheControl.NoCache;
                ShouldRevalidate = context.MustRevalidateByDefault || response.Headers.CacheControl.MustRevalidate || NoCache;
            }

            ETag = response.Headers.ETag;

            var lastModified = LastModified ?? Date;
            var expiration = GetExpiration(lastModified, result, response, context.DefaultExpiration);
            HasExpiration = expiration.HasValue;
            Expiration = expiration ?? lastModified.Add(context.DefaultExpiration);

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
        public DateTimeOffset Expiration { get; set; }
        public bool HasExpiration { get; set; }
        public ISet<string> VaryHeaders { get; }
        public ISet<Uri> DependentUris { get; }
        

        private static DateTimeOffset? GetExpiration(DateTimeOffset lastModified, object result, HttpResponseMessage response, TimeSpan defaultExpiration)
        {
            var cacheableResult = result as ICacheableHttpResult;
            if (cacheableResult != null)
            {
                if (cacheableResult.Duration.HasValue && cacheableResult.Duration.Value > TimeSpan.Zero)
                    return lastModified.Add(cacheableResult.Duration.Value);

                return lastModified.Add(defaultExpiration);
            }

            if (response.Headers.CacheControl != null)
            {
                if (response.Headers.CacheControl.MaxAge.HasValue)
                    return lastModified.Add(response.Headers.CacheControl.MaxAge.Value);

                if (response.Headers.CacheControl.SharedMaxAge.HasValue)
                    return lastModified.Add(response.Headers.CacheControl.SharedMaxAge.Value);
            }

            return response.Content?.Headers.Expires;
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

        public void UpdateExpiration(object result, HttpResponseMessage response, TimeSpan defaultExpiration)
        {
            var lastModified = response.Headers.Date ?? DateTimeOffset.UtcNow;

            if (response.Content?.Headers.LastModified != null)
                lastModified = response.Content.Headers.LastModified.Value;

            var newExpiration = GetExpiration(lastModified, result, response, defaultExpiration);

            if (newExpiration.HasValue && newExpiration > Expiration) 
                Expiration = newExpiration.Value;
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

           if (responseInfo.HasExpiration)
               HasExpiration = responseInfo.HasExpiration;

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