using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryCacheStore : IHttpCacheStore
    {
        private static readonly ConcurrentDictionary<string, CachedItem> _cache = new ConcurrentDictionary<string, CachedItem>();
        private static readonly ConcurrentDictionary<Uri, UriCacheInfo> _uriCache = new ConcurrentDictionary<Uri, UriCacheInfo>();
        private static readonly ResponseSerializer Serializer = new ResponseSerializer();

        internal static string BuildKey(Type resultType, Uri uri, HttpRequestHeaders headers)
        {
            var varyBy = Cache.CurrentVaryByStore.Get(uri);
            var parts = new List<string> { uri.ToString() };

            if (typeof(HttpResponseMessage).IsAssignableFrom(resultType))
                parts.Add("HttpResponseMessage");

            parts.AddRange(headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(h => h.Value.Select(v => $"{UriHelpers.NormalizeHeader(h.Key)}:{UriHelpers.NormalizeHeader(v)}"))
                .Distinct());

            var body = string.Join("-", parts);
            var hash = DigestHelpers.Sha256Hash(Encoding.UTF8.GetBytes(body));

            return Convert.ToBase64String(hash);
        }

        public async Task<CacheResult> GetCachedResult(ICacheContext context)
        {
            var key = BuildKey(context.ResultType, context.Request.RequestUri, context.Request.Headers);

            CachedItem cachedItem = null;
            CachedItem temp;
            if (_cache.TryGetValue(key, out temp))
            {
                if (context.ResponseValidator(context, temp.ResponseInfo) == ResponseValidationResult.OK)
                    cachedItem = temp;
            }

            if (cachedItem == null)
                return CacheResult.Empty;

            object result;

            if (cachedItem.IsHttpResponseMessage)
            {
                var responseBuffer = cachedItem.Result as byte[];

                result = await Serializer.Deserialize(responseBuffer);
            }
            else
            {
                result = cachedItem.Result;
            }

            return new CacheResult(result, cachedItem.ResponseInfo);
        }

        public async Task AddOrUpdate(ICacheContext context)
        {
            var isResponseMessage = false;

            var key = BuildKey(context.ResultType, context.Request.RequestUri, context.Request.Headers);

            CachedItem cachedItem = null;
            CachedItem temp;
            if (_cache.TryGetValue(key, out temp))
            {
                if (context.ResponseValidator(context, temp.ResponseInfo) == ResponseValidationResult.OK)
                    cachedItem = temp;
            }

            var response = context.Result.Result as HttpResponseMessage;

            if (cachedItem == null)
            {
                object result;
                if (response != null)
                {
                    result = await Serializer.Serialize(response);
                    isResponseMessage = true;
                }
                else
                {
                    result = context.Result.Result;
                }
                cachedItem = new CachedItem(context.Result.ResponseInfo)
                {
                    Result = result,
                    IsHttpResponseMessage = isResponseMessage
                };
            }
            else
            {
                cachedItem.ResponseInfo.Merge(context.Result.ResponseInfo);
            }

            _cache.TryAdd(key, cachedItem);

            AddCacheKey(context.Uri, key);
        }

        public IEnumerable<Uri> TryRemove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris)
        {
            var key = BuildKey(context.ResultType, context.Request.RequestUri, context.Request.Headers);

            CachedItem cachedItem;
            if (string.IsNullOrWhiteSpace(key) || !_cache.TryRemove(key, out cachedItem))
                yield break;

            if (context.Uri != null)
                RemoveCacheKey(context.Uri, key);

            yield return context.Uri;

            foreach (var uri in RemoveRelatedUris(cachedItem, context, additionalRelatedUris))
            {
                yield return uri;
            }
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public IEnumerable<Uri> RemoveItem(Uri uri)
        {
            return RemoveRelatedUris(new[] { uri });
        }

        private IEnumerable<Uri> RemoveRelatedUris(CachedItem cachedItem, ICacheContext cacheContext, IEnumerable<Uri> additionalRelatedUris)
        {
            var uris = cacheContext.Result.ResponseInfo?.DependentUris ?? Enumerable.Empty<Uri>();

            if (cachedItem?.ResponseInfo != null)
                uris = uris.Concat(cachedItem.ResponseInfo.DependentUris ?? Enumerable.Empty<Uri>());

            uris = uris.Concat(additionalRelatedUris ?? Enumerable.Empty<Uri>());

            return RemoveRelatedUris(uris);
        }

        private static IEnumerable<Uri> RemoveRelatedUris(IEnumerable<Uri> uris)
        {
            foreach (var uri in uris.Where(u => u != null).Distinct())
            {
                var cacheInfo = GetCacheInfo(uri);

                if (cacheInfo == null)
                    continue;

                lock (cacheInfo)
                {
                    var keys = cacheInfo.CacheKeys.ToArray();

                    foreach (var key in keys)
                    {
                        if (cacheInfo.CacheKeys.Contains(key))
                            cacheInfo.CacheKeys.Remove(key);

                        CachedItem cachedItem;

                        if (!_cache.TryRemove(key, out cachedItem) || cachedItem == null)
                            continue;

                        yield return cachedItem.ResponseInfo.Uri;

                        foreach (var child in RemoveRelatedUris(cachedItem.ResponseInfo.DependentUris))
                            yield return child;
                    }
                }

            }
        }

        private static UriCacheInfo GetCacheInfo(Uri uri)
        {
            var normalizedUri = uri.NormalizeUri();

            UriCacheInfo cacheInfo;

            if (_uriCache.TryGetValue(normalizedUri, out cacheInfo))
                return cacheInfo;

            return null;
        }

        private static void AddCacheKey(Uri uri, string cacheKey)
        {
            var normalizedUri = uri.NormalizeUri();

            UriCacheInfo cacheInfo;

            if (!_uriCache.TryGetValue(normalizedUri, out cacheInfo))
                cacheInfo = new UriCacheInfo();

            lock (cacheInfo)
            {
                if (!cacheInfo.CacheKeys.Contains(cacheKey))
                    cacheInfo.CacheKeys.Add(cacheKey);
            }

            _uriCache[normalizedUri] = cacheInfo;
        }

        private static void RemoveCacheKey(Uri uri, string cacheKey)
        {
            var normalizedUri = uri.NormalizeUri();

            UriCacheInfo cacheInfo;

            if (!_uriCache.TryGetValue(normalizedUri, out cacheInfo))
                return;

            lock (cacheInfo)
            {
                if (cacheInfo.CacheKeys.Contains(cacheKey))
                    cacheInfo.CacheKeys.Remove(cacheKey);
            }
        }

        private class UriCacheInfo
        {
            public UriCacheInfo()
            {
                CacheKeys = new HashSet<string>();
            }

            public ISet<string> CacheKeys { get; }
        }
    }
}