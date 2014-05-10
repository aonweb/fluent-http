using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AonWeb.FluentHttp
{
    public static class Helper
    {
        public static readonly Task TaskComplete = Task.FromResult(true);

        public static T As<T>(this object @this)
        {
            return (T)@this;
        }

        public static Action<T> MergeAction<T>(Action<T> action1, Action<T> action2)
        {
            if (action1 == null && action2 == null)
                return x => { };

            var result = action1 ?? action2;

            if (action1 != null && action2 != null)
            {
                result = x =>
                {
                    action1(x);
                    action2(x);
                };
            }

            return result;
        }

        public static string CombineVirtualPaths(string basePath, string relativePath)
        {
            return string.Concat(basePath.TrimEnd('/'), "/", relativePath.TrimStart('/'));
        }

        /// <summary>
        /// Converts the specified NameValueCollection to a QueryString formatted string i.e. "key1=val1&amp;key2=val2" suitable for use in a Url query string.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A QueryString formatted string i.e. "key1=val1&amp;key2=val2"</returns>
        public static string ToEncodedString(this NameValueCollection list)
        {

            var sb = new StringBuilder();
            foreach (var key in list.AllKeys.OrderBy(k => k))
            {
                foreach (var value in list.GetValues(key).OrderBy(v => v))
                {
                    if (sb.Length != 0)
                        sb.Append("&");

                    sb.Append(HttpUtility.UrlEncode(key));
                    sb.Append("=");
                    sb.Append(HttpUtility.UrlEncode(value));
                }

            }

            return sb.ToString();
        }

        public static void DisposeResponse(HttpResponseMessage response)
        {
            if (response == null)
                return;

            response.Dispose();
        }

        /// <summary>
        /// Converts a list string uris into a list of distinct canonical uri strings
        /// </summary>
        /// <param name="uris"></param>
        /// <returns>A distinct list of canonical uri strings.</returns>
        /// <remarks>Removes duplicates and any items that are not valid uris</remarks>
        internal static IEnumerable<string> NormalizeUris(this IEnumerable<string> uris)
        {
            return uris.Select(NormalizeUri).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct();
        }

        /// <summary>
        /// Tries to converts a string uri into a canonical uri string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>A canonical uri string or empty string if url is invalid.</returns>
        internal static string NormalizeUri(this string uri)
        {
            try
            {
                //try to normalize
                return new UriBuilder(uri).NormalizeQuery().Uri.ToString();
            }
            catch (Exception) { } // if it fails we shouldn't use it any way

            return string.Empty;
        }

        internal static UriBuilder NormalizeQuery(this UriBuilder builder)
        {
            var qsCollection = HttpUtility.ParseQueryString(builder.Query);
            builder.Query = qsCollection.ToEncodedString();

            return builder;
        }

        internal static string BuildKey(Type resultType, Uri uri, HttpHeaders headers, IEnumerable<string> varyBy)
        {

            var keyParts = new List<string>();

            if (typeof(HttpResponseMessage).IsAssignableFrom(resultType))
                keyParts.Add("HttpResponseMessage");

            keyParts.Add(uri.ToString());

            keyParts.AddRange(headers.Where(h => varyBy.Any(v => v.Equals(h.Key, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(h => h.Value.Select(v => string.Format("{0}:{1}", NormalizeHeader(h.Key), NormalizeHeader(v))))
                .Distinct());

            var key = string.Join("-", keyParts);

            var bytes = Encoding.UTF8.GetBytes(key);
 
            var hash = SHA1.Create().ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        internal static string NormalizeHeader(string input)
        {
            return (input ?? string.Empty).ToLowerInvariant().Trim();
        }

        public static ISet<T> MergeSet<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var distinct = (list1 ?? Enumerable.Empty<T>()).Concat(list2 ?? Enumerable.Empty<T>()).Distinct();

            return new HashSet<T>(distinct);
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
    }
}