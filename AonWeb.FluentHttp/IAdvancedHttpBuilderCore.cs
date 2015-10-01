using System;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public interface IAdvancedHttpBuilderCore<out TBuilder>: IAdvancedFluentConfigurable<TBuilder>, IConfigurable<IHttpBuilderSettings> 
        where TBuilder: IAdvancedHttpBuilderCore<TBuilder>
    {
        TBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration);
    }
}