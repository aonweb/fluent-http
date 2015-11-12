using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly IDictionary<CacheKey, CacheEntry> _cache = new Dictionary<CacheKey, CacheEntry>();
        private readonly IDictionary<string, UriCacheInfo> _uriCache = new Dictionary<string, UriCacheInfo>();
        private readonly ResponseSerializer _serializer = new ResponseSerializer();
        private readonly IVaryByProvider _varyBy;

        public InMemoryCacheProvider(IVaryByProvider varyBy)
        {
            _varyBy = varyBy;
        }

        internal static string HashUri(Uri uri)
        {
            var hash = DigestHelpers.Sha256Hash(Encoding.UTF8.GetBytes(uri.ToString()));

            return Convert.ToBase64String(hash);
        }

        public async Task<CacheEntry> Get(ICacheContext context)
        {
            var key = CachingHelpers.BuildKey(_varyBy, context);

            CacheEntry cacheEntry = null;
            CacheEntry temp;

            bool hasCache;

            lock (_cache)
                hasCache = _cache.TryGetValue(key, out temp);

            if (hasCache)
            {
                if (context.ResponseValidator(context, temp.Metadata) == ResponseValidationResult.OK)
                    cacheEntry = temp;
            }

            if (cacheEntry == null)
                return CacheEntry.Empty;

            object result;

            if (cacheEntry.IsHttpResponseMessage)
            {
                var responseBuffer = cacheEntry.Value as byte[];

                result = await _serializer.Deserialize(responseBuffer, context.Token);
            }
            else
            {
                result = cacheEntry.Value;
            }

            return new CacheEntry(result, cacheEntry.Metadata);
        }

        public async Task Put(ICacheContext context, CacheEntry newCacheEntry)
        {
            _varyBy.Put(context.Uri, newCacheEntry.Metadata.VaryHeaders);

            var key = CachingHelpers.BuildKey(_varyBy, context);
            var isResponseMessage = false;
            CacheEntry cacheEntry = null;
            CacheEntry temp;

            bool hasCache;

            lock (_cache)
                hasCache = _cache.TryGetValue(key, out temp);

            if (hasCache)
            {
                if (context.ResponseValidator(context, temp.Metadata) == ResponseValidationResult.OK)
                    cacheEntry = temp;
            }

            var response = newCacheEntry.Value as HttpResponseMessage;

            if (cacheEntry == null)
            {
                object value;
                if (response != null)
                {
                    value = await _serializer.Serialize(response, context.Token);
                    isResponseMessage = true;
                }
                else
                {
                    value = newCacheEntry.Value;
                }

                cacheEntry = new CacheEntry(newCacheEntry.Metadata)
                {
                    Value = value,
                    IsHttpResponseMessage = isResponseMessage
                };
            }
            else
            {
                cacheEntry.Metadata.Merge(newCacheEntry.Metadata);
            }

            lock (_cache)
            {
                if (!_cache.ContainsKey(key))
                    _cache.Add(key, cacheEntry);
            }

            AddCacheKey(context.Uri, key);
        }

        public IEnumerable<Uri> Remove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris)
        {
            var key = CachingHelpers.BuildKey(_varyBy, context);
            CacheEntry cacheEntry;
            if (key == CacheKey.Empty)
                yield break;

            lock (_cache)
            {
                if (!_cache.ContainsKey(key))
                    yield break;

                cacheEntry = _cache[key];

                if (!_cache.Remove(key))
                    yield break;
            }

            if (context.Uri != null)
                RemoveCacheKey(context.Uri, key);

            yield return context.Uri;

            foreach (var uri in RemoveRelatedUris(cacheEntry, context, additionalRelatedUris))
            {
                yield return uri;
            }
        }

        public void Clear()
        {
            lock(_cache)
                _cache.Clear();

            lock (_uriCache)
                _uriCache.Clear();

            _varyBy.Clear();
        }

        public IEnumerable<Uri> Remove(Uri uri)
        {
            return RemoveRelatedUris(new[] { uri });
        }

        private IEnumerable<Uri> RemoveRelatedUris(CacheEntry cacheEntry, ICacheMetadata metadata, IEnumerable<Uri> additionalRelatedUris)
        {
            var uris = metadata.DependentUris ?? Enumerable.Empty<Uri>();

            if (cacheEntry?.Metadata != null)
                uris = uris.Concat(cacheEntry.Metadata.DependentUris ?? Enumerable.Empty<Uri>());

            uris = uris.Concat(additionalRelatedUris ?? Enumerable.Empty<Uri>());

            return RemoveRelatedUris(uris);
        }

        private IEnumerable<Uri> RemoveRelatedUris(IEnumerable<Uri> uris)
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

                        CacheEntry cacheEntry;

                        lock (_cache)
                        {
                            if (!_cache.ContainsKey(key))
                                continue;

                            cacheEntry = _cache[key];

                            if (!_cache.Remove(key) || cacheEntry == null)
                                continue;
                        }

                        yield return cacheEntry.Metadata.Uri;

                        foreach (var child in RemoveRelatedUris(cacheEntry.Metadata.DependentUris))
                            yield return child;
                    }
                }

            }
        }

        private UriCacheInfo GetCacheInfo(Uri uri)
        {
            var hash = HashUri(uri.Normalize());

            UriCacheInfo cacheInfo;
            bool hasCache;

            lock (_uriCache)
                hasCache = _uriCache.TryGetValue(hash, out cacheInfo);

            if (hasCache)
                return cacheInfo;

            return null;
        }

        private void AddCacheKey(Uri uri, CacheKey cacheKey)
        {
            var hash = HashUri(uri.Normalize());

            UriCacheInfo cacheInfo;

            lock (_uriCache)
            {
                if (!_uriCache.TryGetValue(hash, out cacheInfo))
                {
                    cacheInfo = new UriCacheInfo();
                    _uriCache[hash] = cacheInfo;
                }
                    
            }

            lock (cacheInfo)
            {
                if (!cacheInfo.CacheKeys.Contains(cacheKey))
                    cacheInfo.CacheKeys.Add(cacheKey);
            }
        }

        private void RemoveCacheKey(Uri uri, CacheKey cacheKey)
        {
            var hash = HashUri(uri.Normalize());

            UriCacheInfo cacheInfo;

            bool hasCache;

            lock (_uriCache)
                hasCache = _uriCache.TryGetValue(hash, out cacheInfo);

            if (!hasCache)
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