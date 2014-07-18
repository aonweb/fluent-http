using System;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HyperMediaLink : IEquatable<HyperMediaLink>
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public bool Templated { get; set; }

        public override string ToString()
        {
            return string.Format("Rel: '{0}', Href: '{1}', Templated: '{2}'", Rel, Href, Templated);
        }

        #region IEquality Implementation

        public bool Equals(HyperMediaLink other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Rel, other.Rel) && string.Equals(Href, other.Href) && Templated.Equals(other.Templated);
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
            return Equals((HyperMediaLink)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Rel != null ? Rel.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Href != null ? Href.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Templated.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(HyperMediaLink left, HyperMediaLink right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HyperMediaLink left, HyperMediaLink right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}