using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheContext: ICacheContext
    {
        private readonly ICacheSettings _settings;
        private readonly IHandlerContext _handlerContext;
        private readonly Lazy<CacheKey> _cacheKey;

        public CacheContext(ICacheContext context)
            : this(context.GetSettings(), context.GetHandlerContext()) { }

        public CacheContext(ICacheSettings settings, IHandlerContext handlerContext)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (handlerContext == null)
                throw new ArgumentNullException(nameof(handlerContext));

            _settings = settings;

            _handlerContext = handlerContext;

            Request = handlerContext.Request;
            Uri = Request?.RequestUri.NormalizeUri();
            _cacheKey = new Lazy<CacheKey>(() => _settings.CacheKeyBuilder.BuildKey(this));
        }

        public CacheResult Result { get; set; }

        public ResponseInfo ResponseInfo => Result.ResponseInfo;
        public CacheKey CacheKey => _cacheKey.Value;
        public Func<ICacheContext, bool> CacheValidator => _settings.CacheValidator;
        public Func<ICacheContext, ResponseInfo, bool> RevalidateValidator => _settings.RevalidateValidator;
        public Action<CacheResult> ResultInspector => _settings.ResultInspector;
        public Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator => _settings.ResponseValidator;
        public Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator => _settings.AllowStaleResultValidator;
        public CacheHandlerRegister Handler => _settings.Handler;

        public IDictionary Items => _handlerContext.Items;

        public ResponseValidationResult ValidationResult { get; set; }
        public bool Enabled => _settings.Enabled;
       
        public Type ResultType => _handlerContext.ResultType;
        public ISet<HttpMethod> CacheableHttpMethods => _settings.CacheableHttpMethods;
        public ISet<HttpStatusCode> CacheableHttpStatusCodes => _settings.CacheableHttpStatusCodes;
        public TimeSpan? DefaultDurationForCacheableResults => _settings.DefaultDurationForCacheableResults;
        public ISet<string> DefaultVaryByHeaders => _settings.DefaultVaryByHeaders;
        public bool MustRevalidateByDefault => _settings.MustRevalidateByDefault;
        public ISet<Uri> DependentUris => _settings.DependentUris;
        public TimeSpan? CacheDuration => _settings.CacheDuration;
        public bool SuppressTypeMismatchExceptions => _settings.SuppressTypeMismatchExceptions;

        public HttpRequestMessage Request { get; }

        public Uri Uri { get; }

        public ICacheSettings GetSettings()
        {
            return _settings;
        }

        public IHandlerContext GetHandlerContext()
        {
            return _handlerContext;
        }
    }
}