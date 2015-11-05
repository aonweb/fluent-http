using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Helpers
{
    public static class CachingHelpers
    {
        public static CacheKey BuildKey(IVaryByProvider varyByProvider, ICacheContext context)
        {
            return BuildKey(varyByProvider, context.ResultType, context.Uri, context.DefaultVaryByHeaders, context.Request.Headers);
        }

        private static CacheKey BuildKey(IVaryByProvider varyByProvider, Type resultType, Uri uri, IEnumerable<string> defaultVaryByHeaders, HttpRequestHeaders headers)
        {
            
            var parts = new List<string>
            {
                typeof (HttpResponseMessage).IsAssignableFrom(resultType) ? "Http" : "Typed",
                uri.ToString()
            };

            if (typeof (HttpResponseMessage).IsAssignableFrom(resultType))
            {
                var varyBy = GetVaryByHeaders(varyByProvider, uri, defaultVaryByHeaders);
                parts.AddRange(headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase)))
                    .SelectMany(h => h.Value.Select(v => $"{UriHelpers.NormalizeHeader(h.Key)}:{UriHelpers.NormalizeHeader(v)}"))
                    .Distinct());
            }

            return new CacheKey(string.Join("-", parts));
        }

        private static IEnumerable<string> GetVaryByHeaders(IVaryByProvider varyByProvider, Uri uri, IEnumerable<string> defaultVaryByHeaders)
        {
            return defaultVaryByHeaders.ToSet(varyByProvider.Get(uri));
        }

        public static bool CanCacheRequest(ICacheContext context)
        {
            if (context.Uri == null)
                return false;

            if (!CanCacheRequest(context.Request, context.CacheableHttpMethods))
                    return false;

            if (typeof(IEmptyResult).IsAssignableFrom(context.ResultType))
                return false;

            return true;
        }

        public static bool CanCacheRequest(HttpRequestMessage request, ISet<HttpMethod> cacheableHttpMethods)
        {
            if (request == null)
                return false;

            if (!cacheableHttpMethods.Contains(request.Method))
                return false;
            
            if (request.Headers.CacheControl?.NoStore ?? false)
                return false;

            return true;
        }

        public static bool ShouldRevalidate(HttpRequestMessage request, IResponseMetadata responseMetadata, ISet<HttpMethod> cacheableHttpMethods)
        {
            if (request == null)
                return false;

            if (!cacheableHttpMethods.Contains(request.Method))
                return false;

            return responseMetadata != null && responseMetadata.StatusCode == HttpStatusCode.NotModified;
        }

        public static ResponseValidationResult ValidateResponse(IResponseMetadata responseMetadata, ISet<HttpStatusCode> cacheableHttpStatusCodes)
        {
            //This is almost verbatim from the CacheCow HttpCacheHandler's ResponseValidator func

            // 13.4
            //Unless specifically constrained by a cache-control (section 14.9) directive, a caching system MAY always store 
            // a successful response (see section 13.8) as a cache entry, MAY return it without validation if it 
            // is fresh, and MAY return it after successful validation. If there is neither a cache validator nor an 
            // explicit expiration time associated with a response, we do not expect it to be cached, but certain caches MAY violate this expectation 
            // (for example, when little or no network connectivity is available).

            // 14.9.1
            // If the no-cache directive does not specify a field-name, then a cache MUST NOT use the response to satisfy a subsequent request without 
            // successful revalidation with the origin server. This allows an origin server to prevent caching 
            // even by caches that have been configured to return stale responses to client requests.
            //If the no-cache directive does specify one or more field-names, then a cache MAY use the response 
            // to satisfy a subsequent request, subject to any other restrictions on caching. However, the specified 
            // field-name(s) MUST NOT be sent in the response to a subsequent request without successful revalidation 
            // with the origin server. This allows an origin server to prevent the re-use of certain header fields in a metadata, while still allowing caching of the rest of the response.

            if (!cacheableHttpStatusCodes.Contains(responseMetadata.StatusCode))
                return ResponseValidationResult.NotCacheable;

            if (responseMetadata.NoStore)
                return ResponseValidationResult.NotCacheable;

            if (!responseMetadata.HasContent)
                return ResponseValidationResult.NotCacheable;

            if (!responseMetadata.Expiration.HasValue)
                return ResponseValidationResult.NotCacheable;

            if (responseMetadata.NoCache)
                return ResponseValidationResult.MustRevalidate;

            if (responseMetadata.Expiration < DateTimeOffset.UtcNow)
                return responseMetadata.ShouldRevalidate ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

            return ResponseValidationResult.OK;
        }

        public static bool AllowStale(HttpRequestMessage request, IResponseMetadata responseMetadata)
        {
            //This is almost verbatim from the CacheCow HttpCallCacheHandler's  IsFreshOrStaleAcceptable 

            if (responseMetadata == null)
                throw new ArgumentException(SR.CacheContextResponseInfoRequiredError, nameof(responseMetadata));

            if (request == null)
                throw new ArgumentException(SR.CacheContextRequestMessageRequiredError, nameof(request));

            if (responseMetadata.HasContent)
                return false;

            if (responseMetadata.Expiration < DateTimeOffset.UtcNow)
                return false;

            var responseDate = responseMetadata.LastModified ?? responseMetadata.Date;
            var staleness = DateTimeOffset.UtcNow - responseDate;

            if (request.Headers.CacheControl == null)
                return staleness < TimeSpan.Zero;

            if (request.Headers.CacheControl.MinFresh.HasValue)
                return -staleness > request.Headers.CacheControl.MinFresh.Value; // staleness is negative if still fresh

            if (request.Headers.CacheControl.MaxStale) // stale acceptable
                return true;

            if (request.Headers.CacheControl.MaxStaleLimit.HasValue)
                return staleness < request.Headers.CacheControl.MaxStaleLimit.Value;

            if (request.Headers.CacheControl.MaxAge.HasValue)
                return responseDate.Add(request.Headers.CacheControl.MaxAge.Value) > DateTimeOffset.Now;

            return false;
        }

        public static IResponseMetadata CreateResponseMetadata(object result, HttpRequestMessage request, HttpResponseMessage response, ICacheMetadata context)
        {

            return CreateResponseMetadata(result, request, response, true, context.CacheDuration, context.DependentUris, context.MustRevalidate, context.DefaultVaryByHeaders, context.DefaultDurationForCacheableResults);
        }

        private static IResponseMetadata CreateResponseMetadata(
            object result,
            HttpRequestMessage request,
            HttpResponseMessage response,
            bool useGreater = true,
            TimeSpan? cacheDuration = null,
            IEnumerable<Uri> additionalDependentUris = null,
            bool mustRevalidate = false,
            IEnumerable<string> additionalVaryByHeaders = null,
            TimeSpan? defaultCacheDuration = null)
        {
            var metadata = (result as IResultWithMetadata)?.Metadata ?? new ResponseMetadata();


            metadata.Uri = request.RequestUri;
            metadata.StatusCode = response.StatusCode;

            metadata.Date = metadata.Date.GetValue(response.Headers.Date ?? DateTimeOffset.UtcNow, useGreater);
            metadata.LastModified = metadata.LastModified.GetValue(response.Content?.Headers?.LastModified, useGreater);
            metadata.HasContent = metadata.HasContent || response.Content != null;

            if (response.Headers.ETag != null)
            {
                if (metadata.ETag != null && !response.Headers.ETag.Tag.Equals(metadata.ETag))
                    metadata.ETag = (metadata.ETag?? string.Empty) + response.Headers.ETag.Tag;
                else
                    metadata.ETag = response.Headers.ETag?.Tag;
            }

            metadata.NoStore = metadata.NoStore || (response.Headers.CacheControl?.NoStore ?? false);
            metadata.NoCache = metadata.NoCache || (response.Headers.CacheControl?.NoCache ?? false);
            metadata.ShouldRevalidate = metadata.ShouldRevalidate || mustRevalidate || (response.Headers.CacheControl?.MustRevalidate ?? metadata.NoCache);

            var requestedCacheDuration = cacheDuration ?? (result as ICacheableHttpResult)?.Duration ?? defaultCacheDuration;
            var expiration = GetExpiration(metadata.LastModified ?? metadata.Date, response, requestedCacheDuration);

            metadata.Expiration = metadata.Expiration.GetValue(expiration, useGreater);
            
            var newVary = response.Headers.Vary.Concat(additionalVaryByHeaders ?? Enumerable.Empty<string>()).Distinct(StringComparer.OrdinalIgnoreCase);

            metadata.VaryHeaders.Clear();

            foreach (var v in newVary.Where(v => !metadata.VaryHeaders.Contains(v)))
            {
                metadata.VaryHeaders.Add(v);
            }

            var originalDependentUris = metadata.DependentUris ?? new HashSet<Uri>();
            if (additionalDependentUris != null)
            {
                foreach (var u in GetDependentUris(metadata, result, additionalDependentUris).Where(u => !originalDependentUris.Contains(u)))
                {
                    originalDependentUris.Add(u);
                }
            }

            return metadata;
        }

        public static void Merge(this IResponseMetadata metadataA, IResponseMetadata metadataB, bool useGreater = true)
        {
            metadataA.Date = metadataA.Date.GetValue(metadataB.Date, useGreater);
            metadataA.LastModified = metadataA.LastModified.GetValue(metadataB.LastModified, useGreater);
            metadataA.Expiration = metadataA.Expiration.GetValue(metadataB.Expiration, useGreater);

            if (metadataB.ShouldRevalidate)
                metadataA.ShouldRevalidate = metadataB.ShouldRevalidate;

            if (metadataB.ETag != null)
                metadataA.ETag = (metadataA.ETag ?? string.Empty) + metadataB.ETag;

            foreach (var header in metadataB.VaryHeaders.Where(header => !metadataA.VaryHeaders.Contains(header)))
            {
                metadataA.VaryHeaders.Add(header);
            }

            foreach (var uri in metadataB.DependentUris.Where(uri => !metadataA.DependentUris.Contains(uri)))
            {
                metadataA.DependentUris.Add(uri);
            }
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

        private static IEnumerable<Uri> GetDependentUris(IResponseMetadata metadata, object result, IEnumerable<Uri> dependentUris)
        {
            var uris = (dependentUris ?? Enumerable.Empty<Uri>()).Concat(metadata.DependentUris);

            var cacheableResult = result as ICacheableHttpResult;
            if (cacheableResult?.DependentUris != null)
            {
                uris = uris.Concat(cacheableResult.DependentUris);
            }

            return uris.Normalize();
        }

        private static T GetValue<T>(this T a, T b, bool useGreaterThan)
            where T : IComparable<T>
        {
            var comparison = a.CompareTo(b);

            if (useGreaterThan)
                return comparison > 0 ? a : b;

            return comparison < 0 ? a : b;
        }

        private static T? GetValue<T>(this T? a, T? b, bool useGreaterThan)
            where T : struct, IComparable<T>
        {
            if (!a.HasValue && !b.HasValue)
                return null;

            if (!a.HasValue)
                return b.Value;

            if (!b.HasValue)
                return a.Value;

            return GetValue(a.Value, b.Value, useGreaterThan);
        }
    }
}