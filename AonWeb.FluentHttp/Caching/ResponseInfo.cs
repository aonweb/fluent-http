using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Caching
{
    public class ResponseInfo
    {
        public ResponseInfo(object result,
            HttpResponseMessage response,
            TimeSpan defaultExpiration,
            IEnumerable<string> defaultVaryByHeaders,
            bool mustRevalidateByDefault,
            IEnumerable<string> dependentUris)
        {
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
                ShouldRevalidate = mustRevalidateByDefault || response.Headers.CacheControl.MustRevalidate || NoCache;
            }

            ETag = response.Headers.ETag;

            var lastModified = LastModified ?? Date;
            var expiration = GetExpiration(lastModified, result, response, defaultExpiration);
            HasExpiration = expiration.HasValue;
            Expiration = expiration ?? lastModified.Add(defaultExpiration);

            VaryHeaders = new HashSet<string>(response.Headers.Vary.Concat(defaultVaryByHeaders).Distinct(StringComparer.OrdinalIgnoreCase));
            DependentUris = GetDependentUris(result, dependentUris);
        }

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

        public ISet<string> VaryHeaders { get; private set; }

        public ISet<string> DependentUris { get; private set; }

        private DateTimeOffset? GetExpiration(DateTimeOffset lastModified, object result, HttpResponseMessage response, TimeSpan defaultExpiration)
        {
            if (response.Headers.CacheControl != null)
            {
                if (response.Headers.CacheControl.MaxAge.HasValue)
                    return lastModified.Add(response.Headers.CacheControl.MaxAge.Value);

                if (response.Headers.CacheControl.SharedMaxAge.HasValue)
                    return lastModified.Add(response.Headers.CacheControl.SharedMaxAge.Value);
            }

            if (response.Content != null && response.Content.Headers.Expires.HasValue)
                return response.Content.Headers.Expires.Value;

            var cacheableResult = result as ICacheableHttpResult;
            if (cacheableResult != null)
            {
                if (cacheableResult.Duration.HasValue && cacheableResult.Duration.Value > TimeSpan.Zero)
                    return lastModified.Add(cacheableResult.Duration.Value);

                return lastModified.Add(defaultExpiration);
            }

            return null;
        }

        private ISet<string> GetDependentUris(object result, IEnumerable<string> dependentUris)
        {
            var uris = dependentUris;

            var cacheableResult = result as ICacheableHttpResult;
            if (cacheableResult != null)
            {
                uris = uris.Concat(cacheableResult.DependentUris);
            }

            return new HashSet<string>(uris.NormalizeUris());
        }

        public void UpdateExpiration(object result, HttpResponseMessage response, TimeSpan defaultExpiration)
        {
            var lastModified = response.Headers.Date ?? DateTimeOffset.UtcNow;

            if (response.Content != null && response.Content.Headers.LastModified.HasValue)
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

            foreach (var header in responseInfo.VaryHeaders)
            {
                if (!VaryHeaders.Contains(header))
                    VaryHeaders.Add(header);
            }

            foreach (var uri in responseInfo.DependentUris)
            {
                if (!DependentUris.Contains(uri))
                    DependentUris.Add(uri);
            }
        }
    }
}