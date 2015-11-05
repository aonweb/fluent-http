using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheMetadata
    {
        ISet<Uri> DependentUris { get; }
        TimeSpan? CacheDuration { get;  }
        ISet<HttpMethod> CacheableHttpMethods { get; }
        ISet<HttpStatusCode> CacheableHttpStatusCodes { get; }
        ISet<string> DefaultVaryByHeaders { get; }
        bool SuppressTypeMismatchExceptions { get; }
        TimeSpan? DefaultDurationForCacheableResults { get; }
        bool MustRevalidate { get; }
    }
}