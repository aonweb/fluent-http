using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public abstract class CacheHandlerContext : IHandlerContext
    {
        private readonly ICacheContext _cacheContext;

        protected CacheHandlerContext(ICacheContext context)
        {
            _cacheContext = context;
        }

        protected CacheHandlerContext(CacheHandlerContext context)
        {
            _cacheContext = context._cacheContext;
        }

        public IDictionary Items => _cacheContext.Items;
        public ResponseValidationResult ValidationResult => _cacheContext.ValidationResult;
        public bool Enabled => _cacheContext.Enabled;
        
        public Type ResultType => _cacheContext.ResultType;
        public IEnumerable<Uri> DependentUris => _cacheContext.DependentUris;
        public HttpRequestMessage Request => _cacheContext.Request;
        public Uri Uri => _cacheContext.Uri;
        public bool SuppressTypeMismatchExceptions => _cacheContext.SuppressTypeMismatchExceptions;

        public virtual Modifiable GetHandlerResult()
        {
            return new Modifiable();
        }
    }
}