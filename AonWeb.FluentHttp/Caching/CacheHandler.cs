using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.Handlers;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{

    public class CacheHandler<TResult, TContent, TError> : CacheHandlerBase<TResult>, IHttpCallHandler<TResult, TContent, TError>
    {
        public CacheHandler() { }

        protected CacheHandler(CacheSettings<TResult> settings)
            : base(settings) { }

        #region Configuration Methods

        public CacheHandler<TResult, TContent, TError> WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public CacheHandler<TResult, TContent, TError> WithDependentUris(IEnumerable<Uri> uris)
        {
            foreach (var uri in uris)
            {
                WithDependentUri(uri);
            }

            return this;
        }

        public CacheHandler<TResult, TContent, TError> WithDependentUri(Uri uri)
        {
            uri = uri.NormalizeUri();
            if (uri != null && !Settings.DependentUris.Contains(uri))
                Settings.DependentUris.Add(uri);

            return this;
        }

        #endregion

        #region IHttpCallHandler Implementation

        public async Task OnSending(HttpSendingContext<TResult, TContent, TError> context)
        {
            var cacheContext = CreateCacheContext(context, context.Request);

            await TryGetFromCache(cacheContext);

            if (cacheContext.ResultFound)
                context.Result = cacheContext.Result;
        }

        public Task OnSent(HttpSentContext<TResult, TContent, TError> context)
        {
            var cacheContext = CreateCacheContext(context, context.Response);

            TryGetRevalidatedResult(cacheContext, context.Response);

            if (cacheContext.ResultFound)
                context.Result = cacheContext.Result;

            return Helper.TaskComplete;
        }

        public async Task OnResult(HttpResultContext<TResult, TContent, TError> context)
        {
            var cacheContext = CreateCacheContext(context, context.Response);
            
            await TryCacheResult(cacheContext, context.Result, context.Response);
        }

        #endregion

        #region Unimplemented IHttpCallHandler Methods

        // TODO: invalidate caches for uri on error or exception?
        public Task OnError(HttpErrorContext<TResult, TContent, TError> context) { return Helper.TaskComplete; }
        public Task OnException(HttpExceptionContext<TResult, TContent, TError> context) { return Helper.TaskComplete; }

        #endregion
    }

    public class CacheHandler : CacheHandlerBase<HttpResponseMessage>, IHttpCallHandler
    {
        public CacheHandler() { }

        protected CacheHandler(CacheSettings<HttpResponseMessage> settings)
            : base(settings) { }

        #region Configuration Methods

        public CacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public CacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            foreach (var uri in uris)
            {
                WithDependentUri(uri);
            }

            return this;
        }

        public CacheHandler WithDependentUri(Uri uri)
        {
            uri = uri.NormalizeUri();
            if (uri != null && !Settings.DependentUris.Contains(uri))
                Settings.DependentUris.Add(uri);

            return this;
        }

        #endregion

        #region IHttpCallHandler Implementation

        public async Task OnSending(HttpSendingContext context)
        {
            Settings.CacheResultConfiguration = cacheResult => cacheResult.Result.RequestMessage = context.Request;

            var cacheContext = CreateCacheContext(context, context.Request);

            await TryGetFromCache(cacheContext);

            if (cacheContext.ResultFound)
                context.Response = cacheContext.Result;
        }

        public async Task OnSent(HttpSentContext context)
        {
            var cacheContext = CreateCacheContext(context, context.Response);

            Settings.CacheResultConfiguration = cacheResult => cacheResult.Result.RequestMessage = context.Response.RequestMessage;

            TryGetRevalidatedResult(cacheContext, context.Response);

            if (cacheContext.ResultFound)
                context.Response = cacheContext.Result;

            await TryCacheResult(cacheContext, context.Response, context.Response);
        }

        #endregion

        #region Unimplemented IHttpCallHandler Methods

        // TODO: invalidate caches for uri on exception?
        public Task OnException(HttpExceptionContext context) { return Helper.TaskComplete; }

        #endregion
    }
}
