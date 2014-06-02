using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

using AonWeb.FluentHttp.Mocks;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class LocalWebServerResponseInfo : ResponseInfo<LocalWebServerResponseInfo>
    {
        public LocalWebServerResponseInfo()
            :base( HttpStatusCode.OK) { }

        public LocalWebServerResponseInfo(HttpStatusCode statusCode)
            : base(statusCode) { }
    }

    public class LocalWebServerRequestInfo
    {

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