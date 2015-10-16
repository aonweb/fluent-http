using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class RetryHandler : HttpHandler
    {
        public RetryHandler()
        {
            Enabled = Defaults.Current.GetHandlerDefaults().AutoRetryEnabled;
            MaxAutoRetries = Defaults.Current.GetHandlerDefaults().MaxAutoRetries;
            DefaultRetryAfter = Defaults.Current.GetHandlerDefaults().DefaultRetryAfter;
            MaxRetryAfter = Defaults.Current.GetHandlerDefaults().MaxRetryAfter;

            RetryStatusCodes = new HashSet<HttpStatusCode>(Defaults.Current.GetHandlerDefaults().RetryStatusCodes);
            RetryValidator = ShouldRetry;
        }

        private int MaxAutoRetries { get; set; }
        private TimeSpan DefaultRetryAfter { get; set; }
        private TimeSpan MaxRetryAfter { get; }
        private ISet<HttpStatusCode> RetryStatusCodes { get; }
        private Action<RetryContext> OnRetry { get; set; }
        private Func<HttpSentContext, bool> RetryValidator { get; set; }

        public RetryHandler WithAutoRetry(bool enabled = true)
        {
            if (enabled)
                return WithAutoRetry(-1, TimeSpan.Zero);

            Enabled = false;

            return this;
        }

        public RetryHandler WithAutoRetry(int maxAutoRetries, TimeSpan retryAfter)
        {
            Enabled = true;

            if (maxAutoRetries >= 0)
                MaxAutoRetries = maxAutoRetries;

            if (retryAfter >= TimeSpan.Zero && retryAfter <= MaxRetryAfter)
                DefaultRetryAfter = retryAfter;

            return this;
        }

        public RetryHandler WithRetryStatusCode(HttpStatusCode statusCode)
        {
            if (!RetryStatusCodes.Contains(statusCode))
                RetryStatusCodes.Add(statusCode);

            return this;
        }

        public RetryHandler WithRetryValidator(Func<HttpSentContext, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            RetryValidator = validator;

            return this;
        }

        public RetryHandler WithCallback(Action<RetryContext> callback)
        {
            OnRetry = (Action<RetryContext>)Delegate.Combine(OnRetry, callback);

            return this;
        }

        public override HandlerPriority GetPriority(HandlerType type)
        {
            if (type == HandlerType.Sent)
                return HandlerPriority.High;

            return base.GetPriority(type);
        }

        public override async Task OnSent(HttpSentContext context)
        {
            if (!RetryValidator(context)) 
                return;

            var uri = context.Uri;

            var retryCount = context.Items["RetryCount"].As<int?>().GetValueOrDefault();

            if (retryCount >= MaxAutoRetries)
                return;

            var retryAfter = GetRetryAfter(context.Result);

            var ctx = new RetryContext
            {
                StatusCode = context.Result.StatusCode,
                RequestMessage = context.Result.RequestMessage,
                Uri = uri,
                ShouldRetry = retryAfter.HasValue,
                RetryAfter = retryAfter ?? DefaultRetryAfter,
                CurrentRetryCount = retryCount
            };

            OnRetry?.Invoke(ctx);

            if (!ctx.ShouldRetry)
                return;
                
            if (ctx.RetryAfter > TimeSpan.Zero)
                await Task.Delay(ctx.RetryAfter, context.Token);

            context.Items["RetryCount"] = retryCount + 1;

            // dispose of previous response
            ObjectHelpers.Dispose(context.Result);

            context.Result = await context.Builder.RecursiveResultAsync(context.Token);
        }

        private bool ShouldRetry(HttpSentContext context)
        {
            return RetryStatusCodes.Contains(context.Result.StatusCode);
        }

        private TimeSpan? GetRetryAfter(HttpResponseMessage response)
        {
            var retryAfterHeader = response.Headers.RetryAfter;

            if (retryAfterHeader == null) 
                return DefaultRetryAfter;

            
            var retryAfter = TimeSpan.Zero;
            if (retryAfterHeader.Date.HasValue)
                retryAfter = retryAfterHeader.Date.Value - DateTime.UtcNow;
            else if (retryAfterHeader.Delta.HasValue)
                retryAfter = retryAfterHeader.Delta.Value;

            if (retryAfter > TimeSpan.Zero && retryAfter <= MaxRetryAfter)
                return retryAfter;

            return null;

        } 
    }
}