using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp.Caching
{
    public interface IHttpCacheStore
    {
        Task<CacheResult> GetCachedResult(ICacheContext context);

        Task AddOrUpdate(ICacheContext context);

        IEnumerable<Uri> TryRemove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris);

        void Clear();

        IEnumerable<Uri> RemoveItem(Uri uri);
    }
}