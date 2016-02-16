using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface IChildHttpBuilder : IRecursiveHttpBuilder
    {
        Task<HttpResponseMessage> ResultFromRequestAsync(HttpRequestMessage request, CancellationToken token);
        Task<HttpRequestMessage> CreateRequest();
    }
}