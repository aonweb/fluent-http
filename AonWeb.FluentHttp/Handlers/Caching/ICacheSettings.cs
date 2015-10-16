using System;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheSettings: ICacheMetadata
    {
        new bool Enabled { get; set; }
        new TimeSpan? CacheDuration { get; set; }
        new TimeSpan? DefaultDurationForCacheableResults { get; set; }
        CacheHandlerRegister Handler { get; }
        new bool MustRevalidate { get; set; }
        Action<CacheResult> ResultInspector { get; set; }
        Func<ICacheContext, bool> CacheValidator { get; set; }
        Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; set; }
        Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; set; }
        Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; set; }
        ICacheKeyBuilder CacheKeyBuilder { get; set; }
    }
}