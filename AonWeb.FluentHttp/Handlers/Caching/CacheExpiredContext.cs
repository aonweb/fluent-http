using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheExpiredContext : CacheHandlerContext
    {
        public CacheExpiredContext(ICacheContext context, RequestValidationResult reason, IReadOnlyCollection<Uri> expiredUris)
            : base(context)
        {
            ExpiredUris = expiredUris;
            Reason = reason;
        }

        public IReadOnlyCollection<Uri> ExpiredUris { get; }
        public RequestValidationResult Reason { get; }

        public CacheExpiredContext(CacheExpiredContext context)
            : base(context)
        {
            ExpiredUris = context.ExpiredUris;
        }
    }
}