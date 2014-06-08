using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public interface IHalCallBuilder
    {
        IHalCallBuilder WithLink(string link);
        IHalCallBuilder WithLink(Uri link);
        IHalCallBuilder WithLink(Func<string> linkFactory);
        IHalCallBuilder WithLink(Func<Uri> linkFactory);
        IHalCallBuilder WithQueryString(string name, string value);
        IHalCallBuilder WithQueryString(NameValueCollection values);
        IHalCallBuilder AsGet();
        IHalCallBuilder AsPut();
        IHalCallBuilder AsPost();
        IHalCallBuilder AsDelete();
        IHalCallBuilder AsPatch();
        IHalCallBuilder AsHead();
        IHalCallBuilder WithContent<TContent>(TContent content)
            where TContent : IHalRequest;
        IHalCallBuilder WithContent<TContent>(TContent content, Encoding encoding)
            where TContent : IHalRequest;
        IHalCallBuilder WithContent<TContent>(TContent content, Encoding encoding, string mediaType)
            where TContent : IHalRequest;
        IHalCallBuilder WithContent<TContent>(Func<TContent> contentFactory)
            where TContent : IHalRequest;
        IHalCallBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding)
            where TContent : IHalRequest;
        IHalCallBuilder WithContent<TContent>(Func<TContent> contentFactory, Encoding encoding, string mediaType)
            where TContent : IHalRequest;
        IHalCallBuilder WithDefaultResult<TResult>(TResult result)
            where TResult : IHalResource;
        IHalCallBuilder WithDefaultResult<TResult>(Func<TResult> resultFactory)
            where TResult : IHalResource;

        IHalCallBuilder WithErrorType<TError>();

        Task<TResult> ResultAsync<TResult>()
            where TResult : IHalResource;

        Task SendAsync();

        IHalCallBuilder CancelRequest();

        IAdvancedHalCallBuilder Advanced { get; }
    }
}