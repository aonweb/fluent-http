using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHandlerContext : IModifiableHandlerContext
    {
        HttpRequestMessage Request { get; }
    }
}