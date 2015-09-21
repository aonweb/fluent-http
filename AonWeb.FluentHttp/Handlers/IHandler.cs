using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHandler<in THandlerType>
    {
        bool Enabled { get; }
        HandlerPriority GetPriority(THandlerType type);
    }

    public interface IHandler : IHandler<HandlerType>
    {
        Task OnSending(SendingContext context);
        Task OnSent(SentContext context);
        Task OnException(ExceptionContext context);
    }
}