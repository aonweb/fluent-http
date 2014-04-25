using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AonWeb.Fluent.HAL.Representations
{
    public class HalRequest : IHalRequest
    {
        public HalRequest(string impactedUri)
            : this(new[] { impactedUri }) { }

        public HalRequest(params string[] impactedUris)
        {
            ImpactedUris = impactedUris;
        }

        [JsonIgnore]
        public IEnumerable<string> ImpactedUris { get; private set; }
    }

    public class HalRequest<T> : HalRequest
        where T : class, IHalResource
    {
        public HalRequest(T resource)
            : this(resource, null) { }

        public HalRequest(T resource, params string[] impactedUris)
            : base(GetLinks(resource, impactedUris)) { }

        private static string[] GetLinks(T resource, string[] impactedUris)
        {
            IEnumerable<string> list = impactedUris ?? new string[0];

            if (resource != null)
                list = list.Concat(new[] { resource.GetSelf() });

            return list.ToArray();
        }
    }
}