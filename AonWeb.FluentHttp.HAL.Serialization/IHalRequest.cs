using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public interface IHalRequest
    {
        [JsonIgnore]
        IEnumerable<Uri> DependentUris { get; }
    }
}