using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Tests.Helpers
{

    public class TestResult : ResultWithResponseMetadata, IEquatable<TestResult>
    {
        public static string SerializedDefault1 = @"{""StringProperty"":""TestString1"",""IntProperty"":1,""BoolProperty"":true,""DateOffsetProperty"":""2000-01-01T00:00:00-05:00"",""DateProperty"":""2000-01-01T00:00:00""}";
        public static string SerializedDefault2 = @"{""StringProperty"":""TestString2"",""IntProperty"":2,""BoolProperty"":false,""DateOffsetProperty"":""2000-01-02T00:00:00-05:00"",""DateProperty"":""2000-01-02T00:00:00""}";

        public static TestResult Default1()
        {
            return new TestResult
            {
                StringProperty = "TestString1",
                IntProperty = 1,
                BoolProperty = true,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 1, 0, 0, 0),
            };
        }

        public static TestResult Default2()
        {
            return new TestResult
            {
                StringProperty = "TestString2",
                IntProperty = 2,
                BoolProperty = false,
                DateOffsetProperty = new DateTimeOffset(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(-5)),
                DateProperty = new DateTime(2000, 1, 2, 0, 0, 0),
            };
        }

        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public DateTimeOffset DateOffsetProperty { get; set; }
        public DateTime DateProperty { get; set; }

        #region Equality

        public bool Equals(TestResult other)
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TestResult)obj);
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

        public static bool operator ==(TestResult left, TestResult right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TestResult left, TestResult right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    public class SubTestResult : TestResult
    {
        public bool SubBoolProperty { get; set; }
    }

    public class AlternateTestResult : TestResult
    {
        public string Result { get; set; }
    }

    public class CacheableTestResult : TestResult, ICacheableHttpResult
    {
        public TimeSpan? Duration { get; set; } = TimeSpan.FromMinutes(5);

        public IEnumerable<Uri> DependentUris { get { yield break; } }
    }

    public class CacheableTestResultWithDurationNull : CacheableTestResult
    {
        public CacheableTestResultWithDurationNull()
        {
            Duration = null;
        }
    }

    public class CacheableTestResultWithDurationZero : CacheableTestResult
    {
        public CacheableTestResultWithDurationZero()
        {
            Duration = TimeSpan.Zero;
        }
    }

    public class TestException : Exception { }
}