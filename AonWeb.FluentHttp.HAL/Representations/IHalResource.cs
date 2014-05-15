namespace AonWeb.FluentHttp.HAL.Representations
{
    public interface IHalResource
    {
        HyperMediaLinks Links { get; set; }
    }

    public interface IHalResource<TLinks> : IHalResource
        where TLinks : HyperMediaLinks
    {
        new TLinks Links { get; set; }
    }
}