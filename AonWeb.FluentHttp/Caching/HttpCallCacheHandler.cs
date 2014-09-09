using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.Handlers;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public class HttpCallCacheHandler : CacheHandlerBase, IHttpCallHandler
    {
        public HttpCallCacheHandler() { }

        protected HttpCallCacheHandler(CacheSettings settings)
            : base(settings) { }

        #region Configuration Methods

        public HttpCallCacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public HttpCallCacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public HttpCallCacheHandler WithDependentUri(Uri uri)
        {
            if (uri == null)
                return this;

            uri = uri.NormalizeUri();

            if (uri != null && !Settings.DependentUris.Contains(uri))
                Settings.DependentUris.Add(uri);

            return this;
        }

        #endregion

        #region IHttpCallHandler Implementation

        public async Task OnSending(HttpSendingContext context)
        {
            Settings.CacheResultConfiguration = cacheResult => ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Request;

            await TryGetFromCache(context);
        }

        public async Task OnSent(HttpSentContext context)
        {

            Settings.CacheResultConfiguration = cacheResult => ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Result.RequestMessage;

            await TryGetRevalidatedResult(context, context.Result);

            await TryCacheResult(context, context.Result, context.Result);
        }

        #endregion

        #region Unimplemented IHttpCallHandler Methods

        // TODO: invalidate caches for uri on exception?
        public Task OnException(HttpExceptionContext context) { return Task.Delay(0); }

        #endregion
    }
}
