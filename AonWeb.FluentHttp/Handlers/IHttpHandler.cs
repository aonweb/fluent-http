using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpHandler : IHandler<HandlerType>
    {
        Task OnSending(HttpSendingContext context);
        Task OnSent(HttpSentContext context);
        Task OnException(HttpExceptionContext context);
    }
}