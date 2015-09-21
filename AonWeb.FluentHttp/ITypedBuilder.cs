using System.Threading;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp
{
    public interface ITypedBuilder :
        IFluentConfigurableWithAdvanced<ITypedBuilder, IAdvancedTypedBuilder>,
        IFluentConfigurable<ITypedBuilder, IHttpBuilderSettings>,
        IFluentConfigurable<ITypedBuilder, ITypedBuilderSettings>
    {
        Task<TResult> ResultAsync<TResult>();
        Task<TResult> ResultAsync<TResult>(CancellationToken token);
        Task SendAsync();
        Task SendAsync(CancellationToken token);
        void CancelRequest();
    }
}