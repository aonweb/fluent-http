using System;
using AonWeb.FluentHttp.Caching;

namespace AonWeb.FluentHttp.Xamarin
{
    public class XamarinInitializer: Initializer
    {
        public override void Initialize()
        {
            Cache.SetManager(() => new CacheManager(
                 CreateProvider(),
                new VaryByProvider( CreateProvider()),
                new UriInfoProvider( CreateProvider()),
                new ResponseSerializer()));
            ClientProvider.SetFactory(() => new ModernHttpClientBuilderFactory());
        }

        private ICacheProvider CreateProvider()
        {
            throw new NotImplementedException();

            //return new SqlLiteCacheProvider(new PlatformSettings(), Services.Serialization.ServiceStackV3.JsonSerializer);
        }
    }
}