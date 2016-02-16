using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public interface IBuilderContext<out TBuilder> : IContext
    {
        TBuilder Builder { get; }
        bool SuppressCancellationErrors { get; }
        bool IsSuccessfulResponse(HttpResponseMessage response);
    }
}