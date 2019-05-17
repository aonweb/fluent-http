using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface IHttpBuilder : 
        IFluentConfigurableWithAdvanced<IHttpBuilder, IAdvancedHttpBuilder>,
        IHttpBuilderCore<IHttpBuilder>
    {
        Task<HttpResponseMessage> ResultAsync(CancellationToken token);
        void CancelRequest();
    }
}