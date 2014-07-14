using System;
using System.Collections.Generic;

using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;

using Newtonsoft.Json;

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

        public static string TestListEmbeddedArrayChildJson = @"{
  ""parent"": [
    {
      ""result"": ""parent1"",
      ""_embedded"": {
        ""children"": [
          {
            ""result"": ""child1""
          },
          {
            ""result"": ""child2""
          }
        ],
        ""links"": {
          ""self"": {
            ""href"": ""http://localhost:8889/canonical/parent1/children"",
            ""templated"": false
          }
        },
        ""count"": 2
      }
    },
    {
      ""result"": ""parent2"",
      ""_embedded"": {
        ""children"": [
          {
            ""result"": ""child1""
          },
          {
            ""result"": ""child2""
          }
        ],
        ""links"": {
          ""self"": {
            ""href"": ""http://localhost:8889/canonical/parent2/children"",
            ""templated"": false
          }
        },
        ""count"": 2
      }
    }
  ],
  ""links"": {
    ""self"": {
      ""href"": ""http://localhost:8889/canonical/parent2"",
      ""templated"": false
    }
  },
  ""count"": 2
}";
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

    public class TestListResourceWithLinks : HalResource<TestLinks>
    {
        [HalEmbedded("results")]
        public IList<TestResource> Results { get; set; }
    }

    public class TestListEmbeddedArrayParentsResource : TestResource
    {
        [JsonProperty("parent")]
        public IList<TestListEmbeddedArrayParentResource> Results { get; set; }
        public int Count { get; set; }
    }

    public class TestListEmbeddedArrayParentResource : TestResource
    {
        [JsonProperty("_embedded")]
        public TestListEmbeddedArrayChildrenResource Children { get; set; }
    }

    public class TestListEmbeddedArrayChildrenResource : TestResource
    {
        [JsonProperty("children")]
        public IList<TestPoco> Results { get; set; }
        public int Count { get; set; }
    }

    public class TestPoco
    {
        public string Result { get; set; }
    }
}