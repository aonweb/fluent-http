using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheableHttpResult
    {
        [JsonIgnore]
        TimeSpan? Duration { get; }

        [JsonIgnore]
        IEnumerable<string> DependentUris { get; }
    }
}