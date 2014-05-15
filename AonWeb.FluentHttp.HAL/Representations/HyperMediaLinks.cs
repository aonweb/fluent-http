using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    [JsonObject]
    public class HyperMediaLinks : List<HyperMediaLink>
    {
        public Uri Self()
        {
            return this.GetSelf();
        }
    }
}