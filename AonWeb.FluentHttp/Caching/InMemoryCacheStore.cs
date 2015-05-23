using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.OptionsModel;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryCacheStore : IHttpCacheStore
    {
        private const string CacheName = "HttpCallCacheInMemoryStore";

        private static MemoryCache _cache = CreateCache();
        private static readonly ConcurrentDictionary<string, UriCacheInfo> _uriCache = new ConcurrentDictionary<string, UriCacheInfo>();
        private static readonly ResponseSerializer _serializer = new ResponseSerializer();

        public async Task<CacheResult> GetCachedResult(CacheContext context)
        {
            var cachedItem = _cache.Get(context.Key) as CachedItem;

            if (cachedItem == null)
                return CacheResult.Empty;

            object result;

            if (cachedItem.IsHttpResponseMessage)
            {
                var responseBuffer = cachedItem.Result as byte[];

                result = await _serializer.Deserialize(responseBuffer);
            }
            else
            {
                result = cachedItem.Result;
            }

            return new CacheResult(result, cachedItem.ResponseInfo);
        }

        public async Task AddOrUpdate(CacheContext context)
        {
            var isResponseMessage = false;

            var response = context.CacheResult.Result as HttpResponseMessage;

            var cachedItem = _cache.Get(context.Key) as CachedItem;

            if (cachedItem == null)
            {
                object result;
                if (response != null)
                {
                    result = await _serializer.Serialize(response);
                    isResponseMessage = true;
                }
                else
                {
                    result = context.CacheResult.Result;
                }
                cachedItem = new CachedItem(context.CacheResult.ResponseInfo)
                {
                    Result = result,
                    IsHttpResponseMessage = isResponseMessage
                };
            }
            else
            {
                cachedItem.ResponseInfo.Merge(context.CacheResult.ResponseInfo);
            }

            _cache.Set(context.Key, cachedItem, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = cachedItem.ResponseInfo.Expiration
            });

            AddCacheKey(context.Uri, context.Key);
        }

        public IList<string> TryRemove(CacheContext context, IEnumerable<Uri> additionalRelatedUris)
        {
            if (string.IsNullOrWhiteSpace(context.Key))
                return new string[0];

            var item = _cache.Get(context.Key) as CachedItem;

            _cache.Remove(context.Key);

            if (context.Uri != null)
                RemoveCacheKey(context.Uri, context.Key);

            return new []{ context.Key}.Concat(RemoveRelatedUris(item, context, additionalRelatedUris)).ToList();
        }

        public void Clear()
        {
            _cache.Dispose();

            _cache = CreateCache();
        }

        public IEnumerable<string> RemoveItem(Uri uri)
        {
            return RemoveRelatedUris(new[] { uri });
        }

        private static MemoryCache CreateCache()
        {
            return new MemoryCache(new MemoryCacheOptions());
        }

        private IEnumerable<string> RemoveRelatedUris(CachedItem cachedItem, CacheContext cacheContext, IEnumerable<Uri> additionalRelatedUris)
        {
            IEnumerable<Uri> uris;

            if (cacheContext != null
                && cacheContext.CacheResult != null
                && cacheContext.CacheResult.ResponseInfo != null)
            {
                uris = cacheContext.CacheResult.ResponseInfo.DependentUris ?? Enumerable.Empty<Uri>();
            }
            else
            {
                uris = Enumerable.Empty<Uri>();
            } 

            if (cachedItem != null && cachedItem.ResponseInfo != null)
                uris = uris.Concat(cachedItem.ResponseInfo.DependentUris ?? Enumerable.Empty<Uri>());

            uris = uris.Concat(additionalRelatedUris ?? Enumerable.Empty<Uri>());

            return RemoveRelatedUris(uris);
        }

        private static IEnumerable<string> RemoveRelatedUris(IEnumerable<Uri> uris)
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

                        var cachedItem = _cache.Get(key) as CachedItem;

                        if (cachedItem == null)
                            continue;

                        _cache.Remove(key);

                        yield return key;

                        foreach (var child in RemoveRelatedUris(cachedItem.ResponseInfo.DependentUris))
                            yield return child;
                    }
                }

            }
        }

        private static UriCacheInfo GetCacheInfo(Uri uri)
        {
            var key = uri.GetLeftPart(UriPartial.Path);

            UriCacheInfo cacheInfo;

            if (_uriCache.TryGetValue(key, out cacheInfo))
                return cacheInfo;

            return null;
        }

        private static void AddCacheKey(Uri uri, string cacheKey)
        {
            var key = uri.GetLeftPart(UriPartial.Path);

            UriCacheInfo cacheInfo;

            if (!_uriCache.TryGetValue(key, out cacheInfo))
                cacheInfo = new UriCacheInfo();

            lock (cacheInfo)
            {
                if (!cacheInfo.CacheKeys.Contains(cacheKey))
                    cacheInfo.CacheKeys.Add(cacheKey);
            }

            _uriCache[key] = cacheInfo;
        }

        private static void RemoveCacheKey(Uri uri, string cacheKey)
        {
            var key = uri.GetLeftPart(UriPartial.Path);

            UriCacheInfo cacheInfo;

            if (!_uriCache.TryGetValue(key, out cacheInfo))
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

            public ISet<string> CacheKeys { get; private set; }
        }
    }
}