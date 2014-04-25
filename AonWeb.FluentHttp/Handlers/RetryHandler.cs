using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.Fluent.Http.Handlers
{
    public class RetryHandler : IRetryHandler
    {

        private readonly RetrySettings _settings;

        public RetryHandler()
            : this(new RetrySettings()) { }

        internal RetryHandler(RetrySettings settings)
        {
            _settings = settings;
        }

        public IRetryHandler WithAutoRetry()
        {
            return WithAutoRetry(-1, -1);
        }

        public IRetryHandler WithAutoRetry(int maxAutoRetries, int retryAfter)
        {
            _settings.AllowAutoRetry = true;

            if (maxAutoRetries >= 0)
                _settings.MaxAutoRetries = maxAutoRetries;

            if (retryAfter >= 0)
                _settings.RetryAfter = retryAfter;

            return this;
        }

        public IRetryHandler WithHandler(Action<HttpRetryContext> handler)
        {
            _settings.RetryHandler = Utils.MergeAction(_settings.RetryHandler, handler);

            return this;
        }

        public HttpRetryContext HandleRetry(HttpCallBuilder builder, HttpResponseMessage response, int retryCount = 0)
        {
            // TODO: probably need to implement the circuit breaker pattern here.

            if (_settings.AllowAutoRetry && ShouldRetry(response))
            {
                var uri = builder.Settings.Uri;

                if (retryCount > _settings.MaxAutoRetries)
                    return null;

                var retryAfter = GetRetryAfter(response);

                var ctx = new HttpRetryContext
                {
                    StatusCode = response.StatusCode,
                    RequestMessage = response.RequestMessage,
                    Uri = uri,
                    ShouldRetry = retryAfter.HasValue,
                    RetryAfter = retryAfter ?? _settings.RetryAfter
                };

                if (_settings.RetryHandler != null)
                    _settings.RetryHandler(ctx);

                if (!ctx.ShouldRetry)
                    return null;

                return ctx;
            }

            return null;
        }

        private int? GetRetryAfter(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.ServiceUnavailable)
                return _settings.RetryAfter;

            var retryAfterHeader = response.Headers.RetryAfter;

            if (retryAfterHeader != null)
            {
                int tempMs = 0;
                if (retryAfterHeader.Date.HasValue)
                {
                    tempMs = (int)(retryAfterHeader.Date.Value - DateTime.UtcNow).TotalMilliseconds;
                }
                else if (retryAfterHeader.Delta.HasValue)
                {
                    tempMs = (int)retryAfterHeader.Delta.Value.TotalMilliseconds;
                }

                if (tempMs > 0 && tempMs < _settings.MaxRetryAfter)
                    return tempMs;
            }

            return null;
        }

        private bool ShouldRetry(HttpResponseMessage response)
        {
            return _settings.RetryStatusCodes.Contains(response.StatusCode);
        }
    }
}