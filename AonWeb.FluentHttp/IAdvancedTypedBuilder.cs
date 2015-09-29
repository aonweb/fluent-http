namespace AonWeb.FluentHttp
{
    public interface IAdvancedTypedBuilder :  ITypedBuilder,
        IAdvancedHttpBuilderCore<IAdvancedTypedBuilder>,
        IFluentConfigurable<IAdvancedTypedBuilder, IAdvancedHttpBuilder>
    {

    }
}