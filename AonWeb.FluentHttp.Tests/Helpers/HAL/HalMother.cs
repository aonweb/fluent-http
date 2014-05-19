using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;

namespace AonWeb.FluentHttp.Tests.Helpers.HAL
{
    public class HalMother
    {
        public static string TestResourceJson = "{\"result\":\"Response1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}";
        public static string TestResourceJson2 = "{\"result\":\"Response2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}";
        public static string TestResourceWithLinksJson = "{\"result\":\"Response1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false},\"link1\":{\"href\":\"http://localhost:8889/link1/1\",\"templated\":false}}}";
        public static string TestResourceWithLinksJson2 = "{\"result\":\"Response2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false},\"link1\":{\"href\":\"http://localhost:8889/link1/1\",\"templated\":false}}}";
        public static string TestListJson = "{\"_embedded\":{\"results\":[{\"result\":\"Response1ForItem1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}},{\"result\":\"Response1ForItem2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/2\",\"templated\":false}}},{\"result\":\"Response1ForItem3\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}]},\"_links\":{\"self\":{\"href\":\"http://localhost:8889/list/1\",\"templated\":false}}}";
        public static string TestListJson2 = "{\"_embedded\":{\"results\":[{\"result\":\"Response2ForItem1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}},{\"result\":\"Response2ForItem2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/2\",\"templated\":false}}},{\"result\":\"Response2ForItem3\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}]},\"_links\":{\"self\":{\"href\":\"http://localhost:8889/list/1\",\"templated\":false}}}";

    }

    public class TestResource : HalResource
    {
        public string Result { get; set; }
    }

    public class TestResourceWithLinks : HalResource<TestLinks>
    {
        public string Result { get; set; }
    }

    public class TestLinks : HyperMediaLinks
    {
        public Uri Link1()
        {
            return this.GetLink("link1");
        }
    }

    public class TestListResource : HalResource
    {
        [HalEmbedded("results")]
        public IList<TestResource> Results { get; set; }
    }
}