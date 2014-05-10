using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryVaryByStore : IVaryByStore
    {
        private static readonly object _lock = new object();
        private static readonly ConcurrentDictionary<string, ISet<string>> _cache = new ConcurrentDictionary<string, ISet<string>>();

        public IEnumerable<string> Get(Uri uri)
        {
            var key = uri.GetLeftPart(UriPartial.Path);

            ISet<string> headers;

            if (!_cache.TryGetValue(key, out headers))
                return Enumerable.Empty<string>();

            return headers;
        }

        public void AddOrUpdate(Uri uri, IEnumerable<string> newHeaders)
        {
            var key = uri.ToString();

            ISet<string> headers;

            if (!_cache.TryGetValue(key, out headers))
                headers = new HashSet<string>();

            lock (_lock)
                headers = Helper.MergeSet(headers, newHeaders.Select(Helper.NormalizeHeader));

            _cache[key] = headers;
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public bool TryRemove(Uri uri)
        {
            var key = uri.ToString();

            ISet<string> headers;

            return _cache.TryRemove(key, out headers);
        }
    }
}