using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HalRequest : IHalRequest
    {
        public HalRequest() { }

        public HalRequest(Uri dependentUri)
            : this(new[] { dependentUri }) { }

        public HalRequest(params Uri[] dependentUris)
        {
            DependentUris = dependentUris;
        }

        [JsonIgnore]
        public IEnumerable<Uri> DependentUris { get; }
    }

    public class HalRequest<T> : HalRequest
        where T : class, IHalResource
    {
        public HalRequest(T resource)
            : this(resource, null) { }

        public HalRequest(T resource, params Uri[] dependentUris)
            : base(GetLinks(resource, dependentUris)) { }

        private static Uri[] GetLinks(T resource, Uri[] dependentUris)
        {
            IEnumerable<Uri> list = dependentUris ?? new Uri[0];

            if (resource != null)
                list = list.Concat(new[] { resource.GetSelf() });

            return list.Distinct().ToArray();
        }
    }
}