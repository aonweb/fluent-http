using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class TypedHandler : ITypedHandler
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual HandlerPriority GetPriority(HandlerType type)
        {
            return HandlerPriority.Default;
        }

        public virtual Task OnSending(TypedSendingContext context) { return Task.FromResult(true); }
        public virtual Task OnSent(TypedSentContext context) { return Task.FromResult(true); }
        public virtual Task OnResult(TypedResultContext context) { return Task.FromResult(true); }
        public virtual Task OnError(TypedErrorContext context) { return Task.FromResult(true); }
        public virtual Task OnException(TypedExceptionContext context) { return Task.FromResult(true); }
    }
}