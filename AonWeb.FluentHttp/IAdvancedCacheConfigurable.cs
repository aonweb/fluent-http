using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public interface IAdvancedCacheConfigurable<out TBuilder> : IFluentConfigurable<TBuilder, ICacheSettings>
        where TBuilder : IAdvancedCacheConfigurable<TBuilder>
    {
        
    }
}