using System.Collections.Generic;

namespace AonWeb.FluentHttp.Caching
{
    public class UriCacheInfo
    {
        public UriCacheInfo()
        {
            CacheKeys = new HashSet<CacheKey>();
        }

        public ISet<CacheKey> CacheKeys { get; }
    }
}