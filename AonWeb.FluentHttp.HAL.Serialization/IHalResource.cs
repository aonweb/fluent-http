using AonWeb.FluentHttp.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public interface IHalResource: IResultWithMetadata
    {
        [JsonProperty("_links")]
        HyperMediaLinks Links { get; set; }
    }

    public interface IHalResource<TLinks> : IHalResource
        where TLinks : HyperMediaLinks, new()
    {
        [JsonProperty("_links")]
        new TLinks Links { get; set; }
    }
}