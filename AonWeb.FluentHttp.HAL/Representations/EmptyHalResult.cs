using System;

using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public class EmptyHalResult : ResultWithResponseMetadata, IHalResource, IEmptyResult  {
        public HyperMediaLinks Links
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}