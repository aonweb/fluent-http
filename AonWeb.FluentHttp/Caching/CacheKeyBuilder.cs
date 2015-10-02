using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheKeyBuilder: ICacheKeyBuilder
    {
        public CacheKey BuildKey(ICacheContext context)
        {
            return BuildKey(context.ResultType, context.Uri, context.Request.Headers);
        }

        private static CacheKey BuildKey(Type resultType, Uri uri, HttpRequestHeaders headers)
        {
            var varyBy = Cache.CurrentVaryByStore.Get(uri);
            var parts = new List<string> { uri.ToString() };

            if (typeof(HttpResponseMessage).IsAssignableFrom(resultType))
                parts.Add("HttpResponseMessage");

            parts.AddRange(headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(h => h.Value.Select(v => $"{UriHelpers.NormalizeHeader(h.Key)}:{UriHelpers.NormalizeHeader(v)}"))
                .Distinct());

            return new CacheKey(string.Join("-", parts));
        }
    }
}