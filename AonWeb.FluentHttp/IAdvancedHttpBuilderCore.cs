using System;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public interface IAdvancedHttpBuilderCore<out TBuilder>
    {
        TBuilder WithClientConfiguration(Action<IHttpClientBuilder> configuration);
    }
}