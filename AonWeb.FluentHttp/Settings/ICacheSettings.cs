using System;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Settings
{
    public interface ICacheSettings: ICacheValidator
    {
        new bool Enabled { get; set; }
        new TimeSpan? CacheDuration { get; set; }
        new TimeSpan? DefaultDurationForCacheableResults { get; set; }
        new bool MustRevalidate { get; set; }
        new Action<CacheEntry> ResultInspector { get; set; }
        new Func<ICacheContext, bool> RequestValidator { get; set; }
        new Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; set; }
        new Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; set; }
        new Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; set; }
    }
}