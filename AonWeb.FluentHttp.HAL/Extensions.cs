using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public static class Extensions
    {
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

        public static string GetLink(this IHalResource resource, string key)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.GetLink(key);
        }

        public static string GetLink(this IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return resource.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static string GetLink(this IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.GetLink(key, tokens);
        }

        public static string GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key)
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

        public static string GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            return linkEntity.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static string GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            var formatUrl = linkEntity.GetLink(key);

            if (tokens == null)
                throw new ArgumentNullException("tokens");

            var output = formatUrl;

            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token.Key))
                    throw new ArgumentException(string.Format("A supplied url token for url '{0}' was null or empty.", formatUrl));

                var val = HttpUtility.UrlEncode((token.Value ?? string.Empty).ToString());

                output = output.Replace("{" + token.Key + "}", val);
            }

            return output;
        }

        public static string GetSelf(this IHalResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException("resource");

            return resource.Links.GetSelf();
        }

        public static string GetSelf(this IEnumerable<HyperMediaLink> linkEntity)
        {
            return linkEntity.GetLink(HalResource.LinkKeySelf);
        }
    }
}