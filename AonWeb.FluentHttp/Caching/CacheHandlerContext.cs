using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using AonWeb.FluentHttp.Handlers;
using JetBrains.Annotations;


namespace AonWeb.FluentHttp.Caching
{
    public abstract class CacheHandlerContext : IHandlerContext
    {
        private readonly CacheContext _cacheContext;

        protected CacheHandlerContext(CacheContext context)
        {
            _cacheContext = context;
        }

        protected CacheHandlerContext(CacheHandlerContext context)
        {
            _cacheContext = context._cacheContext;
        }

        public IDictionary Items { get { return _cacheContext.Items; } }
        public ResponseValidationResult ValidationResult { get { return _cacheContext.ValidationResult; } }
        public bool Enabled { get { return _cacheContext.Enabled; } }
        public Type ResultType { get { return _cacheContext.ResultType; } }
        public IEnumerable<Uri> DependentUris { get { return _cacheContext.DependentUris; } }

        [CanBeNull]
        public HttpRequestMessage Request { get { return _cacheContext.Request; } }

        [CanBeNull]
        public string Key { get { return _cacheContext.Key; } }

        [CanBeNull]
        public Uri Uri { get { return _cacheContext.Uri; } }

        public abstract ModifyTracker GetHandlerResult();
    }
}