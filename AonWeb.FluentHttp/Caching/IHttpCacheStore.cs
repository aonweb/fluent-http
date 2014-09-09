using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Caching
{
    public interface IHttpCacheStore
    {
        Task<CacheResult> GetCachedResult(CacheContext context);

        Task AddOrUpdate(CacheContext context);

        IList<string> TryRemove(CacheContext context, IEnumerable<Uri> additionalRelatedUris);

        void Clear();

        IEnumerable<string> RemoveItem(Uri uri);
    }
}