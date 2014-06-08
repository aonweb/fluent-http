using System;
using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IHttpCallFormatter
    {
        Task<HttpContent> CreateContent(object value, TypedHttpCallContext context);

        Task<object> DeserializeResult(HttpResponseMessage response, TypedHttpCallContext context);

        Task<object> DeserializeError(HttpResponseMessage response, TypedHttpCallContext context);
    }
}