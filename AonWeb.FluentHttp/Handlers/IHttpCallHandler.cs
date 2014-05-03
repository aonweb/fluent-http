using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpCallHandler
    {
        HttpCallHandlerPriority GetPriority(HttpCallHandlerType type);
        Task OnSending(HttpSendingContext context);
        Task OnSent(HttpSentContext context);
        Task OnException(HttpExceptionContext context);
    }

    public interface IHttpCallHandler<TResult, TContent, TError>
    {
        HttpCallHandlerPriority GetPriority(HttpCallHandlerType type);
        Task OnSending(HttpSendingContext<TResult, TContent, TError> context);
        Task OnSent(HttpSentContext<TResult, TContent, TError> context);
        Task OnResult(HttpResultContext<TResult, TContent, TError> context);
        Task OnError(HttpErrorContext<TResult, TContent, TError> context);
        Task OnException(HttpExceptionContext<TResult, TContent, TError> context);
    }
}