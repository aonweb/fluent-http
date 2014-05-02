using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public abstract class HalResource : IHalResource
    {
        public const string LinkKeySelf = "self";

        [JsonProperty("_links")]
        public HyperMediaLinks Links { get; set; }
    }
}