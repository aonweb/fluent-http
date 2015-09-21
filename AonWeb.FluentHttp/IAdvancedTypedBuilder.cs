namespace AonWeb.FluentHttp
{
    public interface IAdvancedTypedBuilder :  ITypedBuilder,
        IAdvancedFluentConfigurable<IAdvancedTypedBuilder>,
        IAdvancedHttpBuilderCore<IAdvancedTypedBuilder>
    {

    }
}