namespace AonWeb.FluentHttp
{
    public interface IAdvancedFluentConfigurable<out TAdvancedFluent>
        where TAdvancedFluent : IAdvancedFluentConfigurable<TAdvancedFluent> { }
}