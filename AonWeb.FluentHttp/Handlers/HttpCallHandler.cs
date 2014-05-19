using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    [ExcludeFromCodeCoverage]
    public abstract class HttpCallHandler : IHttpCallHandler
    {
        private bool _enabled = true;

        public virtual bool Enabled { get{ return _enabled; } set { _enabled = value; } }

        public virtual HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            return HttpCallHandlerPriority.Default;
        }

        public virtual Task OnSending(HttpSendingContext context)     { return Helper.TaskComplete; }
        public virtual Task OnSent(HttpSentContext context)           { return Helper.TaskComplete; }
        public virtual Task OnException(HttpExceptionContext context) { return Helper.TaskComplete; }
    }

    [ExcludeFromCodeCoverage]
    public abstract class HttpCallHandler<TResult, TContent, TError> : IHttpCallHandler<TResult, TContent, TError>
    {
        private bool _enabled = true;

        public virtual bool Enabled { get { return _enabled; } set { _enabled = value; } }

        public virtual HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            return HttpCallHandlerPriority.Default;
        }

        public virtual Task OnSending(HttpSendingContext<TResult, TContent, TError> context)     { return Helper.TaskComplete; }
        public virtual Task OnSent(HttpSentContext<TResult, TContent, TError> context)           { return Helper.TaskComplete; }
        public virtual Task OnResult(HttpResultContext<TResult, TContent, TError> context)         { return Helper.TaskComplete; }
        public virtual Task OnError(HttpErrorContext<TResult, TContent, TError> context)         { return Helper.TaskComplete; }
        public virtual Task OnException(HttpExceptionContext<TResult, TContent, TError> context) { return Helper.TaskComplete; }
    }

}