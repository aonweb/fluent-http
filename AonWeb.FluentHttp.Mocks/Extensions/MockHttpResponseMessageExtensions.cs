using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public static class MockHttpResponseMessageExtensions
    {
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

        public static MockHttpResponseMessage WithNoCacheHeader(this MockHttpResponseMessage response)
        {
            if (response.Headers.CacheControl == null)
                response.Headers.CacheControl = new CacheControlHeaderValue();

            response.Headers.CacheControl.NoCache = true;
            response.Headers.CacheControl.NoStore = true;

            return response;
        }

        public static MockHttpResponseMessage WithPublicCacheHeader(this MockHttpResponseMessage response, DateTimeOffset? expires = null)
        {
            return response.WithCacheHeader("public", expires);
        }

        public static MockHttpResponseMessage WithPrivateCacheHeader(this MockHttpResponseMessage response, DateTimeOffset? expires = null)
        {
            return response.WithCacheHeader("private", expires);
        }

        public static MockHttpResponseMessage WithCacheHeader(this MockHttpResponseMessage response, string publicity = "public", DateTimeOffset? expires = null)
        {
            if (response.Headers.CacheControl == null)
                response.Headers.CacheControl = new CacheControlHeaderValue();

            if (publicity == "public")
            {
                response.Headers.CacheControl.Public = true;
                response.Headers.CacheControl.Private = false;
            }
            else
            {
                response.Headers.CacheControl.Public = false;
                response.Headers.CacheControl.Private = true;
            }

            var exp = expires.GetValueOrDefault(DateTimeOffset.UtcNow.AddMinutes(15)).ToUniversalTime();
            response.Headers.CacheControl.MaxAge = exp - DateTimeOffset.UtcNow;

            if (response.Content == null)
                response.Content = new StringContent("");

            response.Content.Headers.Expires = exp;
            response.Content.Headers.LastModified = DateTimeOffset.UtcNow;

            return response;
        }

        public static MockHttpResponseMessage WithHeader(this MockHttpResponseMessage response, string key, string value)
        {
            response.Headers.TryAddWithoutValidation(key, value);

            return response;
        }
    }
}