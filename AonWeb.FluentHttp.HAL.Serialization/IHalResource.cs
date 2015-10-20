using AonWeb.FluentHttp.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public interface IHalResource: IWritableResponseMetadata
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