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
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<CacheKey, CacheEntry> _cache = new ConcurrentDictionary<CacheKey, CacheEntry>();
        private readonly ConcurrentDictionary<string, UriCacheInfo> _uriCache = new ConcurrentDictionary<string, UriCacheInfo>();
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

            if (_cache.TryGetValue(key, out temp))
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

                result = await _serializer.Deserialize(responseBuffer);
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

            if (_cache.TryGetValue(key, out temp))
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
                    value = await _serializer.Serialize(response);
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

            _cache.TryAdd(key, cacheEntry);

            AddCacheKey(context.Uri, key);
        }

        public IEnumerable<Uri> Remove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris)
        {
            var key = CachingHelpers.BuildKey(_varyBy, context);
            CacheEntry cacheEntry;
            if (key == CacheKey.Empty || !_cache.TryRemove(key, out cacheEntry))
                yield break;

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
            _cache.Clear();
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

                        if (!_cache.TryRemove(key, out cacheEntry) || cacheEntry == null)
                            continue;

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

            if (_uriCache.TryGetValue(hash, out cacheInfo))
                return cacheInfo;

            return null;
        }

        private void AddCacheKey(Uri uri, CacheKey cacheKey)
        {
            var hash = HashUri(uri.Normalize());

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

        private void RemoveCacheKey(Uri uri, CacheKey cacheKey)
        {
            var hash = HashUri(uri.Normalize());

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