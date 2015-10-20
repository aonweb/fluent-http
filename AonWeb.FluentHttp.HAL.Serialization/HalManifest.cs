using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public class HalManifest : HalResource, IHalManifest
    {
        public virtual TimeSpan? Duration => null;

        public virtual IEnumerable<Uri> DependentUris { get { yield break; } }
    }

    public abstract class HalManifest<TLinks> : HalResource<TLinks>, IHalManifest
        where TLinks: HyperMediaLinks, new()
    {
        public virtual TimeSpan? Duration => null;

        public virtual IEnumerable<Uri> DependentUris { get { yield break; } }
    }
}