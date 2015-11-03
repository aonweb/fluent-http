using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryVaryByProvider : IVaryByProvider
    {
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<string, ISet<string>> _cache = new ConcurrentDictionary<string, ISet<string>>();

        public IEnumerable<string> Get(Uri uri)
        {
            var key = uri.GetSchemeHostPath();

            ISet<string> headers;

            if (!_cache.TryGetValue(key, out headers))
                return Enumerable.Empty<string>();

            return headers;
        }

        public bool Put(Uri uri, IEnumerable<string> newHeaders)
        {
            var key = uri.ToString();

            ISet<string> headers;

            if (!_cache.TryGetValue(key, out headers))
                headers = new HashSet<string>();

            lock (_lock)
                headers = headers.ToSet(newHeaders.Select(UriHelpers.NormalizeHeader));

            _cache[key] = headers;

            return true;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public bool Remove(Uri uri)
        {
            var key = uri.ToString();

            ISet<string> headers;

            return _cache.TryRemove(key, out headers);
        }
    }
}