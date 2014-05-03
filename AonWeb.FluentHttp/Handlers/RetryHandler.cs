using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public class RetryHandler : HttpCallHandler
    {

        private const int DefaultMaxAutoRetries = 2;
        private const int DefaultRetryAfter = 100;
        private const int DefaultMaxRetryAfter = 5000;

        // TODO: allow this to be configurable?
        private static readonly HashSet<HttpStatusCode> RetryStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.ServiceUnavailable };

        public RetryHandler()
        {
            AllowAutoRetry = true;
            MaxAutoRetries = DefaultMaxAutoRetries;
            RetryAfter = DefaultRetryAfter;
            MaxRetryAfter = DefaultMaxRetryAfter;
        }

        private bool AllowAutoRetry { get; set; }
        private int MaxAutoRetries { get; set; }
        private int RetryAfter { get; set; }
        private int MaxRetryAfter { get; set; }
        private Action<HttpRetryContext> OnRetry { get; set; }

        public RetryHandler WithAutoRetry()
        {
            return WithAutoRetry(-1, -1);
        }

        public RetryHandler WithAutoRetry(int maxAutoRetries, int retryAfter)
        {
            AllowAutoRetry = true;

            if (maxAutoRetries >= 0)
                MaxAutoRetries = maxAutoRetries;

            if (retryAfter >= 0)
                RetryAfter = retryAfter;

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
            if (AllowAutoRetry && ShouldRetry(context.Response))
            {
                var uri = context.Uri;

                var retryCount = context.Items["RetryCount"].As<int?>().GetValueOrDefault();

                if (retryCount > MaxAutoRetries)
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

                
                if (ctx.RetryAfter > 0)
                    await Task.Delay(ctx.RetryAfter, context.TokenSource.Token);

                context.Items["RetryCount"] = retryCount + 1;
                context.Response = await context.Builder.ResultAsync();
            }
        }

        private static bool ShouldRetry(HttpResponseMessage response)
        {
            return RetryStatusCodes.Contains(response.StatusCode);
        }

        private int? GetRetryAfter(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.ServiceUnavailable)
                return RetryAfter;

            var retryAfterHeader = response.Headers.RetryAfter;

            if (retryAfterHeader != null)
            {
                var tempMs = 0;
                if (retryAfterHeader.Date.HasValue)
                    tempMs = (int)(retryAfterHeader.Date.Value - DateTime.UtcNow).TotalMilliseconds;
                else if (retryAfterHeader.Delta.HasValue)
                    tempMs = (int)retryAfterHeader.Delta.Value.TotalMilliseconds;

                if (tempMs > 0 && tempMs < MaxRetryAfter)
                    return tempMs;
            }

            return null;
        } 
    }
}