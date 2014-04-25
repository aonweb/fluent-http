using Newtonsoft.Json;

namespace AonWeb.Fluent.HAL.Representations
{
    public abstract class HalResource : IHalResource
    {
        public const string LinkKeySelf = "self";

        [JsonProperty("_links")]
        public HyperMediaLinks Links { get; set; }
    }
}