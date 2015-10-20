using System;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    public class EmptyHalRequest : HalRequest, IEmptyRequest
    {
        public EmptyHalRequest() { }

        public EmptyHalRequest(Uri dependentUri)
            : base(dependentUri) { }

        public EmptyHalRequest(params Uri[] dependentUris)
            : base(dependentUris) { }
    }
}