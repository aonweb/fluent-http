using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Handlers.Caching
{
    public class CacheExpiringContext : CacheHandlerContext
    {
        private readonly ModifiableUriList _relatedUris;

        public CacheExpiringContext(ICacheContext context,  RequestValidationResult reason)
            : base(context)
        {
            _relatedUris = new ModifiableUriList();
            Reason = reason;
        }

        public CacheExpiringContext(CacheExpiringContext context)
            : base(context)
        {
            _relatedUris = context._relatedUris;
        }

        public ICollection<Uri> RelatedUris
        {
            get { return _relatedUris.Value; }
            set { _relatedUris.Value = value; }
        }

        public RequestValidationResult Reason { get; }

        public override Modifiable GetHandlerResult()
        {
            return _relatedUris;
        }
    }
}