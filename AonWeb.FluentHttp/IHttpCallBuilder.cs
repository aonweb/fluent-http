using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface IHttpCallBuilder
    {
        IHttpCallBuilder WithUri(string uri);
        IHttpCallBuilder WithUri(Uri uri);
        IHttpCallBuilder WithBaseUri(string uri);
        IHttpCallBuilder WithBaseUri(Uri uri);
        IHttpCallBuilder WithRelativePath(string pathAndQuery);
        IHttpCallBuilder WithQueryString(string name, string value);
        IHttpCallBuilder WithQueryString(NameValueCollection values);
        IHttpCallBuilder WithQueryString(IEnumerable<KeyValuePair<string, string>> values);
        IHttpCallBuilder AsGet();
        IHttpCallBuilder AsPut();
        IHttpCallBuilder AsPost();
        IHttpCallBuilder AsDelete();
        IHttpCallBuilder AsPatch();
        IHttpCallBuilder AsHead();
        IHttpCallBuilder WithContent(string content);
        IHttpCallBuilder WithContent(string content, Encoding encoding);
        IHttpCallBuilder WithContent(string content, Encoding encoding, string mediaType);
        IHttpCallBuilder WithContent(Func<string> contentFactory);
        IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding);
        IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding, string mediaType);
        IHttpCallBuilder WithContent(Func<HttpContent> contentFactory);
        Task<HttpResponseMessage> ResultAsync();

        IHttpCallBuilder CancelRequest();

        IAdvancedHttpCallBuilder Advanced { get; }
    }
}