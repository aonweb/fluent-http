using System.Net.Http;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp
{
    public interface IFormatter
    {
        Task<HttpContent> CreateContent(object value, ITypedBuilderContext context);
        Task<object> DeserializeResult(HttpResponseMessage response, ITypedBuilderContext context);
        Task<object> DeserializeError(HttpResponseMessage response, ITypedBuilderContext context);
    }
}