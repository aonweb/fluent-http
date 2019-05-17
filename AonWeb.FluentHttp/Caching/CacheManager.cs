using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            if (cacheEntry == null)
                return CacheEntry.Empty;

            var validation = context.ResponseValidator(context, cacheEntry.Metadata);

            if (validation == ResponseValidationResult.Stale && !context.AllowStaleResultValidator(context, cacheEntry.Metadata))
                return CacheEntry.Empty;

            if (validation != ResponseValidationResult.OK
                && validation != ResponseValidationResult.Stale
                && validation != ResponseValidationResult.MustRevalidate)
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

            return new CacheEntry(result, cacheEntry.Metadata) { IsHttpResponseMessage = cacheEntry.IsHttpResponseMessage };
        }

        public async Task Put(ICacheContext context, CacheEntry newCacheEntry)
        {
            var key = await GetKey(context);
            var isHttpResponseMessage = newCacheEntry.IsHttpResponseMessage;
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
                    isHttpResponseMessage = true;
                }
                else
                {
                    value = newCacheEntry.Value;
                }

                cacheEntry = new CacheEntry(newCacheEntry.Metadata)
                {
                    Value = value,
                    IsHttpResponseMessage = isHttpResponseMessage
                };
            }
            else
            {
                cacheEntry.Metadata.Merge(newCacheEntry.Metadata);
            }

            // TODO: this may not be the place for this, I'd like to provide some ability to control jitter
            var duration = CachingHelpers.GetCacheDuration(cacheEntry.Metadata);

            await Task.WhenAll(
                _cache.Put(key, cacheEntry, duration),
                 _varyBy.Put(context.Uri, newCacheEntry.Metadata.VaryHeaders, duration),
                 _uriInfo.Put(context.Uri, key, duration)
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

            var builder = new StringBuilder();
            var isHttpResponseMessage = typeof(HttpResponseMessage).IsAssignableFrom(resultType);


            builder.Append(isHttpResponseMessage ? "Http" : "Typed")
                .Append("-")
                .Append(uri?.ToString() ?? "uri://unknown");

            if (!isHttpResponseMessage)
                return builder.ToString();

            var varyBy = await GetVaryByHeaders(uri, defaultVaryByHeaders);

            foreach (var header in headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase))).OrderBy(h => h.Key))
            {
                builder.Append(";");
                builder.Append(UriHelpers.NormalizeHeader(header.Key)).Append(":");
                builder.Append(string.Join(",", header.Value.OrderBy(v => v).Select(UriHelpers.NormalizeHeader).Distinct()));
            }

            return builder.ToString();
        }

        private  async Task<IEnumerable<string>> GetVaryByHeaders(Uri uri, IEnumerable<string> defaultVaryByHeaders)
        {
            var headers = await _varyBy.Get(uri);

            return defaultVaryByHeaders.ToSet(headers);
        }

    }
}