using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.HAL.Representations
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