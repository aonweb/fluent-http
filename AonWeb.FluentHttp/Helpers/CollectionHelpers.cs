using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Helpers
{
    public static class CollectionHelpers
    {
        internal static MediaTypeFormatterCollection FluentAdd(this MediaTypeFormatterCollection collection, MediaTypeFormatter formatter)
        {
            collection.Add(formatter);

            return collection;
        }

        internal static HttpHeaderValueCollection<T> AddDistinct<T>(this HttpHeaderValueCollection<T> headers, Func<T, bool> predicate, string value)
            where T : class
        {
            if (!headers.Any(predicate))
                headers.ParseAdd(value);

            return headers;
        }

        internal static HttpHeaderValueCollection<T> AddDistinct<T>(this HttpHeaderValueCollection<T> headers, Func<T, string> prop, string value)
            where T : class
        {
            return headers.AddDistinct(h => string.Equals(prop(h), value, StringComparison.OrdinalIgnoreCase), value);
        }

        public static ISet<T> ToSet<T>(this IEnumerable<T> primary, params IEnumerable<T>[] additional)
        {
            if (primary is ISet<T> set && (additional == null || additional.Length == 0))
                return set;

            return primary.ToSet(EqualityComparer<T>.Default, additional);
        }

        public static ISet<T> ToSet<T>(this IEnumerable<T> primary, IEqualityComparer<T> comparer, params IEnumerable<T>[] additional)
        {
            if (primary is ISet<T> set && (additional == null || additional.Length == 0))
                return set;

            var allItems = primary.Concat((additional ?? Enumerable.Empty<IEnumerable<T>>()).SelectMany(x => x));

            var distinct = allItems.Distinct();

            return new HashSet<T>(distinct);
        }

        public static ISet<T> Merge<T>(this ISet<T> primary, IEnumerable<T> other)
        {
            if (other == null)
                return primary;

            foreach (var item in other.Where(item => !primary.Contains(item)))
            {
                primary.Add(item);
            }

            return primary;
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (collection == null)
                return list;

            foreach (var item in collection)
                list.Add(item);

            return list;
        }
    }
}