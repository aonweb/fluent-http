using System.Net.Http;
using System.Threading;

namespace AonWeb.FluentHttp
{
    public interface IBuilderContext<out TBuilder, out TSettings> : IContextWithSettings<TSettings>
    {
        TBuilder Builder { get; }
        bool SuppressCancellationErrors { get; }
        bool IsSuccessfulResponse(HttpResponseMessage response);
    }
}