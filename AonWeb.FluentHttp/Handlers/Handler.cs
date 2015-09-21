using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class Handler : IHandler
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual HandlerPriority GetPriority(HandlerType type)
        {
            return HandlerPriority.Default;
        }

        public virtual Task OnSending(SendingContext context)     { return Task.Delay(0); }
        public virtual Task OnSent(SentContext context)           { return Task.Delay(0); }
        public virtual Task OnException(ExceptionContext context) { return Task.Delay(0); }
    }

    public abstract class TypedHandler : ITypedHandler
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual HandlerPriority GetPriority(HandlerType type)
        {
            return HandlerPriority.Default;
        }

        public virtual Task OnSending<TResult, TContent>(TypedSendingContext<TResult, TContent> context) { return Task.Delay(0); }
        public virtual Task OnSent<TResult>(TypedSentContext<TResult> context) { return Task.Delay(0); }
        public virtual Task OnResult<TResult>(TypedResultContext<TResult> context) { return Task.Delay(0); }
        public virtual Task OnError<TError>(TypedErrorContext<TError> context) { return Task.Delay(0); }
        public virtual Task OnException(TypedExceptionContext context) { return Task.Delay(0); }
    }

}