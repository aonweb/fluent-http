using AonWeb.FluentHttp;
using AonWeb.FluentHttp.Caching;

namespace Aonweb.FluentHttp.Full
{
    public class FullInitializer: Initializer
    {
        public override void Initialize()
        {
            Cache.SetManager(() => new CacheManager(
                new MemoryCacheProvider(),
                new VaryByProvider(new MemoryCacheProvider()),
                new UriInfoProvider(new MemoryCacheProvider()),
                new ResponseSerializer()));
        }
    }
}