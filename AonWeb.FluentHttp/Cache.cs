using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp
{
    public static class Cache
    {
        private static readonly Lazy<IHttpCacheStore> CacheStore = new Lazy<IHttpCacheStore>(() => Defaults.Caching.CacheStoreFactory(), true);
        private static readonly Lazy<IVaryByStore> VaryStore = new Lazy<IVaryByStore>(() => Defaults.Caching.VaryByStoreFactory(), true);
        private static readonly Lazy<byte[]> CacheKey = new Lazy<byte[]>();  

        public static void Clear()
        {
            CurrentCacheStore.Clear();
        }

        public static void Remove(string uri)
        {
           Remove(new Uri(uri));
        }

        public static void Remove(Uri uri)
        {
            CurrentCacheStore.RemoveItem(uri);
        }

        internal static IHttpCacheStore CurrentCacheStore => CacheStore.Value;

        internal static IVaryByStore CurrentVaryByStore => VaryStore.Value;

        internal static string BuildKey(Type resultType, Uri uri, HttpRequestHeaders headers)
        {
            var varyBy = CurrentVaryByStore.Get(uri);
            var parts = new List<string> { uri.ToString() };

            if (typeof(HttpResponseMessage).IsAssignableFrom(resultType))
                parts.Add("HttpResponseMessage");

            parts.AddRange(headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(h => h.Value.Select(v => $"{NormalizeHeader(h.Key)}:{NormalizeHeader(v)}"))
                .Distinct());


            var body = string.Join("-", parts);
            var bodyBytes = Encoding.UTF8.GetBytes(body);

            var hashAlgorithm = new HMACSHA1(CacheKey.Value);
            var hashBytes = hashAlgorithm.ComputeHash(bodyBytes);
            return Convert.ToBase64String(hashBytes);
        }

        internal static string NormalizeHeader(string input)
        {
            return (input ?? string.Empty).ToLowerInvariant().Trim();
        }
    }
}