using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class VaryByProvider : IVaryByProvider
    {
        private readonly ICacheProvider _cache;

        public VaryByProvider(ICacheProvider cache)
        {
            _cache = cache;
        }

        public async Task<IEnumerable<string>> Get(Uri uri)
        {
            var key = GetKey(uri);

            var headers = await _cache.Get<ISet<string>>(key);

            return headers ?? Enumerable.Empty<string>();
        }

        public async Task<bool> Put(Uri uri, IEnumerable<string> newHeaders)
        {
            var key = GetKey(uri);

            var headers = await _cache.Get<ISet<string>>(key) ?? new HashSet<string>();

            lock (headers)
            {
                headers.Merge(newHeaders.Select(UriHelpers.NormalizeHeader));
            }

            return await _cache.Put(key, headers);
        }

        public Task<bool> Delete(Uri uri)
        {
            var key = GetKey(uri);

            return _cache.Delete(key);
        }

        public Task DeleteAll()
        {
            return _cache.DeleteAll();
        }

        private static string GetKey(Uri uri)
        {
            var hash = DigestHelpers.Sha256Hash(Encoding.UTF8.GetBytes(uri.Normalize().GetSchemeHostPath()));

            return "VaryBy:" + Convert.ToBase64String(hash);
        }
    }
}