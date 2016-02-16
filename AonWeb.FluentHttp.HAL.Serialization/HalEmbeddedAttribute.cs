using System;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HalEmbeddedAttribute : Attribute
    {
        public HalEmbeddedAttribute()
        { }

        public HalEmbeddedAttribute(string rel)
            : this()
        {
            Rel = rel;
        }

        public string Rel { get; set; }

        public Type Type { get; set; }
    }
}