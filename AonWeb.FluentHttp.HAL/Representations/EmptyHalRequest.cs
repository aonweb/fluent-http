using System;

using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Representations
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