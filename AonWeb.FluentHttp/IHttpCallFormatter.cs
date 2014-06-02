using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IHttpCallFormatter<TResult, TContent, TError>
    {
        Task<HttpContent> CreateContent<T>(T value, HttpCallContext<TResult, TContent, TError> context);

        Task<TResult> DeserializeResult(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context);

        Task<TError> DeserializeError(HttpResponseMessage response, HttpCallContext<TResult, TContent, TError> context);
    }
}