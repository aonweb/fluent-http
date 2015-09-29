using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class HttpCacheHandler : CacheHandlerCore, IHandler
    {
        public HttpCacheHandler() { }

        protected HttpCacheHandler(CacheSettings settings)
            : base(settings) { }

        #region Configuration Methods

        public HttpCacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public HttpCacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public HttpCacheHandler WithDependentUri(Uri uri)
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

        public async Task OnSending(SendingContext context)
        {
            Settings.ResultInspector = cacheResult =>
            {
                ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Request;
            };

            await TryGetFromCache(context);
        }

        public async Task OnSent(SentContext context)
        {

            Settings.ResultInspector = cacheResult => ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Result.RequestMessage;

            await TryGetRevalidatedResult(context, context.Result);

            await TryCacheResult(context, context.Result, context.Result);
        }

        #endregion

        #region Unimplemented IHttpCallHandler Methods

        // TODO: invalidate caches for uri on exception?
        public Task OnException(ExceptionContext context) { return Task.Delay(0); }

        #endregion
    }
}
