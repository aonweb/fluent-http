using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestListResourceWithLinks : HalResource<TestLinks>, IEquatable<TestListResourceWithLinks>
    {
        [HalEmbedded("results")]
        public IList<TestResource> Results { get; set; }

        #region IEquatable<TestListResource>

        public bool Equals(TestListResourceWithLinks other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other.Results)) return false;
            if (ReferenceEquals(Results, other.Results)) return true;
            if (Results.Count != other.Results.Count) return false;

            for (var i = 0; i < Results.Count; i++)
            {
                var a = Results[i];
                var b = other.Results[i];

                if (!a.Equals(b))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestListResourceWithLinks)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Results?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Links?.GetHashCode() ?? 0;
                return hashCode;
            }
        }

        #endregion

        #region HalDefaults
        [JsonIgnore]
        public const string SerializedDefault1 = @"{
  ""_links"": {
    ""self"": {
      ""href"": ""http://link.com/1"",
      ""templated"": false
    },
    ""link2"": {
      ""href"": ""http://link.com/2"",
      ""templated"": false
    },
    ""link3"": {
      ""href"": ""http://link.com/3"",
      ""templated"": false
    }
  },
  ""_embedded"": {
    ""results"": [
      {
        ""stringProperty"": ""TestString"",
        ""intProperty"": 2,
        ""boolProperty"": true,
        ""dateOffsetProperty"": ""2000-01-01T00:00:00-05:00"",
        ""dateProperty"": ""2000-01-01T00:00:00"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/self/1"",
            ""templated"": false
          },
          ""template"": {
            ""href"": ""http://link.com/self/1/child/{child-id}"",
            ""templated"": true
          },
          ""nontemplate"": {
            ""href"": ""http://link.com/self/1/child/1"",
            ""templated"": false
          }
        }
      },
      {
        ""stringProperty"": ""TestString2"",
        ""intProperty"": 2,
        ""boolProperty"": false,
        ""dateOffsetProperty"": ""2000-01-02T00:00:00-05:00"",
        ""dateProperty"": ""2000-01-02T00:00:00"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/self/2"",
            ""templated"": false
          },
          ""template"": {
            ""href"": ""http://link.com/self/2/child/{child-id}"",
            ""templated"": true
          },
          ""nontemplate"": {
            ""href"": ""http://link.com/self/2/child/2"",
            ""templated"": false
          }
        }
      }
    ]
  }
}";


        public static TestListResourceWithLinks Default1()
        {
            return new TestListResourceWithLinks
            {
                Links = new TestLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                },
                Results = new List<TestResource>
                {
                    TestResource.Default1(),
                    TestResource.Default2(),
                }
            };
        }

        #endregion
    }
}