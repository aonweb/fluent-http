using System;
using System.Net;

using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheHandlerDefaults
    {
        public static bool CacheValidator(CacheContext context)
        {
            if (!context.Enabled)
                return false;

            if (context.Request == null || string.IsNullOrWhiteSpace(context.Key))
                return false;

            var request = context.Request;

            if (!context.CacheableMethods.Contains(request.Method))
                return false;

            // client can tell HttpCallCacheHandler not to do caching for a particular request
            // rather than expiring here and facing a thundering herd, let a success repopulate
            if (request.Headers.CacheControl != null)
            {
                if (request.Headers.CacheControl.NoStore)
                    return false;
            }

            if (typeof(IEmptyResult).IsAssignableFrom(context.ResultType))
                return false;

            return true;
        }

        public static bool RevalidateValidator(CacheContext context)
        {
            if (!context.Enabled)
                return false;

            var request = context.Request;

            if (request == null)
                return false;

            if (!context.CacheableMethods.Contains(request.Method))
                return false;

            return context.ResponseInfo != null && context.ResponseInfo.StatusCode == HttpStatusCode.NotModified;
        }

        public static ResponseValidationResult ResponseValidator(CacheContext context)
        {
            //This is almost verbatim from the CacheCow HttpCallCacheHandler's ResponseValidator func

            // 13.4
            //Unless specifically constrained by a cache-control (section 14.9) directive, a caching system MAY always store 
            // a successful response (see section 13.8) as a cache entry, MAY return it without validation if it 
            // is fresh, and MAY return it after successful validation. If there is neither a cache validator nor an 
            // explicit expiration time associated with a response, we do not expect it to be cached, but certain caches MAY violate this expectation 
            // (for example, when little or no network connectivity is available).

            // 14.9.1
            // If the no-cache directive does not specify a field-name, then a cache MUST NOT use the response to satisfy a subsequent request without 
            // successful revalidation with the origin server. This allows an origin server to prevent caching 
            // even by caches that have been configured to return stale responses to client requests.
            //If the no-cache directive does specify one or more field-names, then a cache MAY use the response 
            // to satisfy a subsequent request, subject to any other restrictions on caching. However, the specified 
            // field-name(s) MUST NOT be sent in the response to a subsequent request without successful revalidation 
            // with the origin server. This allows an origin server to prevent the re-use of certain header fields in a result, while still allowing caching of the rest of the response.

            var responseInfo = context.ResponseInfo;

            if (!context.CacheableStatusCodes.Contains(responseInfo.StatusCode))
                return ResponseValidationResult.NotCacheable;

            if (responseInfo.NoStore)
                return ResponseValidationResult.NotCacheable;

            if (!responseInfo.HasContent)
                return ResponseValidationResult.NotCacheable;

            if (!responseInfo.HasExpiration)
                return ResponseValidationResult.NotCacheable;

            if (responseInfo.NoCache)
                return ResponseValidationResult.MustRevalidate;

            if (responseInfo.Expiration < DateTimeOffset.UtcNow)
                return responseInfo.ShouldRevalidate ? ResponseValidationResult.MustRevalidate : ResponseValidationResult.Stale;

            return ResponseValidationResult.OK;
        }

        public static bool AllowStaleResultValidator(CacheContext context)
        {
            var request = context.Request;
            var responseInfo = context.ResponseInfo;

            //This is almost verbatim from the CacheCow HttpCallCacheHandler's  IsFreshOrStaleAcceptable 

            if (responseInfo == null)
                throw new ArgumentException(SR.CacheContextResponseInfoRequiredError, "context");

            if (request == null)
                throw new ArgumentException(SR.CacheContextRequestMessageRequiredError, "context");

            if (responseInfo.HasContent)
                return false;

            var responseDate = responseInfo.LastModified ?? responseInfo.Date;

            if (responseInfo.HasExpiration)
                if (responseInfo.Expiration < DateTimeOffset.UtcNow)
                    return false;

            var staleness = DateTimeOffset.UtcNow - responseDate;

            if (request.Headers.CacheControl == null)
                return staleness < TimeSpan.Zero;

            if (request.Headers.CacheControl.MinFresh.HasValue)
                return -staleness > request.Headers.CacheControl.MinFresh.Value; // staleness is negative if still fresh

            if (request.Headers.CacheControl.MaxStale) // stale acceptable
                return true;

            if (request.Headers.CacheControl.MaxStaleLimit.HasValue)
                return staleness < request.Headers.CacheControl.MaxStaleLimit.Value;

            if (request.Headers.CacheControl.MaxAge.HasValue)
                return responseDate.Add(request.Headers.CacheControl.MaxAge.Value) > DateTimeOffset.Now;

            return false;
        }

    }
}