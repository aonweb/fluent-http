using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp.Xamarin
{
    public class SqlLiteCacheProvider: ICacheProvider
    {
        public Task<CacheEntry> Get(ICacheContext context)
        {
            throw new NotImplementedException();
        }

        public Task Put(ICacheContext context, CacheEntry newCacheEntry)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> Remove(ICacheContext context, IEnumerable<Uri> additionalRelatedUris)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Uri> Remove(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}