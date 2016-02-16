using System;

namespace AonWeb.FluentHttp
{
    public interface IFluentConfigurable<out TFluent, out TSettings> : IConfigurable<TSettings>
        where TFluent : IFluentConfigurable<TFluent, TSettings>
    {
        new TFluent WithConfiguration(Action<TSettings> configuration);
    }
}