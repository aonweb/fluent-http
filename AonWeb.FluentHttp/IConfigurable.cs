using System;

namespace AonWeb.FluentHttp
{
    public interface IConfigurable<out TSettings>
    {
        void WithConfiguration(Action<TSettings> configuration);
    }
}