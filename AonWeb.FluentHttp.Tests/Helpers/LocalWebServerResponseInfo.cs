using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class LocalWebServerResponseInfo
    {
        public LocalWebServerResponseInfo()
        {
            Body = string.Empty;
            ContentEncoding = Encoding.UTF8;
            StatusCode = HttpStatusCode.OK;
            Headers = new Dictionary<string, string>();
        }

        public string Body { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public LocalWebServerResponseInfo AddHeader(string key, string value)
        {
            Headers[key] = value;

            return this;
        }
    }

    public class LocalWebServerRequestInfo
    {
        public LocalWebServerRequestInfo()
        {

        }

        public string Body { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public NameValueCollection Headers { get; set; }
        public string HttpMethod { get; set; }
        public Uri Url { get; set; }
        public bool HasEntityBody { get; set; }
        public long ContentLength { get; set; }
        public Uri UrlReferrer { get; set; }
        public string RawUrl { get; set; }
        public string[] AcceptTypes { get; set; }
        public string UserAgent { get; set; }
    }
}