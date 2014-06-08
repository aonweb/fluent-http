using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHttpCallHandler
    {
        bool Enabled { get; }
        HttpCallHandlerPriority GetPriority(HttpCallHandlerType type);
        Task OnSending(HttpSendingContext context);
        Task OnSent(HttpSentContext context);
        Task OnException(HttpExceptionContext context);
    }

    public interface ITypedHttpCallHandler
    {
        bool Enabled { get; }
        HttpCallHandlerPriority GetPriority(HttpCallHandlerType type);

        Task OnSending<TResult, TContent>(TypedHttpSendingContext<TResult, TContent> context);
        Task OnSent<TResult>(TypedHttpSentContext<TResult> context);
        Task OnResult<TResult>(TypedHttpResultContext<TResult> context);
        Task OnError<TError>(TypedHttpCallErrorContext<TError> context);
        Task OnException(TypedHttpCallExceptionContext context);
    }
    
    public interface ITypedHttpCallHandler<TResult, TContent, TError>: ITypedHttpCallHandler
    {
        Task OnSending(TypedHttpSendingContext<TResult, TContent> context);
        Task OnSent(TypedHttpSentContext<TResult> context);
        Task OnResult(TypedHttpResultContext<TResult> context);
        Task OnError(TypedHttpCallErrorContext<TError> context);
    }

    public interface IBoxedHttpCallHandler : ITypedHttpCallHandler<object, object, object>
    {
    }
}