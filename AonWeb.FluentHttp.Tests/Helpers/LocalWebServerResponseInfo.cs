using System.Collections.Generic;
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
}