using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public interface ITypedBuilder :
        IFluentConfigurableWithAdvanced<ITypedBuilder, IAdvancedTypedBuilder>,
        IHttpBuilderCore<ITypedBuilder>,
        IFluentConfigurable<ITypedBuilder, ITypedBuilderSettings>
    {
        Task<TResult> ResultAsync<TResult>();
        Task<TResult> ResultAsync<TResult>(CancellationToken token);
        Task SendAsync();
        Task SendAsync(CancellationToken token);
        void CancelRequest();
    }
}