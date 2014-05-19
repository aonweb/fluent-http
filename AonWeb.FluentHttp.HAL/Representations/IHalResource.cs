using AonWeb.FluentHttp.HAL.Serialization;

using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Representations
{
    public interface IHalResource
    {
        HyperMediaLinks Links { get; set; }
    }

    public interface IHalResource<TLinks> : IHalResource
        where TLinks : HyperMediaLinks, new()
    {
        new TLinks Links { get; set; }
    }
}