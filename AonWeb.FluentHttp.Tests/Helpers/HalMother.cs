namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class HalMother
    {
        public static string TestResourceJson = "{\"result\":\"Response1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}";
        public static string TestResourceJson2 = "{\"result\":\"Response2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}";
        public static string TestResourceWithLinksJson = "{\"result\":\"Response1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false},\"link1\":{\"href\":\"http://localhost:8889/link1/1\",\"templated\":false}}}";
        public static string TestResourceWithLinksJson2 = "{\"result\":\"Response2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false},\"link1\":{\"href\":\"http://localhost:8889/link1/1\",\"templated\":false}}}";
        public static string TestListJson = "{\"_embedded\":{\"results\":[{\"result\":\"Response1ForItem1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}},{\"result\":\"Response1ForItem2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/2\",\"templated\":false}}},{\"result\":\"Response1ForItem3\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}]},\"_links\":{\"self\":{\"href\":\"http://localhost:8889/list/1\",\"templated\":false}}}";
        public static string TestListJson2 = "{\"_embedded\":{\"results\":[{\"result\":\"Response2ForItem1\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}},{\"result\":\"Response2ForItem2\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/2\",\"templated\":false}}},{\"result\":\"Response2ForItem3\",\"_links\":{\"self\":{\"href\":\"http://localhost:8889/canonical/1\",\"templated\":false}}}]},\"_links\":{\"self\":{\"href\":\"http://localhost:8889/list/1\",\"templated\":false}}}";

        public static string TestListEmbeddedArrayJson = @"{
    ""result"": ""parent"",
    ""_embedded"": [{
        ""children"": [{
            ""result"":""child1""
        },
        {
            ""result"":""child1""
        }]
    }],
    ""_links"": {
        ""self"": {
            ""href"": ""http://link.com"",
            ""templated"": false
        }
    }
}";
        public static string TestListEmbeddedPropertyJson = @"{
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
}