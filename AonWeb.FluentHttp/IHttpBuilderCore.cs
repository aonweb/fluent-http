namespace AonWeb.FluentHttp
{
    public interface IHttpBuilderCore<out TBuilder> : IFluentConfigurable<TBuilder, IHttpBuilderSettings> 
        where TBuilder : IFluentConfigurable<TBuilder, IHttpBuilderSettings>
    { }

}