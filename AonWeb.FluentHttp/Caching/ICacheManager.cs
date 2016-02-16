using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheManager
    {
        /// <summary>
        /// Gets the <see cref="CacheEntry"/> specified by the <paramref name="context"/> or an Empty <see cref="CacheEntry"/> if none is found.
        /// </summary>
        /// <param name="context">The current cache context</param>
        /// <returns>The specified <see cref="CacheEntry"/> or an Empty <see cref="CacheEntry"/> </returns>
        Task<CacheEntry> Get(ICacheContext context);

        /// <summary>
        /// Puts the <paramref name="newCacheEntry"/> or updated the expiration of existing <see cref="CacheEntry"/> specified by the <paramref name="context"/> 
        /// </summary>
        /// <param name="context">The current cache context</param>
        /// <param name="newCacheEntry">The <see cref="CacheEntry"/> to add or update</param>
        /// <returns></returns>
        Task Put(ICacheContext context, CacheEntry newCacheEntry);

        /// <summary>
        /// Deletes the <see cref="CacheEntry"/> specified by the <paramref name="context"/> and all related <see cref="CacheEntry"/> from the cache.
        /// </summary>
        /// <param name="context">The current cache context</param>
        /// <param name="additionalDependentUris">Any additional uris to remove</param>
        /// <returns>A list of <see cref="Uri"/> removed from the cache.</returns>
        Task<IList<Uri>> Delete(ICacheContext context, IEnumerable<Uri> additionalDependentUris);

        /// <summary>
        /// Deletes the <see cref="CacheEntry"/> with the specified <paramref name="uri"/> and all related uris from the cache.
        /// </summary>
        /// <param name="uri">The uri of the item to remove.</param>
        /// <returns>A list of <see cref="Uri"/> removed from the cache.</returns>
        Task<IList<Uri>> Delete(Uri uri);

        /// <summary>
        /// Deletes all entries from the cache.
        /// </summary>
        /// <returns></returns>
        Task DeleteAll();
    }
}