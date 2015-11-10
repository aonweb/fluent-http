using System;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Helpers
{
    public static class UriHelpers
    {
        internal static bool IsRelativeUriWithAbsolutePath(this Uri uri)
        {
            if (uri?.IsAbsoluteUri ?? true)
                return false;

            if (string.IsNullOrWhiteSpace(uri.OriginalString))
                return true;

            return uri.OriginalString.StartsWith("/");
        }

        public static Uri AppendPath(this Uri baseUri, Uri relativeUri)
        {
            return new Uri(UriStringHelpers.CombineVirtualPaths(baseUri.GetSchemeHostPath(), relativeUri.OriginalString));
        }

        public static Uri AppendPath(this Uri uri, string path)
        {
            return new Uri(UriStringHelpers.CombineVirtualPaths(uri.ToString(), path));
        }

        internal static string GetSchemeHostPath(this Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (!uri.IsAbsoluteUri)
                throw new ArgumentException(nameof(uri), SR.ArgumentUriMustBeAbsoluteError);

            var scheme = uri.Scheme;
            var authority = uri.Authority;
            var path = uri.AbsolutePath;

            return $"{scheme}://{authority}{path}";
        }



        /// <summary>
        /// Converts a list string uris into a list of distinct canonical uri strings
        /// </summary>
        /// <param name="uris"></param>
        /// <returns>A distinct list of canonical uri strings.</returns>
        /// <remarks>Removes duplicates and any items that are not valid uris</remarks>
        internal static IEnumerable<Uri> Normalize(this IEnumerable<Uri> uris)
        {
            return uris.Select(Normalize).Where(u => u != null).Distinct();
        }

        /// <summary>
        /// Tries to converts a string uri into a canonical uri string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>A canonical uri string or empty string if url is invalid.</returns>
        public static Uri Normalize(this Uri uri)
        {
            return new UriBuilder(uri).NormalizeQuery().Uri;
        }

        /// <summary>
        /// Tries to converts a uri builder's query into a canonical format
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>Uri Builder with modified query</returns>
        public static UriBuilder NormalizeQuery(this UriBuilder builder)
        {
            var qsCollection = NormalizedUriQueryCollection.FromQueryString(builder.Query);
            builder.Query = qsCollection.ToEncodedString();

            return builder;
        }

        internal static string NormalizeHeader(string input)
        {
            return (input ?? string.Empty).ToLowerInvariant().Trim();
        }
    }
}