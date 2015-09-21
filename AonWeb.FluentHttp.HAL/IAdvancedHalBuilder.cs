namespace AonWeb.FluentHttp.HAL
{
    public interface IAdvancedHalBuilder :  IHalBuilder,
        IAdvancedFluentConfigurable<IAdvancedHalBuilder>,
        IAdvancedHttpBuilderCore<IAdvancedHalBuilder>,
        IFluentConfigurable<IAdvancedHalBuilder, IAdvancedTypedBuilder>
    {

    }
}