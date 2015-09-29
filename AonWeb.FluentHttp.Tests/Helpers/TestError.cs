using System;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class TestError : IEquatable<TestError>
    {
        public string Result { get; set; }

        #region IEquatable<TestError>
        public bool Equals(TestError other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Result, other.Result);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((TestError)obj);
        }

        public override int GetHashCode()
        {
            return Result?.GetHashCode() ?? 0;
        }


        #endregion

        #region HalDefaults
        [JsonIgnore]
        public const string SerializedDefault1 = "{\"Result\":\"Response1\"}";

        public static TestError Default1()
        {
            return new TestError
            {
                Result = "Response1"
            };
        }

        #endregion
    }
}