using System;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestPoco : IEquatable<TestPoco>
    {
        public string Result { get; set; }

        #region IEquatable<TestPoco>

        public bool Equals(TestPoco other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Result, other.Result);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestPoco)obj);
        }

        public override int GetHashCode()
        {
            return (Result != null ? Result.GetHashCode() : 0);
        }

        #endregion
    }
}