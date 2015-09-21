using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Helpers
{
    internal static class CollectionHelpers
    {
        internal static MediaTypeFormatterCollection FluentAdd(this MediaTypeFormatterCollection collection, MediaTypeFormatter formatter)
        {
            collection.Add(formatter);

            return collection;
        }

        public static HttpHeaderValueCollection<T> AddDistinct<T>(this HttpHeaderValueCollection<T> headers, Func<T, bool> predicate, string value)
            where T : class
        {
            if (!headers.Any(predicate))
                headers.ParseAdd(value);

            return headers;
        }

        public static HttpHeaderValueCollection<T> AddDistinct<T>(this HttpHeaderValueCollection<T> headers, Func<T, string> prop, string value)
            where T : class
        {
            return headers.AddDistinct(h => string.Equals(prop(h), value, StringComparison.OrdinalIgnoreCase), value);
        }

        public static ISet<T> MergeSet<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var distinct = (list1 ?? Enumerable.Empty<T>()).Concat(list2 ?? Enumerable.Empty<T>()).Distinct();

            return new HashSet<T>(distinct);
        }
    }
}