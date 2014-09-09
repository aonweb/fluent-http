using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheExpiringContext : CacheHandlerContext
    {
        private readonly UriListModifyTracker _relatedUris;

        public CacheExpiringContext(CacheContext context)
            : base(context)
        {
            _relatedUris = new UriListModifyTracker();
        }

        public ICollection<Uri> RelatedUris
        {
            get { return _relatedUris.Value; }
            set { _relatedUris.Value = value; }
        }

        public override ModifyTracker GetHandlerResult()
        {
            return _relatedUris;
        }
    }
}