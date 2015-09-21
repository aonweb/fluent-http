using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AonWeb.FluentHttp.Helpers
{
    public static class UriHelpers
    {
        public static string CombineVirtualPaths(string basePath, string relativePath)
        {
            return string.Concat(basePath.TrimEnd('/'), "/", relativePath.TrimStart('/'));
        }

        internal static bool IsAbsolutePath(string pathAndQuery)
        {
            if (string.IsNullOrWhiteSpace(pathAndQuery))
                return true;

            return pathAndQuery.StartsWith("/");
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
        internal static IEnumerable<Uri> NormalizeUris(this IEnumerable<Uri> uris)
        {
            return uris.Select(NormalizeUri).Where(u => u != null).Distinct();
        }

        /// <summary>
        /// Tries to converts a string uri into a canonical uri string
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>A canonical uri string or empty string if url is invalid.</returns>
        internal static Uri NormalizeUri(this Uri uri)
        {
            return new UriBuilder(uri).NormalizeQuery().Uri;
        }

        /// <summary>
        /// Tries to converts a uri builder's query into a canonical format
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>Uri Builder with modified query</returns>
        internal static UriBuilder NormalizeQuery(this UriBuilder builder)
        {
            var qsCollection = new UriQueryCollection(builder.Query);
            builder.Query = qsCollection.ToEncodedString();

            return builder;
        }
    }
}