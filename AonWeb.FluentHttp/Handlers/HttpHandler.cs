using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class HttpHandler : IHttpHandler
    {
        public virtual bool Enabled { get; set; } = true;

        public virtual HandlerPriority GetPriority(HandlerType type)
        {
            return HandlerPriority.Default;
        }

        public virtual Task OnSending(HttpSendingContext context)     { return Task.Delay(0); }
        public virtual Task OnSent(HttpSentContext context)           { return Task.Delay(0); }
        public virtual Task OnException(HttpExceptionContext context) { return Task.Delay(0); }
    }
}