using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class InMemoryVaryByStore : IVaryByStore
    {
        private static readonly object Lock = new object();
        private static readonly ConcurrentDictionary<string, ISet<string>> Cache = new ConcurrentDictionary<string, ISet<string>>();

        public IEnumerable<string> Get(Uri uri)
        {
            var key = uri.GetSchemeHostPath();

            ISet<string> headers;

            if (!Cache.TryGetValue(key, out headers))
                return Enumerable.Empty<string>();

            return headers;
        }

        public void AddOrUpdate(Uri uri, IEnumerable<string> newHeaders)
        {
            var key = uri.ToString();

            ISet<string> headers;

            if (!Cache.TryGetValue(key, out headers))
                headers = new HashSet<string>();

            lock (Lock)
                headers = CollectionHelpers.MergeSet(headers, newHeaders.Select(FluentHttp.Cache.NormalizeHeader));

            Cache[key] = headers;
        }

        public void Clear()
        {
            Cache.Clear();
        }

        public bool TryRemove(Uri uri)
        {
            var key = uri.ToString();

            ISet<string> headers;

            return Cache.TryRemove(key, out headers);
        }
    }
}