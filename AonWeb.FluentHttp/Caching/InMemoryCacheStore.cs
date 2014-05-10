using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryCacheStore : IHttpCacheStore
    {
        private const string CacheName = "HttpCallCacheInMemoryStore";

        private static MemoryCache _cache = CreateCache();
        private static readonly ConcurrentDictionary<string, UriCacheInfo> _uriCache = new ConcurrentDictionary<string, UriCacheInfo>();
        private static readonly ResponseSerializer _serializer = new ResponseSerializer();

        public async Task<CacheResult<T>> GetCachedResult<T>(CacheContext<T> context)
        {
            var cachedItem = _cache.Get(context.Key) as CachedItem;

            if (cachedItem == null)
                return CacheResult<T>.Empty;

            T result;

            if (cachedItem.IsHttpResponseMessage)
            {
                var responseBuffer = cachedItem.Result as byte[];

                result = (T)((object)await _serializer.Deserialize(responseBuffer));
            }
            else
            {
                result = (T)cachedItem.Result;
            }

            return new CacheResult<T>(cachedItem.ResponseInfo) { Result = result };
        }

        public async Task AddOrUpdate<T>(CacheContext<T> context)
        {
            var isResponseMessage = false;

            var response = context.Result.Result as HttpResponseMessage;

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

            _cache.Set(context.Key, cachedItem, cachedItem.ResponseInfo.Expiration);
            AddCacheKey(context.Uri, context.Key);
        }

        public bool TryRemove<T>(CacheContext<T> context)
        {
            var item = _cache.Remove(context.Key) as CachedItem;

            RemoveCacheKey(context.Uri, context.Key);
            TryRemoveDependentUris(item, context.Result.ResponseInfo);

            return item != null;
        }

        public void Clear()
        {
            _cache.Dispose();

            _cache = CreateCache();
        }

        public void RemoveItem(Uri uri)
        {
            TryRemoveDependentUris(new[] { uri.ToString() });
        }

        private static MemoryCache CreateCache()
        {
            return new MemoryCache(CacheName);
        }

        private void TryRemoveDependentUris(CachedItem cachedItem, ResponseInfo responseInfo)
        {
            var uris = responseInfo != null ? responseInfo.DependentUris : Enumerable.Empty<string>();

            if (cachedItem != null)
                uris = uris.Concat(cachedItem.ResponseInfo.DependentUris);

            TryRemoveDependentUris(uris);
        }

        private void TryRemoveDependentUris(IEnumerable<string> uris)
        {
            foreach (var uriString in uris.Distinct())
            {

                Uri uri;
                if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri))
                    continue;

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
                        TryRemoveDependentUris(cachedItem.ResponseInfo.DependentUris);
                    }
                }

            }
        }

        private UriCacheInfo GetCacheInfo(Uri uri)
        {

            var key = uri.GetLeftPart(UriPartial.Path);

            UriCacheInfo cacheInfo;

            if (_uriCache.TryGetValue(key, out cacheInfo))
                return cacheInfo;

            return null;
        }

        private void AddCacheKey(Uri uri, string cacheKey)
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

        private void RemoveCacheKey(Uri uri, string cacheKey)
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