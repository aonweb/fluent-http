using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    [JsonObject]
    public class HyperMediaLinks : List<HyperMediaLink>
    {
    }
}