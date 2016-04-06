using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly ICacheProvider _cache;
        private readonly IUriInfoProvider _uriInfo;
        private readonly IVaryByProvider _varyBy;
        private readonly ResponseSerializer _serializer;

        public CacheManager(ICacheProvider cache, IVaryByProvider varyBy,  IUriInfoProvider uriInfo, ResponseSerializer serializer)
        {
            _varyBy = varyBy;
            _cache = cache;
            _uriInfo = uriInfo;
            _serializer = serializer;
        }

        public async Task<CacheEntry> Get(ICacheContext context)
        {
            var key = await GetKey(context);

            var cacheEntry = await _cache.Get<CacheEntry>(key);

            if (cacheEntry == null || context.ResponseValidator(context, cacheEntry.Metadata) != ResponseValidationResult.OK)
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
            var key = await GetKey(context);
            var isResponseMessage = false;
            CacheEntry cacheEntry = null;
            var temp = await _cache.Get<CacheEntry>(key);

            if (temp != null && context.ResponseValidator(context, temp.Metadata) == ResponseValidationResult.OK)
                cacheEntry = temp;

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

            await Task.WhenAll(
                _cache.Put(key, cacheEntry),
                 _varyBy.Put(context.Uri, newCacheEntry.Metadata.VaryHeaders),
                 _uriInfo.Put(context.Uri, key)
            );
        }

        public async Task<IList<Uri>> Delete(ICacheContext context, IEnumerable<Uri> additionalDependentUris)
        {
            var key = await GetKey(context);
            var uris = new List<Uri>();

            if (key == CacheKey.Empty)
                return uris;

            var cacheEntry = await _cache.Get<CacheEntry>(key);

            if (cacheEntry == null)
                return uris;

            var deleteTask = _cache.Delete(key);
            var deleteUriTask = _uriInfo.DeleteKey(context.Uri, key);

            await Task.WhenAll(deleteTask, deleteUriTask);

            if (!deleteTask.Result)
                return uris;

            if (context.Uri != null)
                uris.Add(context.Uri);

            var relatedUris = await DeleteDependentUris(cacheEntry, context, additionalDependentUris);

            uris.AddRange(relatedUris);

            return uris;
        }


        public Task<IList<Uri>> Delete(Uri uri)
        {
            return DeleteDependentUris(new[] { uri });
        }

        public Task DeleteAll()
        {
            return Task.WhenAll(
                _cache.DeleteAll(),
                _varyBy.DeleteAll(),
                _uriInfo.DeleteAll()
            );
        }

        private Task<IList<Uri>> DeleteDependentUris(CacheEntry cacheEntry, ICacheMetadata metadata, IEnumerable<Uri> additionalDependentUris)
        {
            var uris = metadata.DependentUris ?? Enumerable.Empty<Uri>();

            if (cacheEntry?.Metadata != null)
                uris = uris.Concat(cacheEntry.Metadata.DependentUris ?? Enumerable.Empty<Uri>());

            uris = uris.Concat(additionalDependentUris ?? Enumerable.Empty<Uri>());

            return DeleteDependentUris(uris);
        }

        private async Task<IList<Uri>> DeleteDependentUris(IEnumerable<Uri> uris)
        {
            //this could probably be made more efficient by batching and providing a some type of batching interface for the cache provider
            var removed = new List<Uri>();

            foreach (var uri in uris.Where(u => u != null).Distinct())
            {
                var uriInfo = await _uriInfo.Get(uri);

                if (uriInfo == null)
                    continue;

                await _uriInfo.Delete(uri);

                var keys = uriInfo.CacheKeys.ToList();

                foreach (var key in keys)
                {
                    var cacheEntry = await _cache.Get<CacheEntry>(key);

                    if (cacheEntry == null)
                        continue;

                    removed.Add(cacheEntry.Metadata.Uri);

                    var deleteTask = _cache.Delete(key);

                    var relatedRemovedTask = DeleteDependentUris(cacheEntry.Metadata.DependentUris);

                    await Task.WhenAll(deleteTask, relatedRemovedTask);

                    removed.AddRange(relatedRemovedTask.Result);
                }
            }

            return removed;
        }
        private Task<CacheKey> GetKey(ICacheContext context)
        {
            return GetKey(context.ResultType, context.Uri, context.DefaultVaryByHeaders, context.Request?.Headers);
        }

        private async Task<CacheKey> GetKey(Type resultType, Uri uri, IEnumerable<string> defaultVaryByHeaders, HttpRequestHeaders headers)
        {

            var parts = new List<string>
            {
                typeof (HttpResponseMessage).IsAssignableFrom(resultType) ? "Http" : "Typed",
                uri?.ToString() ?? string.Empty
            };

            if (typeof(HttpResponseMessage).IsAssignableFrom(resultType))
            {
                var varyBy = await GetVaryByHeaders(uri, defaultVaryByHeaders);
                parts.AddRange(headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase)))
                    .SelectMany(h => h.Value.Select(v => $"{UriHelpers.NormalizeHeader(h.Key)}:{UriHelpers.NormalizeHeader(v)}"))
                    .Distinct());
            }

            return new CacheKey(string.Join("-", parts));
        }

        private  async Task<IEnumerable<string>> GetVaryByHeaders(Uri uri, IEnumerable<string> defaultVaryByHeaders)
        {
            var headers = await _varyBy.Get(uri);

            return defaultVaryByHeaders.ToSet(headers);
        }

    }
}