using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public class RetryHandler : HttpCallHandler
    {
        public RetryHandler()
        {
            Enabled = HttpCallBuilderDefaults.AutoRetryEnabled;
            MaxAutoRetries = HttpCallBuilderDefaults.DefaultMaxAutoRetries;
            RetryAfter = HttpCallBuilderDefaults.DefaultRetryAfter;
            MaxRetryAfter = HttpCallBuilderDefaults.DefaultMaxRetryAfter;

            RetryStatusCodes = new HashSet<HttpStatusCode>(HttpCallBuilderDefaults.DefaultRetryStatusCodes);
            RetryValidator = ShouldRetry;
        }

        private int MaxAutoRetries { get; set; }
        private TimeSpan RetryAfter { get; set; }
        private TimeSpan MaxRetryAfter { get; set; }
        private ISet<HttpStatusCode> RetryStatusCodes { get; set; }
        private Action<HttpRetryContext> OnRetry { get; set; }
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
                RetryAfter = retryAfter;

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
                throw new ArgumentNullException("validator");

            RetryValidator = validator;

            return this;
        }

        public RetryHandler WithCallback(Action<HttpRetryContext> callback)
        {
            OnRetry = Helper.MergeAction(OnRetry, callback);

            return this;
        }

        public override HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            if (type == HttpCallHandlerType.Sent)
                return HttpCallHandlerPriority.High;

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

            var retryAfter = GetRetryAfter(context.Response);

            var ctx = new HttpRetryContext
            {
                StatusCode = context.Response.StatusCode,
                RequestMessage = context.Response.RequestMessage,
                Uri = uri,
                ShouldRetry = retryAfter.HasValue,
                RetryAfter = retryAfter ?? RetryAfter,
                CurrentRetryCount = retryCount
            };

            if (OnRetry != null)
                OnRetry(ctx);

            if (!ctx.ShouldRetry)
                return;

                
            if (ctx.RetryAfter > TimeSpan.Zero)
                await Task.Delay(ctx.RetryAfter, context.TokenSource.Token);

            context.Items["RetryCount"] = retryCount + 1;

            // dispose of previous response
            Helper.DisposeResponse(context.Response);

            context.Response = await context.Builder.RecursiveResultAsync();
        }

        private bool ShouldRetry(HttpSentContext context)
        {
            return RetryStatusCodes.Contains(context.Response.StatusCode);
        }

        private TimeSpan? GetRetryAfter(HttpResponseMessage response)
        {
            var retryAfterHeader = response.Headers.RetryAfter;

            if (retryAfterHeader == null) 
                return RetryAfter;

            
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