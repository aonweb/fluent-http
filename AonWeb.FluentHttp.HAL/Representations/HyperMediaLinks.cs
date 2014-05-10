using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    [JsonObject]
    public class HyperMediaLinks : List<HyperMediaLink>
    {
        public string Self
        {
            get
            {
                return this.GetSelf();
            }
        }
    }
}