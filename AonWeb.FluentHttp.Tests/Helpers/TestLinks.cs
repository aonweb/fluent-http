using System;
using AonWeb.FluentHttp.HAL.Serialization;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestLinks : HyperMediaLinks
    {
        public Uri Link1()
        {
            return this.GetLink("link1");
        }
    }
}