using System;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Serialization
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