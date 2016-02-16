using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp.Serialization
{
    public static class UriQueryCollectionExtensions
    {
        public static IUriQueryCollection Set(this IUriQueryCollection query, string name, string value)

        {
            query.Set(name, new[] { value });

            return query;
        }

        public static IUriQueryCollection Set(this IUriQueryCollection query, IEnumerable<KeyValuePair<string, string>> values)

        {
            if (values == null)
                return query;

            var lookup = values.ToLookup(p => p.Key, p => p.Value);

            foreach (var group in lookup)
                query.Set(group.Key, group.ToList());

            return query;
        }

        public static IUriQueryCollection Set(this IUriQueryCollection query, IEnumerable<KeyValuePair<string, IEnumerable<string>>> values)

        {
            if (values == null)
                return query;

            var lookup = values.ToLookup(p => p.Key, p => p.Value);

            foreach (var group in lookup)
                query.Set(group.Key, group.SelectMany(g => g.ToList()));

            return query;
        }

        public static IUriQueryCollection Add(this IUriQueryCollection query, string name, IEnumerable<string> values)

        {
            if (string.IsNullOrWhiteSpace(name))
                return query;

            foreach (var value in values)
            {
                query.Add(name, value);
            }

            return query;
        }

        public static IUriQueryCollection Add(this IUriQueryCollection query, IEnumerable<KeyValuePair<string, string>> values)
 
        {
            if (values == null)
                return query;

            foreach (var pair in values)
                query.Add(pair.Key, pair.Value);

            return query;
        }

        public static IUriQueryCollection Add(this IUriQueryCollection query, IEnumerable<KeyValuePair<string, IEnumerable<string>>> values)
 
        {
            if (values == null)
                return query;

            foreach (var pair in values)
                query.Add(pair.Key, pair.Value);

            return query;
        }
    }
}