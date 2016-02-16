
namespace AonWeb.FluentHttp.HAL
{
    public interface IAdvancedHalBuilder :  IHalBuilder,
        IAdvancedHttpBuilderCore<IAdvancedHalBuilder>,
        IFluentConfigurable<IAdvancedHalBuilder, IAdvancedTypedBuilder>,
        IAdvancedCacheConfigurable<IAdvancedHalBuilder>
    {

    }
}