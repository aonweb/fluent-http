using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public static class MockHttpResponseMessageExtensions
    {
        public static MockHttpResponseMessage WithHttpContent(this MockHttpResponseMessage response, HttpContent content)
        {
            response.Content = content;
            return response;
        }

        public static MockHttpResponseMessage WithContent(this MockHttpResponseMessage response, string content)
        {
            response.ContentString = content;
            return response;
        }

        public static MockHttpResponseMessage WithContentEncoding(this MockHttpResponseMessage response, Encoding contentEncoding)
        {
            response.ContentEncoding = contentEncoding;
            return response;
        }

        public static MockHttpResponseMessage WithContentType(this MockHttpResponseMessage response, string contentType)
        {
            response.ContentType = contentType;
            return response;
        }

        public static MockHttpResponseMessage WithStatusCode(this MockHttpResponseMessage response, HttpStatusCode statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        public static MockHttpResponseMessage WithReasonPhrase(this MockHttpResponseMessage response, string statusDescription)
        {
            response.ReasonPhrase = statusDescription;
            return response;
        }

        public static MockHttpResponseMessage WithNoCacheNoStoreHeader(this MockHttpResponseMessage response)
        {
            return response.WithNoCacheHeader().WithNoStoreHeader();
        }

        public static MockHttpResponseMessage WithNoCacheHeader(this MockHttpResponseMessage response)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            response.Headers.CacheControl.NoCache = true;

            return response;
        }

        public static MockHttpResponseMessage WithNoStoreHeader(this MockHttpResponseMessage response)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            response.Headers.CacheControl.NoStore = true;

            return response;
        }

        public static MockHttpResponseMessage WithMustRevalidateHeader(this MockHttpResponseMessage response)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            response.Headers.CacheControl.MustRevalidate = true;

            return response;
        }

        public static MockHttpResponseMessage WithPublicCacheHeader(this MockHttpResponseMessage response)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            response.Headers.CacheControl.Public = true;
            response.Headers.CacheControl.Private = false;

            return response;
        }

        public static MockHttpResponseMessage WithPrivateCacheHeader(this MockHttpResponseMessage response)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            response.Headers.CacheControl.Public = false;
            response.Headers.CacheControl.Private = true;

            return response;
        }

        public static MockHttpResponseMessage WithEtag(this MockHttpResponseMessage response, string etag)
        {

            if (!string.IsNullOrWhiteSpace(etag))
            {
                if (!etag.StartsWith("\""))
                    etag = $"\"{etag}\"";

                response.Headers.ETag = EntityTagHeaderValue.Parse(etag);
            }
            else
            {
                response.Headers.ETag = null;
            }


            return response;
        }

        public static MockHttpResponseMessage WithMaxAge(this MockHttpResponseMessage response, TimeSpan maxAge)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            if (maxAge == TimeSpan.Zero)
                response.Headers.CacheControl.MaxAge = null;
            else
                response.Headers.CacheControl.MaxAge = maxAge;

            return response;
        }

        public static MockHttpResponseMessage WithDefaultExpiration(this MockHttpResponseMessage response)
        {
            return response.WithExpirationOrDefault();
        }

        public static MockHttpResponseMessage WithExpirationOrDefault(this MockHttpResponseMessage response, DateTimeOffset? expiration = null)
        {
            return response.WithExpiration(expiration ?? DateTimeOffset.UtcNow.AddMinutes(15));
        }

        public static MockHttpResponseMessage WithExpiration(this MockHttpResponseMessage response, DateTimeOffset expiration)
        {
            var exp = expiration.ToUniversalTime();
            response.WithMaxAge(exp - DateTimeOffset.UtcNow);

            if (response.Content != null)
                response.Content.Headers.Expires = exp;

            return response;
        }

        public static MockHttpResponseMessage WithCacheHeader(this MockHttpResponseMessage response, string publicity = "public", DateTimeOffset? expires = null, DateTimeOffset? lastModified = null, string etag = null, TimeSpan? maxAge = null)
        {
            response.Headers.CacheControl = response.Headers.CacheControl ?? new CacheControlHeaderValue();

            if (publicity == "public")
                response.WithPublicCacheHeader();
            else
                response.WithPrivateCacheHeader();


            if (maxAge != null)
                response.WithMaxAge(maxAge.Value);


            if (response.Content != null)
                response.Content.Headers.LastModified = lastModified ?? DateTimeOffset.UtcNow;

            response.WithEtag(etag);

            return response;
        }

        public static MockHttpResponseMessage WithHeader(this MockHttpResponseMessage response, string key, string value)
        {
            response.Headers.TryAddWithoutValidation(key, value);

            return response;
        }
    }
}