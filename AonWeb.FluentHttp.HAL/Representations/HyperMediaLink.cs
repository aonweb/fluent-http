using System;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HyperMediaLink
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public bool IsTemplated { get; set; }

        public override string ToString()
        {
            return string.Format("Rel: '{0}', Href: '{1}', IsTemplated: '{2}'", Rel, Href, IsTemplated);
        }
    }
}