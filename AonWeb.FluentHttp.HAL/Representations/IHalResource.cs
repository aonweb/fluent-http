using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public interface IHalResource
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