using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL
{
    public static class Defaults
    {
        static Defaults()
        {
            Hal = new HalBuilderDefaults();
            Factory = new FactoryDefaults();

            Reset();
        }
        
        public static readonly HalBuilderDefaults Hal;
        public static readonly FactoryDefaults Factory;

        public class HalBuilderDefaults
        {
            
        }

        public class FactoryDefaults
        {
            public Action<IHalBuilder> DefaultHalBuilderConfiguration { get; set; }
        }

        public static void Reset()
        {
            Factory.DefaultHalBuilderConfiguration = null;
        }
    }
}