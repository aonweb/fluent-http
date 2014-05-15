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
            var uri = GetLinkString(linkEntity, key);

            return new Uri(uri);
        }

        private static string GetLinkString(this IEnumerable<HyperMediaLink> linkEntity, string key)
        {
            if (linkEntity == null)
                throw new ArgumentNullException("linkEntity");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            if (!linkEntity.Any())
                throw new ArgumentException("HyperMediaLinks entity does not contain any links");

            HyperMediaLink link;

            try
            {
                link = linkEntity.Single(l => string.Equals(l.Rel, key, StringComparison.OrdinalIgnoreCase));
            }
            catch (InvalidOperationException ex)
            {
                throw new KeyNotFoundException(string.Format("Could not locate key '{0}' in HyperMediaLinks entity Links collection", key), ex);
            }

            return link.Href;
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            return linkEntity.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            var formatUrl = linkEntity.GetLinkString(key);

            if (tokens == null)
                throw new ArgumentNullException("tokens");

            var outputUrl = formatUrl;

            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Key))
                    throw new ArgumentException(string.Format("A supplied url token for url '{0}' was null or empty.", formatUrl));

                var val = HttpUtility.UrlEncode((token.Value ?? string.Empty).ToString());

                outputUrl = outputUrl.Replace("{" + token.Key + "}", val);
            }

            return new Uri(outputUrl);
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

            if (!linkEntity.HasLink(key)) 
                return null;

            return linkEntity.GetLink(key);
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            return linkEntity.TryGetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            if (!linkEntity.HasLink(key))
                return null;

            return linkEntity.GetLink(key, tokens);
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
    }
}