using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public abstract class CacheContextBase
    {
        private readonly CacheSettings _settings;
        private readonly IHttpCallContext _callContext;

        protected CacheContextBase(
            CacheSettings settings,
            IHttpCallContext callContext,
            HttpRequestMessage request)
        {
            _settings = settings;
            _callContext = callContext;
            Request = request;

            //TODO: canonical url stuff?
            Uri = request.RequestUri;
            Key = Helper.BuildKey(_settings.ResultType, Uri, request.Headers, settings.GetVaryByHeaders(Uri));

        }

        public IDictionary Items { get { return _callContext.Items; } }

        public HttpRequestMessage Request { get; private set; }
        public string Key { get; private set; }

        public Uri Uri { get; private set; }

        public ResponseValidationResult ValidationResult { get; set; }

        public abstract ResponseInfo ResponseInfo { get; }

        public bool Enabled { get { return _settings.Enabled; } }
        public Type ResultType { get { return _settings.ResultType; } }
        public ISet<HttpMethod> CacheableMethods { get { return _settings.CacheableMethods; } }
        public ISet<HttpStatusCode> CacheableStatusCodes { get { return _settings.CacheableStatusCodes; } }
        public TimeSpan DefaultExpiration { get { return _settings.DefaultExpiration; } }
        public IEnumerable<string> DefaultVaryByHeaders { get { return _settings.DefaultVaryByHeaders; } }
        public bool MustRevalidateByDefault { get { return _settings.MustRevalidateByDefault; } }
        public IEnumerable<Uri> DependentUris { get { return _settings.DependentUris; } }


    }
}