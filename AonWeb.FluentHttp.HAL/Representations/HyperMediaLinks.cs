using System;
using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class HyperMediaLinks : List<HyperMediaLink>, IEquatable<HyperMediaLinks>
    {
        public Uri Self()
        {
            return this.GetSelf();
        }

        public bool Equals(HyperMediaLinks other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Count != other.Count) return false;
            for (var i = 0; i < Count; i++)
            {
              if(!this[i].Equals(other[i]))
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as HyperMediaLinks;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Aggregate(0, (current, link) => (current*397) ^ link.GetHashCode());
        }
    }
}