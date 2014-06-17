using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.Handlers;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheHandler : CacheHandlerBase, IHttpCallHandler
    {
        public CacheHandler() { }

        protected CacheHandler(CacheSettings settings)
            : base(settings) { }

        #region Configuration Methods

        public CacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public CacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public CacheHandler WithDependentUri(Uri uri)
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

            var cacheContext = CreateCacheContext(context, context.Request);

            await TryGetFromCache(cacheContext);

            if (cacheContext.ResultFound)
                context.Response = (HttpResponseMessage)cacheContext.Result;
        }

        public async Task OnSent(HttpSentContext context)
        {
            var cacheContext = CreateCacheContext(context, context.Response);

            Settings.CacheResultConfiguration = cacheResult => ((HttpResponseMessage)cacheResult.Result).RequestMessage = context.Response.RequestMessage;

            TryGetRevalidatedResult(cacheContext, context.Response);

            if (cacheContext.ResultFound)
                context.Response = (HttpResponseMessage)cacheContext.Result;

            await TryCacheResult(cacheContext, context.Response, context.Response);
        }

        #endregion

        #region Unimplemented IHttpCallHandler Methods

        // TODO: invalidate caches for uri on exception?
        public Task OnException(HttpExceptionContext context) { return Helper.TaskComplete; }

        #endregion
    }
}
