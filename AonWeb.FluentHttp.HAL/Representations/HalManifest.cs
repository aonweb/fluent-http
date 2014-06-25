using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HalManifest : HalResource, IHalManifest
    {
        public virtual TimeSpan? Duration {  get { return null; } }

        public virtual IEnumerable<Uri> DependentUris { get { yield break; } }
    }

    public abstract class HalManifest<TLinks> : HalResource<TLinks>, IHalManifest
        where TLinks: HyperMediaLinks, new()
    {
        public virtual TimeSpan? Duration { get { return null; } }

        public virtual IEnumerable<Uri> DependentUris { get { yield break; } }
    }
}