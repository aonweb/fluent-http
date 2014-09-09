using System.Collections.Generic;
using AonWeb.FluentHttp.Handlers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheExpiredContext : CacheHandlerContext
    {
        public CacheExpiredContext(CacheContext context, IEnumerable<string> relatedKeys)
            : base(context)
        {
            RelatedKeys = relatedKeys;
        }

        public IEnumerable<string> RelatedKeys { get; set; }

        public override ModifyTracker GetHandlerResult()
        {
            return new ModifyTracker();
        }
    }
}