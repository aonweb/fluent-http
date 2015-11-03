using System;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheValidator: ICacheMetadata
    {
        CacheHandlerRegister HandlerRegister { get; }
        Action<CacheEntry> ResultInspector { get; }
        Func<ICacheContext, bool> RequestValidator { get; }
        Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; }
        Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; }
        Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; }
    }
}