using System;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestListEmbeddedPropertyParentsResource : TestResource, IEquatable<TestListEmbeddedPropertyParentsResource>
    {
        [HalEmbedded("parent")]
        public IList<TestListEmbeddedPropertyParentResource> Results { get; set; }

        public int Count { get; set; }

        #region IEquatable<TestListEmbeddedPropertyParentsResource>

        public bool Equals(TestListEmbeddedPropertyParentsResource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Count != other.Count) return false;
            if (!base.Equals(other)) return false;
            if (ReferenceEquals(Results, other.Results)) return true;
            if (ReferenceEquals(null, other.Results)) return false;
            if (ReferenceEquals(null, Results)) return false;

            return !Results.Where((t, i) => !t.Equals(other.Results[i])).Any();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TestListEmbeddedPropertyParentsResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Count;
                hashCode = (hashCode * 397) ^ (Results?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        #endregion

        #region HalDefaults

        [JsonIgnore]
        public new const string SerializedDefault1 = @"{
  ""count"": 2,
  ""stringProperty"": ""TestString"",
  ""intProperty"": 2,
  ""boolProperty"": true,
  ""dateOffsetProperty"": ""2000-01-01T00:00:00-05:00"",
  ""dateProperty"": ""2000-01-01T00:00:00"",
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
    ""parent"": [
      {
        ""stringProperty"": ""TestString1"",
        ""intProperty"": 2,
        ""boolProperty"": true,
        ""dateOffsetProperty"": ""2000-01-01T00:00:00-05:00"",
        ""dateProperty"": ""2000-01-01T00:00:00"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          }
        },
        ""_embedded"": {
          ""children"": {
            ""results"": [
              {
                ""result"": ""child-result1""
              },
              {
                ""result"": ""child-result2""
              }
            ],
            ""count"": 2,
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
          }
        }
      },
      {
        ""stringProperty"": ""TestString2"",
        ""intProperty"": 3,
        ""boolProperty"": false,
        ""dateOffsetProperty"": ""2000-01-02T00:00:00-05:00"",
        ""dateProperty"": ""2000-01-03T00:00:00"",
        ""_links"": {
          ""self"": {
            ""href"": ""http://link.com/1"",
            ""templated"": false
          }
        },
        ""_embedded"": {
          ""children"": {
            ""results"": [
              {
                ""result"": ""child-result1""
              },
              {
                ""result"": ""child-result2""
              },
              {
                ""result"": ""child-result3""
              }
            ],
            ""count"": 3,
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
        }
      }
    ]
  }
}";
        [JsonIgnore]
        public new const string SerializedDefault2 = "";


        public new static TestListEmbeddedPropertyParentsResource Default1()
        {
            return new TestListEmbeddedPropertyParentsResource
            {
                StringProperty = "TestString",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
                Results = new List<TestListEmbeddedPropertyParentResource>
                {
                    TestListEmbeddedPropertyParentResource.Default1(),
                    TestListEmbeddedPropertyParentResource.Default2()
                },
                Count = 2,
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                }
            };
        }

        public new static TestListEmbeddedPropertyParentsResource Default2()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}