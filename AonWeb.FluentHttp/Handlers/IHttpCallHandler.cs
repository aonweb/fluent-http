using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IHandler<in THandlerType>
    {
        bool Enabled { get; }
        HttpCallHandlerPriority GetPriority(THandlerType type);
    }

    public interface IHttpCallHandler : IHandler<HttpCallHandlerType>
    {
        Task OnSending(HttpSendingContext context);
        Task OnSent(HttpSentContext context);
        Task OnException(HttpExceptionContext context);
    }

    public interface ITypedHttpCallHandler : IHandler<HttpCallHandlerType>
    {
        Task OnSending<TResult, TContent>(TypedHttpSendingContext<TResult, TContent> context);
        Task OnSent<TResult>(TypedHttpSentContext<TResult> context);
        Task OnResult<TResult>(TypedHttpResultContext<TResult> context);
        Task OnError<TError>(TypedHttpErrorContext<TError> context);
        Task OnException(TypedHttpExceptionContext context);
    }
    
    public interface ITypedHttpCallHandler<TResult, TContent, TError>: ITypedHttpCallHandler
    {
        Task OnSending(TypedHttpSendingContext<TResult, TContent> context);
        Task OnSent(TypedHttpSentContext<TResult> context);
        Task OnResult(TypedHttpResultContext<TResult> context);
        Task OnError(TypedHttpErrorContext<TError> context);
    }

    public interface IBoxedHttpCallHandler : ITypedHttpCallHandler<object, object, object>
    {
    }
}