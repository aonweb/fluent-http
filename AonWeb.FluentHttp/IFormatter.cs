using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface IFormatter
    {
        MediaTypeFormatterCollection MediaTypeFormatters { get; }

        Task<HttpContent> CreateContent(object value, ITypedBuilderContext context);
        Task<object> DeserializeResult(HttpResponseMessage response, ITypedBuilderContext context);
        Task<object> DeserializeError(HttpResponseMessage response, ITypedBuilderContext context);
    }
}