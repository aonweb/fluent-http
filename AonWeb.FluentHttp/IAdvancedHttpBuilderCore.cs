using System;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public interface IAdvancedHttpBuilderCore<out TBuilder>: IAdvancedFluentConfigurable<TBuilder>
        where TBuilder: IAdvancedFluentConfigurable<TBuilder>
    {
        TBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration);
    }
}