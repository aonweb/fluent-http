using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Gets the <see cref="CacheEntry"/> specified by the <paramref name="context"/> or an Empty <see cref="CacheEntry"/> if none is found.
        /// </summary>
        /// <param name="context">The current cache context</param>
        /// <param name="token"></param>
        /// <returns>The specified <see cref="CacheEntry"/> or an Empty <see cref="CacheEntry"/> </returns>
        Task<CacheEntry> Get(ICacheContext context);

        /// <summary>
        /// Adds the <paramref name="newCacheEntry"/> or updated the expiration of existing <see cref="CacheEntry"/> specified by the <paramref name="context"/> 
        /// </summary>
        /// <param name="context">The current cache context</param>
        /// <param name="newCacheEntry">The <see cref="CacheEntry"/> to add or update</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task Put(ICacheContext context, CacheEntry newCacheEntry);

        /// <summary>
        /// Removes the <see cref="CacheEntry"/> specified by the <paramref name="context"/> and all related <see cref="CacheEntry"/> from the cache.
        /// </summary>
        /// <param name="context">The current cache context</param>
        /// <param name="additionalRelatedUris">Any additional uris to remove</param>
        /// <returns>A list of <see cref="Uri"/> removed from the cache.</returns>
        IEnumerable<Uri> Remove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris);

        /// <summary>
        /// Removes the <see cref="CacheEntry"/> with the specified <paramref name="uri"/> and all related uris from the cache.
        /// </summary>
        /// <param name="uri">The uri of the item to remove.</param>
        /// <returns>A list of <see cref="Uri"/> removed from the cache.</returns>
        IEnumerable<Uri> Remove(Uri uri);

        /// <summary>
        /// Clears all the items from the cache
        /// </summary>
        void Clear();
    }
}