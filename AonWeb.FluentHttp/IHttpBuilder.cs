using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{

    public interface IHttpBuilderCore<out TBuilder> : IFluentConfigurable<TBuilder, IHttpBuilderSettings> 
        where TBuilder : IFluentConfigurable<TBuilder, IHttpBuilderSettings>
    { }

    public interface IHttpBuilder : 
        IFluentConfigurableWithAdvanced<IHttpBuilder, IAdvancedHttpBuilder>,
        IHttpBuilderCore<IHttpBuilder>
    {
        Task<HttpResponseMessage> ResultAsync();
        Task<HttpResponseMessage> ResultAsync(CancellationToken token);
        void CancelRequest();
    }
}