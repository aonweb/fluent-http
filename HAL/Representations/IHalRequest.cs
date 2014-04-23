using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.Fluent.HAL.Representations
{
    public interface IHalRequest
    {
        [JsonIgnore]
        IEnumerable<string> ImpactedUris { get; }
    }
}