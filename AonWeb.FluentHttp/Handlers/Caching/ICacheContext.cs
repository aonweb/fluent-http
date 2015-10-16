using System;
using System.Net.Http;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public interface ICacheContext : IContextWithSettings<ICacheSettings>, ICacheMetadata
    {
        HttpRequestMessage Request { get; }
        CacheKey CacheKey { get; }
        Uri Uri { get; }
        CacheResult Result { get; set; }
        CacheHandlerRegister Handler { get; }
        Action<CacheResult> ResultInspector { get; }
        Func<ICacheContext, IResponseMetadata, ResponseValidationResult> ResponseValidator { get; }
        Func<ICacheContext, bool> CacheValidator { get; }
        Func<ICacheContext, IResponseMetadata, bool> RevalidateValidator { get; }
        Func<ICacheContext, IResponseMetadata, bool> AllowStaleResultValidator { get; }
        ResponseValidationResult ValidationResult { get; set; }
        IHandlerContext GetHandlerContext();
    }
}