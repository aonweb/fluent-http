using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface ITypedHandler : IHandler<HandlerType>
    {
        Task OnSending(TypedSendingContext context);
        Task OnSent(TypedSentContext context);
        Task OnResult(TypedResultContext context);
        Task OnError(TypedErrorContext context);
        Task OnException(TypedExceptionContext context);
    }
}