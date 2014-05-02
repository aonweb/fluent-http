using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpCallHandler
    {
        HttpCallHandlerPriority GetPriority(HttpCallHandlerType type);
        Task OnSending(HttpCallContext context);
        Task OnSent(HttpCallContext context);
        Task OnException(HttpExceptionContext context);
    }

    public interface IHttpCallHandler<TResult, TContent, TError>
    {
        HttpCallHandlerPriority GetPriority(HttpCallHandlerType type);
        Task OnSending(HttpCallContext<TResult, TContent, TError> context);
        Task OnSent(HttpCallContext<TResult, TContent, TError> context);
        Task OnResult(HttpCallContext<TResult, TContent, TError> context);
        Task OnError(HttpErrorContext<TResult, TContent, TError> context);
        Task OnException(HttpExceptionContext<TResult, TContent, TError> context);
    }
}