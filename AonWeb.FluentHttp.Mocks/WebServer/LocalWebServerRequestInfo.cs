using System;
using System.Collections.Specialized;
using System.Text;

namespace AonWeb.FluentHttp.Mocks.WebServer
{
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