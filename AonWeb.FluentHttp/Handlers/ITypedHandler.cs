using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface ITypedHandler<TResult, TContent, TError> : ITypedHandler
    {
        Task OnSending(TypedSendingContext<TResult, TContent> context);
        Task OnSent(TypedSentContext<TResult> context);
        Task OnResult(TypedResultContext<TResult> context);
        Task OnError(TypedErrorContext<TError> context);
    }

    public interface ITypedHandler : IHandler<HandlerType>
    {
        Task OnSending<TResult, TContent>(TypedSendingContext<TResult, TContent> context);
        Task OnSent<TResult>(TypedSentContext<TResult> context);
        Task OnResult<TResult>(TypedResultContext<TResult> context);
        Task OnError<TError>(TypedErrorContext<TError> context);
        Task OnException(TypedExceptionContext context);
    }
}