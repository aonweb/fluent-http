using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheSettings
    {
        bool Enabled { get; set; }
        ISet<Uri> DependentUris { get; }
        TimeSpan? CacheDuration { get; set; }
        CacheHandlerRegister Handler { get; }
        ISet<HttpMethod> CacheableHttpMethods { get; }
        ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        ISet<string> DefaultVaryByHeaders { get; }
        bool SuppressTypeMismatchExceptions { get; }
        TimeSpan? DefaultDurationForCacheableResults { get; set; }
        bool MustRevalidateByDefault { get; set; }
        Action<CacheResult> ResultInspector { get; set; }
        Func<ICacheContext, bool> CacheValidator { get; set; }
        Func<ICacheContext, ResponseInfo, ResponseValidationResult> ResponseValidator { get; set; }
        Func<ICacheContext, ResponseInfo, bool> RevalidateValidator { get; set; }
        Func<ICacheContext, ResponseInfo, bool> AllowStaleResultValidator { get; set; }
        ICacheKeyBuilder CacheKeyBuilder { get; set; }
    }
}