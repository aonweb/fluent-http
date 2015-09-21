using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;

namespace AonWeb.FluentHttp.Mocks.WebServer
{
    public class LocalRequestSettings : ILocalRequestSettings
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
        public long RequestCount { get; set; }
        public long RequestCountForThisUrl { get; set; }

        public ILocalRequestSettings WithConfiguration(Action<ILocalRequestSettings> configuration)
        {
            configuration?.Invoke(this);

            return this;
        }

        void IConfigurable<ILocalRequestSettings>.WithConfiguration(Action<ILocalRequestSettings> configuration)
        {
            WithConfiguration(configuration);
        }
    }
}