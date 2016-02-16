using System;
using System.Runtime.Serialization;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestResource : HalResource, IEquatable<TestResource>
    {
        
        public string StringProperty { get; set; }
        
        public int IntProperty { get; set; }
        
        public bool BoolProperty { get; set; }
        
        public DateTimeOffset DateOffsetProperty { get; set; }
        
        public DateTime DateProperty { get; set; }

        #region Equality

        public bool Equals(TestResource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!DateProperty.Equals(other.DateProperty)) return false;
            if (!DateOffsetProperty.Equals(other.DateOffsetProperty) && DateOffsetProperty != DateTimeOffset.MinValue && other.DateOffsetProperty != DateTimeOffset.MinValue) return false;
            if (!BoolProperty.Equals(other.BoolProperty)) return false;
            if (IntProperty != other.IntProperty) return false;
            if (!string.Equals(StringProperty, other.StringProperty)) return false;
            if (ReferenceEquals(Links, other.Links)) return true;
            if (ReferenceEquals(null, other.Links)) return false;
            if (ReferenceEquals(null, Links)) return false;
            if (!Links.Equals(other.Links)) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var other = obj as TestResource;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = DateProperty.GetHashCode();
                hashCode = (hashCode * 397) ^ DateOffsetProperty.GetHashCode();
                hashCode = (hashCode * 397) ^ BoolProperty.GetHashCode();
                hashCode = (hashCode * 397) ^ IntProperty;
                hashCode = (hashCode * 397) ^ StringProperty.GetHashCode();
                hashCode = (hashCode * 397) ^ Links.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(TestResource left, TestResource right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestResource left, TestResource right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region HalDefaults

        [JsonIgnore]
        public const string SerializedDefault1 = @"{
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
}";
        [JsonIgnore]
        public const string SerializedDefault2 = "";

        public static TestResource Default1()
        {
            return new TestResource
            {
                StringProperty = "TestString",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
                Links = new HyperMediaLinks()
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

        public static TestResource Default2()
        {
            return new TestResource
            {
                StringProperty = "TestString2",
                IntProperty = 2,
                BoolProperty = false,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 2, 0, 0, 0),
                Links = new HyperMediaLinks()
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

    public class SubTestResource : TestResource
    {
        public bool SubBoolProperty { get; set; }
        public bool? SubNullableBoolProperty { get; set; }

        #region Defaults
        public new static SubTestResource Default1()
        {
            return new SubTestResource
            {
                SubBoolProperty = false,
                StringProperty = "TestString",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
                Links = new HyperMediaLinks()
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
        #endregion
    }

    public class AlternateTestResource : HalResource
    {
        public string Result { get; set; }
    }
}