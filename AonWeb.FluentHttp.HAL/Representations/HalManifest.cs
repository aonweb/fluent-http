using System;
using System.Collections.Generic;
using AonWeb.Fluent.Http.Cache;

namespace AonWeb.Fluent.HAL.Representations
{
    public class HalManifest : HalResource, ICachedHttpResult
    {
        public TimeSpan? Duration
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<string> AssociatedUris
        {
            get
            {
                yield break;
            }
        }
    }
}