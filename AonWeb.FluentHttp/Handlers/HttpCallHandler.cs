using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public abstract class HttpCallHandler : IHttpCallHandler
    {
        public virtual HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            return HttpCallHandlerPriority.Default;
        }

        public virtual async Task OnSending(HttpCallContext context)        { /* do nothing */ }
        public virtual async Task OnSent(HttpCallContext context)           { /* do nothing */ }
        public virtual async Task OnException(HttpExceptionContext context) { /* do nothing */ }
    }

    public abstract class HttpCallHandler<TResult, TContent, TError> : IHttpCallHandler<TResult, TContent, TError>
    {
        public virtual HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            return HttpCallHandlerPriority.Default;
        }

        public virtual async Task OnSending(HttpCallContext<TResult, TContent, TError> context)        { /* do nothing */ }
        public virtual async Task OnSent(HttpCallContext<TResult, TContent, TError> context)           { /* do nothing */ }
        public virtual async Task OnResult(HttpCallContext<TResult, TContent, TError> context)         { /* do nothing */ }
        public virtual async Task OnError(HttpErrorContext<TResult, TContent, TError> context)         { /* do nothing */ }
        public virtual async Task OnException(HttpExceptionContext<TResult, TContent, TError> context) { /* do nothing */ }
    }

}