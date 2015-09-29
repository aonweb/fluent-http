using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestListEmbeddedArrayParentResource : TestResource
    {
        [HalEmbedded("children")]
        public IList<TestPoco> Results { get; set; }

        #region HalDefaults

        [JsonIgnore]
        public new const string SerializedDefault1 = @"{
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
    ""children"": [
      {
        ""result"": ""child-result1""
      },
      {
        ""result"": ""child-result2""
      }
    ]
  }
}";

        [JsonIgnore]
        public new const string SerializedDefault2 = "";
        [JsonIgnore]
        public const string SerializedDefaultWithNullEmbed = @"{
  ""stringProperty"": ""TestString2"",
  ""intProperty"": 2,
  ""boolProperty"": false,
  ""dateOffsetProperty"": ""2000-01-02T00:00:00-05:00"",
  ""dateProperty"": ""2000-01-02T00:00:00"",
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
    ""children"": null
  }
}";

        public new static TestListEmbeddedArrayParentResource Default1()
        {
            return new TestListEmbeddedArrayParentResource
            {
                StringProperty = "TestString",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
                Results = new List<TestPoco>
                {
                    new TestPoco {Result = "child-result1"},
                    new TestPoco {Result = "child-result2"},
                },
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink {Rel = "self", Href = "http://link.com/1"},
                    new HyperMediaLink {Rel = "link2", Href = "http://link.com/2"},
                    new HyperMediaLink {Rel = "link3", Href = "http://link.com/3"}
                }
            };
        }

        public new static TestListEmbeddedArrayParentResource Default2()
        {
            throw new NotImplementedException();
        }

        public static TestListEmbeddedArrayParentResource DefaultWithNullEmbed()
        {
            return new TestListEmbeddedArrayParentResource
            {
                StringProperty = "TestString2",
                IntProperty = 2,
                BoolProperty = false,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 2, 0, 0, 0),
                Results = null,
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                }
            };
        }

        #endregion
    }
}