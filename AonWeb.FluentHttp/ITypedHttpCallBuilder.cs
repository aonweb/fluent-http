using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface ITypedHttpCallBuilder
    {
        ITypedHttpCallBuilder WithUri(string uri);
        ITypedHttpCallBuilder WithUri(Uri uri);
        ITypedHttpCallBuilder WithBaseUri(string uri);
        ITypedHttpCallBuilder WithBaseUri(Uri uri);
        ITypedHttpCallBuilder WithRelativePath(string pathAndQuery);
        ITypedHttpCallBuilder WithQueryString(string name, string value);
        ITypedHttpCallBuilder WithQueryString(NameValueCollection values);
        ITypedHttpCallBuilder WithQueryString(IEnumerable<KeyValuePair<string, string>> values);
        ITypedHttpCallBuilder AsGet();
        ITypedHttpCallBuilder AsPut();
        ITypedHttpCallBuilder AsPost();
        ITypedHttpCallBuilder AsDelete();
        ITypedHttpCallBuilder AsPatch();
        ITypedHttpCallBuilder AsHead();
        ITypedHttpCallBuilder WithContent<TContent>(TContent content);
        ITypedHttpCallBuilder WithContent<TContent>(TContent content, Encoding encoding);
        ITypedHttpCallBuilder WithContent<TContent>(TContent content, Encoding encoding, string mediaType);
        ITypedHttpCallBuilder WithContent<TContent>(Func<TContent> contentFactory);
        ITypedHttpCallBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding);
        ITypedHttpCallBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding, string mediaType);
        ITypedHttpCallBuilder WithDefaultResult<TResult>(TResult result);
        ITypedHttpCallBuilder WithDefaultResult<TResult>(Func<TResult> resultFactory);
        ITypedHttpCallBuilder WithErrorType<TError>();
        Task<TResult> ResultAsync<TResult>();
        Task SendAsync();

        ITypedHttpCallBuilder CancelRequest();

        // conversion methods
        IAdvancedTypedHttpCallBuilder Advanced { get; }
    }
}