using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class TypedCacheHandler : CacheHandlerBase, IBoxedHttpCallHandler
    {
        public TypedCacheHandler() { }

        protected TypedCacheHandler(CacheSettings settings)
            : base(settings) { }

        #region Configuration Methods

        public TypedCacheHandler WithCaching(bool enabled = true)
        {
            Settings.Enabled = enabled;

            return this;
        }

        public TypedCacheHandler WithDependentUris(IEnumerable<Uri> uris)
        {
            if (uris == null)
                return this;

            foreach (var uri in uris)
                WithDependentUri(uri);

            return this;
        }

        public TypedCacheHandler WithDependentUri(Uri uri)
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

        public async Task OnSending(TypedHttpSendingContext<object, object> context)
        {
            var cacheContext = CreateCacheContext(context, context.Request);

            await TryGetFromCache(cacheContext);

            if (cacheContext.ResultFound)
                context.Result = cacheContext.Result;
        }

        public Task OnSent(TypedHttpSentContext<object> context)
        {
            var cacheContext = CreateCacheContext(context, context.Response);

            TryGetRevalidatedResult(cacheContext, context.Response);

            if (cacheContext.ResultFound)
                context.Result = cacheContext.Result;

            return Task.Delay(0);
        }

        public async Task OnResult(TypedHttpResultContext<object> context)
        {
            var cacheContext = CreateCacheContext(context, context.Response);
            
            await TryCacheResult(cacheContext, context.Result, context.Response);
        }

        #endregion

        #region Unimplemented IHttpCallHandler Methods

        // TODO: invalidate caches for uri on error or exception?
        public Task OnError(TypedHttpCallErrorContext<object> context) { return Task.Delay(0); }

        Task ITypedHttpCallHandler.OnSending<TResult, TContent>(TypedHttpSendingContext<TResult, TContent> context)
        {
            var boxedContext = context as TypedHttpSendingContext<object, object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpSendingContext<object, object>(context, context.Request, context.Content, context.HasContent);

            return OnSending(boxedContext);
        }

        Task ITypedHttpCallHandler.OnSent<TResult>(TypedHttpSentContext<TResult> context)
        {
            var boxedContext = context as TypedHttpSentContext<object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpSentContext<object>(context, context.Response);

            return OnSent(boxedContext);
        }

        Task ITypedHttpCallHandler.OnResult<TResult>(TypedHttpResultContext<TResult> context)
        {
            var boxedContext = context as TypedHttpResultContext<object>;

            if (boxedContext == null)
                boxedContext = new TypedHttpResultContext<object>(context, context.Response, context.Result);

            return OnResult(boxedContext);
        }

        Task ITypedHttpCallHandler.OnError<TError>(TypedHttpCallErrorContext<TError> context)
        {
            return Task.Delay(0);
        }

        public Task OnException(TypedHttpCallExceptionContext context) { return Task.Delay(0); }

        #endregion
    }
}