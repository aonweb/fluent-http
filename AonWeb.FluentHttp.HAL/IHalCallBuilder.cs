using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public interface IHalCallBuilder<TResult, TContent, TError>
        where TResult : IHalResource
        where TContent : IHalRequest
    {
        IHalCallBuilder<TResult, TContent, TError> WithLink(string link);
        IHalCallBuilder<TResult, TContent, TError> WithLink(Uri link);
        IHalCallBuilder<TResult, TContent, TError> WithLink(Func<string> linkFactory);
        IHalCallBuilder<TResult, TContent, TError> WithLink(Func<Uri> linkFactory);
        IHalCallBuilder<TResult, TContent, TError> WithQueryString(string name, string value);
        IHalCallBuilder<TResult, TContent, TError> WithQueryString(NameValueCollection values);
        IHalCallBuilder<TResult, TContent, TError> AsGet();
        IHalCallBuilder<TResult, TContent, TError> AsPut();
        IHalCallBuilder<TResult, TContent, TError> AsPost();
        IHalCallBuilder<TResult, TContent, TError> AsDelete();
        IHalCallBuilder<TResult, TContent, TError> AsPatch();
        IHalCallBuilder<TResult, TContent, TError> AsHead();
        IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content);
        IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding);
        IHalCallBuilder<TResult, TContent, TError> WithContent(TContent content, Encoding encoding, string mediaType);
        IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory);
        IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding);
        IHalCallBuilder<TResult, TContent, TError> WithContent(Func<TContent> contentFactory, Encoding encoding, string mediaType);
        IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(TResult result);
        IHalCallBuilder<TResult, TContent, TError> WithDefaultResult(Func<TResult> resultFunc);

        TResult Result();
        Task<TResult> ResultAsync();
        Task Send();

        IHalCallBuilder<TResult, TContent, TError> CancelRequest();

        IAdvancedHalCallBuilder<TResult, TContent, TError> Advanced { get; }
    }
}