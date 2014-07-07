using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HyperMediaLinks : List<HyperMediaLink>
    {
        public Uri Self()
        {
            return this.GetSelf();
        }
    }
}