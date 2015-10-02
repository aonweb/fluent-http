using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheContext : IContextWithSettings<ICacheSettings>
    {
        bool Enabled { get; }
        ISet<Uri> DependentUris { get; }
        TimeSpan? CacheDuration { get; }
        HttpRequestMessage Request { get; }
        CacheKey CacheKey { get; }
        Uri Uri { get; }
        CacheResult Result { get; set; }
        bool MustRevalidateByDefault { get; }
        TimeSpan? DefaultDurationForCacheableResults { get; }
        ISet<string> DefaultVaryByHeaders { get; }
        Action<CacheResult> ResultInspector { get; }
        Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator { get; }
        Func<ICacheContext, bool> CacheValidator { get; }
        Func<ICacheContext, ResponseInfo, bool> RevalidateValidator { get; }
        Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator { get; }
        CacheHandlerRegister Handler { get; }
        ISet<HttpMethod> CacheableHttpMethods { get; }
        ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        ResponseValidationResult ValidationResult { get; set; }
        bool SuppressTypeMismatchExceptions { get; }
        IHandlerContext GetHandlerContext();
    }
}