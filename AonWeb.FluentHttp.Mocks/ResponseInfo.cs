using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{


    public class ResponseInfo : ResponseInfo<ResponseInfo>
    {
        public ResponseInfo()
            :base( HttpStatusCode.OK) { }

        public ResponseInfo(HttpStatusCode statusCode)
            : base(statusCode) { }

        public ResponseInfo(HttpStatusCode statusCode, string content)
            : base(statusCode)
        {
            Body = content;
        }
    }

    public abstract class ResponseInfo<T>
        where T : ResponseInfo<T>
    {
        protected ResponseInfo(HttpStatusCode statusCode)
        {
            Body = string.Empty;
            ContentType = "application/json";
            ContentEncoding = Encoding.UTF8;
            StatusCode = statusCode;
            Headers = new Dictionary<string, string>();
        }

        public string Body { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public T AddHeader(string key, string value)
        {
            Headers[key] = value;

            return this as T;
        }

        public T AddNoCacheHeader()
        {
            return AddHeader("Cache-Control", "no-cache, no-store");
        }

        public T AddPublicCacheHeader(DateTime? expires = null)
        {
            return AddCacheHeader("public", expires);
        }

        public T AddPrivateCacheHeader(DateTime? expires = null)
        {
            return AddCacheHeader("private", expires);
        }

        public T AddCacheHeader(string publicity = "public", DateTime? expires = null)
        {
            var exp = expires.GetValueOrDefault(DateTime.UtcNow.AddMinutes(15)).ToUniversalTime();
            return AddHeader("Cache-Control", publicity + ", max-age=" + (exp - DateTime.UtcNow).TotalSeconds)
                    .AddHeader("Expires", exp.ToString("R"))
                    .AddHeader("Last-Modified", DateTime.UtcNow.ToString("R")) as T;
        }

        public HttpResponseMessage ToHttpResponseMessage()
        {
            var response = new HttpResponseMessage(StatusCode)
            {
                ReasonPhrase = StatusDescription ?? StatusCode.ToString()
            };

            foreach (var header in Headers)
                response.Headers.TryAddWithoutValidation(header.Key, header.Value);

            response.Headers.Date = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(Body))
            {
                 var buffer = ContentEncoding.GetBytes(Body);
                response.Content = new ByteArrayContent(buffer);

                 if (!string.IsNullOrWhiteSpace(ContentType))
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            return response;
        }
    }
}