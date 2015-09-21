using System;
using System.Collections.Specialized;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public interface ILocalRequestSettings: ILocalRequestContext, IFluentConfigurable<ILocalRequestSettings, ILocalRequestSettings>
    {
        new string Body { get; set; }
        new Encoding ContentEncoding { get; set; }
        new string ContentType { get; set; }
        new NameValueCollection Headers { get; set; }
        new string HttpMethod { get; set; }
        new Uri Url { get; set; }
        new bool HasEntityBody { get; set; }
        new long ContentLength { get; set; }
        new Uri UrlReferrer { get; set; }
        new string RawUrl { get; set; }
        new string[] AcceptTypes { get; set; }
        new string UserAgent { get; set; }
        new long RequestCount { get; set; }
        new long RequestCountForThisUrl { get; set; }
    }
}