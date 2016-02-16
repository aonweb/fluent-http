using System;
using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestListEmbeddedPropertyChildrenResource : TestResource, IEquatable<TestListEmbeddedPropertyChildrenResource>
    {
        [JsonProperty("results")]
        public IList<TestPoco> Results { get; set; }
        public int Count { get; set; }

        #region  IEquatable<TestListEmbeddedPropertyChildrenResource>

        public bool Equals(TestListEmbeddedPropertyChildrenResource other)
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
            if (obj.GetType() != typeof(TestListEmbeddedPropertyChildrenResource)) return false;
            return Equals((TestListEmbeddedPropertyChildrenResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Count;
                hashCode = (hashCode * 397) ^ (Results != null ? Results.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        #region HalDefaults
        [JsonIgnore]
        public new const string SerializedDefault1 = "";
        [JsonIgnore]
        public new const string SerializedDefault2 = "";

        public new static TestListEmbeddedPropertyChildrenResource Default1()
        {
            return new TestListEmbeddedPropertyChildrenResource
            {
                StringProperty = "TestString",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
                Count = 2,
                Results = new List<TestPoco>
                {
                    new TestPoco { Result = "child-result1"},
                    new TestPoco { Result = "child-result2"},
                },
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink
                    {
                        Rel = "self",
                        Href= "http://link.com/self/1"
                    },
                    new HyperMediaLink
                    {
                        Rel = "template",
                        Href= "http://link.com/self/1/child/{child-id}",
                        Templated = true
                    },
                    new HyperMediaLink
                    {
                        Rel = "nontemplate",
                        Href= "http://link.com/self/1/child/1",
                        Templated = false
                    }
                }
            };
        }

        public new static TestListEmbeddedPropertyChildrenResource Default2()
        {
            return new TestListEmbeddedPropertyChildrenResource
            {
                StringProperty = "TestString2",
                IntProperty = 2,
                BoolProperty = false,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 2, 0, 0, 0),
                Count = 3,
                Results = new List<TestPoco>
                {
                    new TestPoco { Result = "child-result1"},
                    new TestPoco { Result = "child-result2"},
                    new TestPoco { Result = "child-result3"},
                },
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink
                    {
                        Rel = "self",
                        Href= "http://link.com/self/2"
                    },
                    new HyperMediaLink
                    {
                        Rel = "template",
                        Href= "http://link.com/self/2/child/{child-id}",
                        Templated = true
                    },
                    new HyperMediaLink
                    {
                        Rel = "nontemplate",
                        Href= "http://link.com/self/2/child/2",
                        Templated = false
                    }
                }
            };
        }

        #endregion
    }
}