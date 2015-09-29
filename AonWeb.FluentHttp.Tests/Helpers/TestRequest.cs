using System;
using AonWeb.FluentHttp.HAL.Representations;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class AlternateTestRequest : HalRequest
    {
        public string Result { get; set; }
    }

    public class TestRequest : HalRequest, IEquatable<TestRequest>
    {
        [JsonIgnore]
        public const string SerializedDefault1 = "";
        [JsonIgnore]
        public const string SerializedDefault2 = "";

        public static TestRequest Default1()
        {
            return new TestRequest
            {
                StringProperty = "TestString",
                IntProperty = 2,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
            };
        }

        public static TestRequest Default2()
        {
            return new TestRequest
            {
                StringProperty = "TestString2",
                IntProperty = 2,
                BoolProperty = false,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 2, 0, 0, 0),
            };
        }

        public TestRequest()
        {
            StringProperty = "TestString";
            IntProperty = 2;
            BoolProperty = true;
            DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5));
            DateProperty = new DateTime(2000, 1, 1, 0, 0, 0);
        }

        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public DateTimeOffset DateOffsetProperty { get; set; }
        public DateTime DateProperty { get; set; }

        #region Equality

        public bool Equals(TestRequest other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return DateProperty.Equals(other.DateProperty)
                   && DateOffsetProperty.Equals(other.DateOffsetProperty)
                   && BoolProperty.Equals(other.BoolProperty)
                   && IntProperty == other.IntProperty
                   && string.Equals(StringProperty, other.StringProperty);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((TestRequest)obj);
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
                return hashCode;
            }
        }

        public static bool operator ==(TestRequest left, TestRequest right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestRequest left, TestRequest right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}