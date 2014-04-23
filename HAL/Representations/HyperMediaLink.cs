using System;
using Newtonsoft.Json;

namespace AonWeb.Fluent.HAL.Representations
{
    public class HyperMediaLink
    {
        public string Rel { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("templated")]
        public bool IsTemplated { get; set; }

        public override string ToString()
        {
            return string.Format("Rel: '{0}', Href: '{1}', IsTemplated: '{2}'", Rel, Href, IsTemplated);
        }
    }
}