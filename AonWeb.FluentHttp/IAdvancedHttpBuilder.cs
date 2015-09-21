namespace AonWeb.FluentHttp
{
    public interface IAdvancedHttpBuilder : 
        IHttpBuilder,
        IAdvancedFluentConfigurable<IAdvancedHttpBuilder>,
        IAdvancedHttpBuilderCore<IAdvancedHttpBuilder>
    {

    }
}