using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HalManifest : HalResource, ICacheableHttpResult
    {
        public TimeSpan? Duration {  get { return null; } }

        public IEnumerable<Uri> DependentUris { get { yield break; } }
    }

    public abstract class HalManifest<TLinks> : HalResource<TLinks>, ICacheableHttpResult
        where TLinks: HyperMediaLinks, new()
    {
        public TimeSpan? Duration {  get { return null; } }

        public IEnumerable<Uri> DependentUris { get { yield break; } }
    }
}