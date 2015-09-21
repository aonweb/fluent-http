using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface IRecursiveHttpBuilder : IAdvancedHttpBuilder
    {
        Task<HttpResponseMessage> RecursiveResultAsync(CancellationToken token);
    }
}