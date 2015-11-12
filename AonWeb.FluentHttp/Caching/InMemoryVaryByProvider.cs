using System;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryVaryByProvider : IVaryByProvider
    {
        private readonly IDictionary<string, ISet<string>> _cache = new Dictionary<string, ISet<string>>();

        public IEnumerable<string> Get(Uri uri)
        {
            var key = uri.GetSchemeHostPath();

            ISet<string> headers;

            lock (_cache)
            {
                if (!_cache.TryGetValue(key, out headers))
                    return Enumerable.Empty<string>();
            }

            return headers;
        }

        public bool Put(Uri uri, IEnumerable<string> newHeaders)
        {
            var key = uri.ToString();

            lock (_cache)
            {
                ISet<string> headers;
                if (!_cache.TryGetValue(key, out headers))
                    headers = new HashSet<string>();

                _cache[key] = headers.ToSet(newHeaders.Select(UriHelpers.NormalizeHeader));
            }

            return true;
        }

        public void Clear()
        {
            lock (_cache)
                _cache.Clear();
        }

        public bool Remove(Uri uri)
        {
            var key = uri.ToString();

            lock (_cache)
            {
                if (!_cache.ContainsKey(key))
                    return false;

                return _cache.Remove(key);
            }
        }
    }
}