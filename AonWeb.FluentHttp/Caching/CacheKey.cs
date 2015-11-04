using System;
using System.Text;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheKey : IEquatable<CacheKey>
    {
        private string _key;

        private CacheKey()
            : this(string.Empty)  { }

        public CacheKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var hash = DigestHelpers.Sha256Hash(Encoding.UTF8.GetBytes(key));
                key = Convert.ToBase64String(hash);
            }

            Key = key;
        }

        public string Key
        {
            get { return _key ?? string.Empty; }
            private set { _key = string.IsNullOrWhiteSpace(value) ? string.Empty : value; }
        }

        public static CacheKey Empty { get; } = new CacheKey();

        public override string ToString()
        {
            var key = string.IsNullOrWhiteSpace(Key) ? "none" : Key;
            return $"Key:{key}";
        }

        #region IEquatable<CacheKey>

        public bool Equals(CacheKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((CacheKey)obj);
        }

        public override int GetHashCode()
        {
            return Key?.GetHashCode() ?? 0;
        }

        public static bool operator ==(CacheKey left, CacheKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CacheKey left, CacheKey right)
        {
            return !Equals(left, right);
        }

        #endregion
    }

    
}