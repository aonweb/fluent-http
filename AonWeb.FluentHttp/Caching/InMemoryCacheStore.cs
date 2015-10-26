using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryCacheStore : IHttpCacheStore
    {
        private static readonly ConcurrentDictionary<CacheKey, CachedItem> _cache = new ConcurrentDictionary<CacheKey, CachedItem>();
        private static readonly ConcurrentDictionary<string, UriCacheInfo> _uriCache = new ConcurrentDictionary<string, UriCacheInfo>();
        private static readonly ResponseSerializer Serializer = new ResponseSerializer();

        internal static string HashUri(Uri uri)
        {
            var hash = DigestHelpers.Sha256Hash(Encoding.UTF8.GetBytes(uri.ToString()));

            return Convert.ToBase64String(hash);
        }

        public async Task<CacheResult> GetCachedResult(ICacheContext context)
        {
            CachedItem cachedItem = null;
            CachedItem temp;
            if (_cache.TryGetValue(context.CacheKey, out temp))
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

            CachedItem cachedItem = null;
            CachedItem temp;
            if (_cache.TryGetValue(context.CacheKey, out temp))
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
                    result = await Serializer.Serialize(context.Request, response);
                    isResponseMessage = true;
                }
                else
                {
                    result = context.Result.Result;
                }
                cachedItem = new CachedItem(context.Result.ResponseMetadata)
                {
                    Result = result,
                    IsHttpResponseMessage = isResponseMessage
                };
            }
            else
            {
                cachedItem.ResponseInfo.Merge(context.Result.ResponseMetadata, true);
            }

            _cache.TryAdd(context.CacheKey, cachedItem);

            AddCacheKey(context.Uri, context.CacheKey);
        }

        public IEnumerable<Uri> TryRemove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris)
        {
            CachedItem cachedItem;
            if (context.CacheKey == CacheKey.Empty || !_cache.TryRemove(context.CacheKey, out cachedItem))
                yield break;

            if (context.Uri != null)
                RemoveCacheKey(context.Uri, context.CacheKey);

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
            var uris = cacheContext.Result.ResponseMetadata?.DependentUris ?? Enumerable.Empty<Uri>();

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
            var hash = HashUri(uri.NormalizeUri());

            UriCacheInfo cacheInfo;

            if (_uriCache.TryGetValue(hash, out cacheInfo))
                return cacheInfo;

            return null;
        }

        private static void AddCacheKey(Uri uri, CacheKey cacheKey)
        {
            var hash = HashUri(uri.NormalizeUri());

            UriCacheInfo cacheInfo;

            if (!_uriCache.TryGetValue(hash, out cacheInfo))
                cacheInfo = new UriCacheInfo();

            lock (cacheInfo)
            {
                if (!cacheInfo.CacheKeys.Contains(cacheKey))
                    cacheInfo.CacheKeys.Add(cacheKey);
            }

            _uriCache[hash] = cacheInfo;
        }

        private static void RemoveCacheKey(Uri uri, CacheKey cacheKey)
        {
            var hash = HashUri(uri.NormalizeUri());

            UriCacheInfo cacheInfo;

            if (!_uriCache.TryGetValue(hash, out cacheInfo))
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
                CacheKeys = new HashSet<CacheKey>();
            }

            public ISet<CacheKey> CacheKeys { get; }
        }
    }
}