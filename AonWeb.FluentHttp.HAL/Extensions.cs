using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public static class Extensions
    {
        #region HasLink

        public static bool HasLink(this IHalResource resource, string key)
        {
            if (resource == null || string.IsNullOrWhiteSpace(key))
                return false;

            return resource.Links.HasLink(key);
        }

        public static bool HasLink(this IEnumerable<HyperMediaLink> linkEntity, string key)
        {
            if (linkEntity == null || string.IsNullOrWhiteSpace(key))
                return false;

            return linkEntity.Any(l => string.Equals(l.Rel, key, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region GetLink

        public static Uri GetLink(this IHalResource resource, string key)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.GetLink(key);
        }

        public static Uri GetLink(this IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return resource.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri GetLink(this IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.GetLink(key, tokens);
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key)
        {
            var uri = linkEntity.GetLinkImplementation(key).Href;

            return new Uri(uri);
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            return linkEntity.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            var url = linkEntity.GetTemplatedLinkString(key, tokens);

            return new Uri(url);
        }

        #endregion

        #region TryGetLink

        public static Uri TryGetLink(this IHalResource resource, string key)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.TryGetLink(key);
        }

        public static Uri TryGetLink(this IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return resource.TryGetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri TryGetLink(this IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.TryGetLink(key, tokens);
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key)
        {
            if (linkEntity == null)
                throw new ArgumentNullException("linkEntity");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            var link = linkEntity.GetLinkImplementation(key, false);

            if (link == null)
                return null;

            return new Uri(link.Href);
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            return linkEntity.TryGetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            var uri = linkEntity.GetTemplatedLinkString(key, tokens, false);

            if (uri == null) 
                return null;

            return new Uri(uri);
        }

        #endregion

        #region GetSelf

        public static Uri GetSelf(this IHalResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.GetSelf();
        }

        public static Uri GetSelf(this IEnumerable<HyperMediaLink> linkEntity)
        {
            return linkEntity.GetLink(HalResource.LinkKeySelf);
        }

        #endregion

        #region Internals

        private static HyperMediaLink GetLinkImplementation(this IEnumerable<HyperMediaLink> linkEntity, string key, bool throwOnMissingLink = true)
        {
            if (linkEntity == null)
                throw new ArgumentNullException("linkEntity");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            if (!linkEntity.Any())
                throw new ArgumentException("HyperMediaLinks entity does not contain any links");

            var link = linkEntity.FirstOrDefault(l => string.Equals(l.Rel, key, StringComparison.OrdinalIgnoreCase));

            if (link == null)
            {
                if (throwOnMissingLink)
                    throw new KeyNotFoundException(string.Format("Could not locate key '{0}' in HyperMediaLinks entity Links collection", key));

                return null;
            }

            return link;
        }

        private static string GetTemplatedLinkString(this IEnumerable<HyperMediaLink> linkEntity, string key, IEnumerable<KeyValuePair<string, object>> tokens, bool throwOnMissingLink = true)
        {
            var link = linkEntity.GetLinkImplementation(key, throwOnMissingLink);

            if (link == null)
                return null;

            var formatUrl = link.Href;

            if (tokens == null)
                throw new ArgumentNullException("tokens");

            //TODO: isTemplated and token validation?

            var outputUrl = formatUrl;

            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Key))
                    throw new ArgumentException(string.Format("A supplied url token for url '{0}' was null or empty.", formatUrl));

                var val = HttpUtility.UrlEncode((token.Value ?? string.Empty).ToString());

                outputUrl = outputUrl.Replace("{" + token.Key + "}", val);
            }

            return outputUrl;
        }

        #endregion
    }
}