using System;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestResourceWithLinks : HalResource<TestLinks>, IEquatable<TestResourceWithLinks>
    {
        public string Result { get; set; }

        public bool Equals(TestResourceWithLinks other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Result.Equals(other.Result) && Links.Equals(other.Links);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestResourceWithLinks) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Result?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Links?.GetHashCode() ?? 0;
                return hashCode;
            }
        }

        #region HalDefaults
        [JsonIgnore]
        public const string SerializedDefault1 = @"{
  ""result"": ""Result1"",
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
  }
}";

        public static TestResourceWithLinks Default1()
        {
            return new TestResourceWithLinks
            {
                Result = "Result1",
                Links = new TestLinks
                {
                    new HyperMediaLink{ Rel = "self", Href="http://link.com/1"},
                    new HyperMediaLink{ Rel = "link2", Href="http://link.com/2"},
                    new HyperMediaLink{ Rel = "link3", Href="http://link.com/3"}
                },
            };
        }

        #endregion
    }
}