namespace AonWeb.FluentHttp.Handlers.Caching
{
    public enum CacheHandlerType
    {
        Lookup,
        Hit,
        Miss,
        Store,
        Expiring,
        Expired
    }
}