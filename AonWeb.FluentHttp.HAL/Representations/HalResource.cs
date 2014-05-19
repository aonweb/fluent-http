using System;

using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public abstract class HalResource : IHalResource
    {
        public const string LinkKeySelf = "self";

        [JsonProperty("_links")]
        public HyperMediaLinks Links { get; set; }
    }

    public abstract class HalResource<TLinks> : IHalResource<TLinks>
        where TLinks : HyperMediaLinks, new()
    {
        [JsonIgnore]
        HyperMediaLinks IHalResource.Links
        {
            get
            {
                return Links;
            }
            set
            {
                if (value != null && !(value is TLinks))
                    throw new ArgumentException(string.Format(SR.InvalidTypeErroFormat, typeof(TLinks).Name));
            }
        }

        [JsonProperty("_links")]
        public TLinks Links { get; set; }
    } 
}