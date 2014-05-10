using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public interface IHalRequest
    {
        [JsonIgnore]
        IEnumerable<string> DependentUris { get; }
    }
}