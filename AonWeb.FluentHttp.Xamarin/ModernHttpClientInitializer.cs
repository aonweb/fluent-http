using System;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.Xamarin
{
    public class ModernHttpClientInitializer: Initializer
    {
        public override void Initialize()
        {
            ClientProvider.SetFactory(() => new ModernHttpClientBuilderFactory());
        }
    }

    //public class SqlLiteCacheProviderInitializer : Initializer
    //{
    //    public override void Initialize()
    //    {
    //        Cache.SetManager(() => new CacheManager(
    //             CreateProvider(),
    //            new VaryByProvider(CreateProvider()),
    //            new UriInfoProvider(CreateProvider()),
    //            new ResponseSerializer()));
    //    }

    //    private ICacheProvider CreateProvider()
    //    {
    //        var platformSettings = new PlatformSettings();
    //        var jsonSerializer = new ServiceStackV3.JsonSerializer

    //        return new SqlLiteCacheProvider(new PlatformSettings(), );
    //    }
    //}
}