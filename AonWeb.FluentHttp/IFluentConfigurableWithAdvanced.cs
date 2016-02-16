namespace AonWeb.FluentHttp
{
    public interface IFluentConfigurableWithAdvanced<out TFluent, out TAdvancedFluent>
        where TFluent : IFluentConfigurableWithAdvanced<TFluent, TAdvancedFluent>
        where TAdvancedFluent : IAdvancedFluentConfigurable<TAdvancedFluent>, TFluent
    {
        TAdvancedFluent Advanced { get; }
    }
}