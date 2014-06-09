using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheContext
    {
        private readonly CacheSettings _settings;
        private readonly IHttpCallContext _callContext;

        public CacheContext(CacheSettings settings, IHttpCallContext callContext, HttpRequestMessage request)
        {
            _settings = settings;

            _callContext = callContext;

            Request = request;

            //TODO: canonical url stuff?
            Uri = request.RequestUri;
            Key = Helper.BuildKey(ResultType, Uri, request.Headers, settings.GetVaryByHeaders(Uri));
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

        public IVaryByStore VaryByStore { get { return _settings.VaryByStore; } }
        public IHttpCacheStore CacheStore { get { return _settings.CacheStore; } }

        public IDictionary Items { get { return _callContext.Items; } }

        public HttpRequestMessage Request { get; private set; }
        public string Key { get; private set; }

        public Uri Uri { get; private set; }

        public ResponseValidationResult ValidationResult { get; set; }

        public bool Enabled { get { return _settings.Enabled; } }
        public Type ResultType { get { return _callContext.ResultType; } }
        public ISet<HttpMethod> CacheableMethods { get { return _settings.CacheableMethods; } }
        public ISet<HttpStatusCode> CacheableStatusCodes { get { return _settings.CacheableStatusCodes; } }
        public TimeSpan DefaultExpiration { get { return _settings.DefaultExpiration; } }
        public IEnumerable<string> DefaultVaryByHeaders { get { return _settings.DefaultVaryByHeaders; } }
        public bool MustRevalidateByDefault { get { return _settings.MustRevalidateByDefault; } }
        public IEnumerable<Uri> DependentUris { get { return _settings.DependentUris; } }
    }
}