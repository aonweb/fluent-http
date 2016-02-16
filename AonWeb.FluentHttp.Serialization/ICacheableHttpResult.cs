using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Serialization
{
    public interface ICacheableHttpResult
    {
        [JsonIgnore]
        TimeSpan? Duration { get; }

        [JsonIgnore]
        IEnumerable<Uri> DependentUris { get; }
    }
}