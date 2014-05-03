using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface IHttpCallBuilder
    {
        IHttpCallBuilder WithUri(string uri);
        IHttpCallBuilder WithUri(Uri uri);
        IHttpCallBuilder WithQueryString(string name, string value);
        IHttpCallBuilder WithMethod(string method);
        IHttpCallBuilder WithMethod(HttpMethod method);
        IHttpCallBuilder WithContent(string content);
        IHttpCallBuilder WithContent(string content, Encoding encoding);
        IHttpCallBuilder WithContent(string content, Encoding encoding, string mediaType);
        IHttpCallBuilder WithContent(Func<string> contentFactory);
        IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding);
        IHttpCallBuilder WithContent(Func<string> contentFactory, Encoding encoding, string mediaType);
        IHttpCallBuilder WithContent(Func<HttpContent> contentFactory);
        HttpResponseMessage Result();
        Task<HttpResponseMessage> ResultAsync();

        IHttpCallBuilder CancelRequest();

        IAdvancedHttpCallBuilder Advanced { get; }
    }

    public interface IHttpCallBuilder<TResult, TContent, TError>
    {
        IHttpCallBuilder<TResult, TContent, TError> WithUri(string uri);
        IHttpCallBuilder<TResult, TContent, TError> WithUri(Uri uri);
        IHttpCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value);
        IHttpCallBuilder<TResult, TContent, TError> WithMethod(string method);
        IHttpCallBuilder<TResult, TContent, TError> WithMethod(HttpMethod method);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding);
        IHttpCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding, string mediaType);
        IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result);
        IHttpCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFactory);

        TResult Result();
        Task<TResult> ResultAsync();

        IHttpCallBuilder<TResult, TContent, TError> CancelRequest();

        // conversion methods
        IAdvancedHttpCallBuilder<TResult, TContent, TError> Advanced { get; }
    }
}