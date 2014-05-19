using System;
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

    public interface IHttpCallBuilder<TResult, TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> WithUri(string uri);
        IHttpCallBuilder<TResult, TContent, TError> WithUri(Uri uri);
        IHttpCallBuilder<TResult, TContent, TError> WithBaseUri(string uri);
        IHttpCallBuilder<TResult, TContent, TError> WithBaseUri(Uri uri);
        IHttpCallBuilder<TResult, TContent, TError> WithRelativePath(string pathAndQuery);
        IHttpCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value);
        IHttpCallBuilder<TResult, TContent, TError> WithQueryString(NameValueCollection values);
        IHttpCallBuilder<TResult, TContent, TError> AsGet();
        IHttpCallBuilder<TResult, TContent, TError> AsPut();
        IHttpCallBuilder<TResult, TContent, TError> AsPost();
        IHttpCallBuilder<TResult, TContent, TError> AsDelete();
        IHttpCallBuilder<TResult, TContent, TError> AsPatch();
        IHttpCallBuilder<TResult, TContent, TError> AsHead();
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding, string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result);
        IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFactory);

        Task<TResult> ResultAsync();
        Task SendAsync();

        IHttpCallBuilder<TResult, TContent, TError> CancelRequest();

        // conversion methods
        IAdvancedHttpCallBuilder<TResult, TContent, TError> Advanced { get; }
    }
}