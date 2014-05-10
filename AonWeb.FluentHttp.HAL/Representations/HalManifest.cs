using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HalManifest : HalResource, ICacheableHttpResult
    {
        public TimeSpan? Duration
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<string> DependentUris
        {
            get
            {
                yield break;
            }
        }
    }
}