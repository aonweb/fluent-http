using System;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheContext<TResult> : CacheContextBase
    {
        private readonly CacheSettings<TResult> _settings;

        public CacheContext(CacheSettings<TResult> settings, IHttpCallContext callContext, HttpRequestMessage request)
            : base(settings, callContext, request)
        {
            _settings = settings;
        }

        public CacheResult<TResult> CacheResult { get; set; }

        public override ResponseInfo ResponseInfo
        {
            get { return CacheResult != null ? CacheResult.ResponseInfo : null; }
        }

        public TResult Result
        {
            get { return CacheResult != null ? CacheResult.Result : default(TResult); }
        }

        public bool ResultFound
        {
            get { return CacheResult != null && CacheResult.Found; }
        }

        public Func<CacheContext<TResult>, bool> CacheValidator { get { return _settings.CacheValidator; } }
        public Func<CacheContext<TResult>, bool> RevalidateValidator { get { return _settings.RevalidateValidator; } }
        public Action<CacheResult<TResult>> ResultInspector { get { return _settings.CacheResultConfiguration; } }
        public Func<CacheContext<TResult>, ResponseValidationResult> ResponseValidator { get { return _settings.ResponseValidator; } }
        public Func<CacheContext<TResult>, bool> AllowStaleResultValidator { get { return _settings.AllowStaleResultValidator; } }

        public IVaryByStore VaryByStore { get { return _settings.VaryByStore; } }
        public IHttpCacheStore CacheStore { get { return _settings.CacheStore; } }
    }
}