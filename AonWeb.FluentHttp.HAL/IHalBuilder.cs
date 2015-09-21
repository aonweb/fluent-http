using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.HAL.Representations;

namespace AonWeb.FluentHttp.HAL
{
    public interface IHalBuilder :
        IFluentConfigurableWithAdvanced<IHalBuilder, IAdvancedHalBuilder>,
        IFluentConfigurable<IHalBuilder, ITypedBuilderSettings>
    {
        Task<TResult> ResultAsync<TResult>() 
            where TResult : IHalResource;

        Task<TResult> ResultAsync<TResult>(CancellationToken token)
            where TResult : IHalResource;

        Task SendAsync();
        Task SendAsync(CancellationToken token);
        void CancelRequest();
    }
}