using System;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.HAL.Serialization.Exceptions;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public static class HypermediaLinkHelpers
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
                throw new MissingLinkException(key, (Type)null);

            return resource.Links.GetLink(key);
        }

        public static Uri GetLink(this IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            if (resource == null)
                throw new MissingLinkException(key, (Type)null);

            return resource.Links.GetLink(key, tokenKey, tokenValue);
        }

        public static Uri GetLink(this IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            if (resource == null)
                throw new MissingLinkException(key, (Type)null);

            return resource.Links.GetLink(key, tokens);
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key)
        {
            var link = linkEntity.GetLinkImplementation(key);

            if (link.Templated)
                throw new MissingLinkException($"Cannot get link value. Link for rel '{key}' with uri '{link.Href}' on type {linkEntity?.GetType().FormattedTypeName()} is templated and no template parameters were provided.");

            Uri uri;
            if (!Uri.TryCreate(link.Href, UriKind.Absolute, out uri))
                throw new MissingLinkException($"Cannot get link value. Link for rel '{key}' with value '{link.Href}' on type {linkEntity?.GetType().FormattedTypeName()} is not a valid uri.");

            return uri;
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            if (string.IsNullOrWhiteSpace(tokenKey))
            {
                var uri = linkEntity.TryGetLink(key);
                 
                throw new InvalidTokenException($"A token for rel '{key}' with templated uri '{uri}' on type {linkEntity?.GetType().FormattedTypeName()} was null or empty.");
            }

            return linkEntity.GetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri GetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            var url = linkEntity.GetTemplatedLinkString(key, tokens);

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                throw new MissingLinkException($"Cannot get link value. Link for rel '{key}' with value '{url}' on type {linkEntity?.GetType().FormattedTypeName()} is not a valid uri.");

            return uri;
        }

        #endregion

        #region TryGetLink

        public static Uri TryGetLink(this IHalResource resource, string key)
        {
            return resource?.Links.TryGetLink(key);
        }

        public static Uri TryGetLink(this IHalResource resource, string key, string tokenKey, object tokenValue)
        {
            return resource.TryGetLink(key, new Dictionary<string, object> { { tokenKey ?? string.Empty, tokenValue } });
        }

        public static Uri TryGetLink(this IHalResource resource, string key, IDictionary<string, object> tokens)
        {
            return resource?.Links.TryGetLink(key, tokens);
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key)
        {
            var link = linkEntity?.GetLinkImplementation(key, false);

            Uri uri;
            if (link == null || !Uri.TryCreate(link.Href, UriKind.Absolute, out uri))
                return null;

            return uri;
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, string tokenKey, object tokenValue)
        {
            return linkEntity.TryGetLink(key, new Dictionary<string, object> { { tokenKey, tokenValue } });
        }

        public static Uri TryGetLink(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens)
        {
            var link = linkEntity.GetTemplatedLinkString(key, tokens, false);

            Uri uri;
            if (link == null || !Uri.TryCreate(link, UriKind.Absolute, out uri))
                return null;

            return uri;
        }

        #endregion

        #region GetSelf

        public static Uri GetSelf(this IHalResource resource)
        {
            if (resource == null)
                throw new MissingLinkException(HalResource.LinkKeySelf, (Type)null);

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
            if (linkEntity == null || string.IsNullOrWhiteSpace(key))
            {
                if (throwOnMissingLink)
                    throw new MissingLinkException(key, linkEntity?.GetType());

                return null;
            }

            var link = linkEntity.FirstOrDefault(l => string.Equals(l.Rel, key, StringComparison.OrdinalIgnoreCase));

            if (link == null)
            {
                if (throwOnMissingLink)
                    throw new MissingLinkException(key, linkEntity.GetType());

                return null;
            }

            return link;
        }

        private static string GetTemplatedLinkString(this IEnumerable<HyperMediaLink> linkEntity, string key, IDictionary<string, object> tokens, bool throwOnError = true)
        {
            var link = linkEntity.GetLinkImplementation(key, throwOnError);

            if (link == null)
                return null;

            if ((tokens == null || !tokens.Any()) && !link.Templated)
                return link.Href;

            var formatUrl = link.Href;

            if (throwOnError && !link.Templated)
                throw new InvalidTokenException($"Cannot get templated link value. Link for rel '{key}' with uri '{formatUrl}' on type {linkEntity?.GetType().FormattedTypeName()} is not a templated link.");

            if (throwOnError && (tokens == null || !tokens.Any()))
                throw new InvalidTokenException($"Cannot get templated link value. No tokens provided for rel '{key}' with templated uri '{formatUrl}' on type {linkEntity?.GetType().FormattedTypeName()}.");

            var outputUrl = formatUrl;

            foreach (var token in (tokens ?? Enumerable.Empty<KeyValuePair<string, object>>()))
            {
                if (string.IsNullOrWhiteSpace(token.Key))
                {
                    if (throwOnError)
                        throw new InvalidTokenException($"Cannot get templated link value. A token for rel '{key}' with templated uri '{formatUrl}' on type {linkEntity?.GetType().FormattedTypeName()} was null or empty.");
                    
                    continue;
                }

                var tokenKey = "{" + token.Key + "}";
                var valueString = (token.Value ?? string.Empty).ToString();
                var encodedValue = Uri.EscapeDataString(valueString);

                if (outputUrl.IndexOf(tokenKey, StringComparison.Ordinal) < 0)
                {
                    if (throwOnError)
                        throw new InvalidTokenException($"Cannot get templated link value. Token '{token.Key}' with value '{valueString}' for rel '{key}' with templated uri '{formatUrl}' on type {linkEntity?.GetType().FormattedTypeName()} does not exist.");

                    continue;
                }

                outputUrl = outputUrl.Replace(tokenKey, encodedValue);
            }

            if (!throwOnError)
                return outputUrl;

            var end = 0;
            List<string> missedTokens = null;

            while (true)
            {
                var start = outputUrl.IndexOf("{", end, StringComparison.Ordinal);

                if (start < 0)
                    break;

                end = outputUrl.IndexOf("}", start + 1, StringComparison.Ordinal);

                if (end < 0)
                    break;

                var token = outputUrl.Substring(start + 1, end - start);
                missedTokens = missedTokens ?? new List<string>();

                missedTokens.Add(token);
            }

            if (missedTokens == null)
                return outputUrl;

            var missedTokenString = string.Join(", ", missedTokens);
            throw new InvalidTokenException($"Cannot get templated link value. Token(s) {missedTokenString} where not replaced with values for rel '{key}' with templated uri '{formatUrl}' on type {linkEntity?.GetType().FormattedTypeName()}.");
        }

        #endregion
    }
}