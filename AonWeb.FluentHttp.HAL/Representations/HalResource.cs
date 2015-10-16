using System;
using System.Collections.Generic;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public abstract class HalResource : ResultWithResponseMetadata, IHalResource
    {
        protected HalResource()
        {
            Links = new HyperMediaLinks();
        }

        public const string LinkKeySelf = "self";
        public HyperMediaLinks Links { get; set; }
    }

    public abstract class HalResource<TLinks> : ResultWithResponseMetadata, IHalResource<TLinks>
        where TLinks : HyperMediaLinks, new()
    {
        protected HalResource()
        {
            Links = new TLinks();
        }

        HyperMediaLinks IHalResource.Links
        {
            get
            {
                return Links;
            }
            set
            {
                if (value != null && !(value is TLinks))
                    throw new ArgumentException(string.Format(SR.InvalidTypeErroFormat, typeof(TLinks).Name));
            }
        }

        public TLinks Links { get; set; }
    } 
}