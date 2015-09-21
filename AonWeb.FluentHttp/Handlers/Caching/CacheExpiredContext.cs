using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheExpiredContext : CacheHandlerContext
    {
        public CacheExpiredContext(ICacheContext context, IReadOnlyCollection<Uri> expiredUris)
            : base(context)
        {
            ExpiredUris = expiredUris;
        }

        public IReadOnlyCollection<Uri> ExpiredUris { get; set; }

        public CacheExpiredContext(CacheExpiredContext context)
            : base(context)
        {
            ExpiredUris = context.ExpiredUris;
        }
    }
}