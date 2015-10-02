using AonWeb.FluentHttp.Handlers.Caching;

namespace AonWeb.FluentHttp.Caching
{
    public interface ICacheKeyBuilder
    {
        CacheKey BuildKey(ICacheContext context);
    }
}