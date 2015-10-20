using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.HAL
{
    public static class HalDefaults
    {
        private static readonly Lazy<HalBuilderDefaults> _hal;

        static HalDefaults()
        {
            _hal = new Lazy<HalBuilderDefaults>(() => new HalBuilderDefaults(Defaults.Current));
        }

        public static HalBuilderDefaults GetHalBuilderDefaults(this Defaults defaults)
        {
            return _hal.Value;
        }

        public class HalBuilderDefaults
        {
            public HalBuilderDefaults(Defaults defaults)
            {
                defaults.ResetRequested += (sender, args) => Reset();

                Reset();
            }

            public Action<IHalBuilder> DefaultBuilderConfiguration { get; set; }

            private void Reset()
            {
                DefaultBuilderConfiguration = null;
            }
        }
    }
}