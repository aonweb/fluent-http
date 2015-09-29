using System;
using AonWeb.FluentHttp.HAL.Representations;
using AonWeb.FluentHttp.HAL.Serialization;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestListEmbeddedPropertyParentResource : TestResource, IEquatable<TestListEmbeddedPropertyParentResource>
    {
        [HalEmbedded("children")]
        public TestListEmbeddedPropertyChildrenResource Children { get; set; }

        #region IEquatable<TestListEmbeddedPropertyParentResource>
        public bool Equals(TestListEmbeddedPropertyParentResource other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && Equals(Children, other.Children);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestListEmbeddedPropertyParentResource)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (Children?.GetHashCode() ?? 0);
            }
        }

        #endregion

        #region HalDefaults

        public new static TestListEmbeddedPropertyParentResource Default1()
        {
            return new TestListEmbeddedPropertyParentResource
            {
                StringProperty = "TestString1",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
                Children = TestListEmbeddedPropertyChildrenResource.Default1(),
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink {Rel = "self", Href = "http://link.com/1"}
                }
            };
        }

        public new static TestListEmbeddedPropertyParentResource Default2()
        {
            return new TestListEmbeddedPropertyParentResource
            {
                StringProperty = "TestString2",
                IntProperty = 3,
                BoolProperty = false,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 3, 0, 0, 0),
                Children = TestListEmbeddedPropertyChildrenResource.Default2(),
                Links = new HyperMediaLinks
                {
                    new HyperMediaLink {Rel = "self", Href = "http://link.com/1"}
                }
            };
        }

        #endregion     
    }
}