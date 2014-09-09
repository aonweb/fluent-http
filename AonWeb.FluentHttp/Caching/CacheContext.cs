using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;
using JetBrains.Annotations;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheContext
    {
        private readonly CacheSettings _settings;
        private readonly IHttpCallHandlerContext _callContext;

        public CacheContext(CacheContext context)
            : this(context._settings, context._callContext) { }

        public CacheContext(CacheSettings settings, IHttpCallHandlerContext callContext)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            if (callContext == null)
                throw new ArgumentNullException("callContext");

            _settings = settings;

            _callContext = callContext;

            if (callContext.Request != null)
            {
                Request = callContext.Request;

                //TODO: canonical url stuff?
                Uri = Request.RequestUri;
                Key = Helper.BuildKey(ResultType, Uri, callContext.Request.Headers, settings.GetVaryByHeaders(Uri));
            }
            else
            {
                Key = string.Empty;
            }
        }

        public CacheResult CacheResult { get; set; }

        public ResponseInfo ResponseInfo
        {
            get { return CacheResult != null ? CacheResult.ResponseInfo : null; }
        }

        public object Result
        {
            get { return CacheResult != null ? CacheResult.Result : Helper.GetDefaultValueForType(ResultType); }
        }

        public bool ResultFound
        {
            get { return CacheResult != null && CacheResult.Found; }
        }

        public Func<CacheContext, bool> CacheValidator { get { return _settings.CacheValidator; } }
        public Func<CacheContext, bool> RevalidateValidator { get { return _settings.RevalidateValidator; } }
        public Action<CacheResult> ResultInspector { get { return _settings.CacheResultConfiguration; } }
        public Func<CacheContext, ResponseValidationResult> ResponseValidator { get { return _settings.ResponseValidator; } }
        public Func<CacheContext, bool> AllowStaleResultValidator { get { return _settings.AllowStaleResultValidator; } }
        public CacheHandlerRegister Handler { get { return _settings.Handler; } }
        public IVaryByStore VaryByStore { get { return _settings.VaryByStore; } }
        public IHttpCacheStore CacheStore { get { return _settings.CacheStore; } }
        public IDictionary Items { get { return _callContext.Items; } }
        
        public ResponseValidationResult ValidationResult { get; set; }
        public bool Enabled { get { return _settings.Enabled; } }
        public Type ResultType { get { return _callContext.ResultType; } }
        public ISet<HttpMethod> CacheableMethods { get { return _settings.CacheableMethods; } }
        public ISet<HttpStatusCode> CacheableStatusCodes { get { return _settings.CacheableStatusCodes; } }
        public TimeSpan DefaultExpiration { get { return _settings.DefaultExpiration; } }
        public IEnumerable<string> DefaultVaryByHeaders { get { return _settings.DefaultVaryByHeaders; } }
        public bool MustRevalidateByDefault { get { return _settings.MustRevalidateByDefault; } }
        public IEnumerable<Uri> DependentUris { get { return _settings.DependentUris; } }

        [CanBeNull]
        public HttpRequestMessage Request { get; private set; }

        public string Key { get; private set; }

        [CanBeNull]
        public Uri Uri { get; private set; }
    }
}