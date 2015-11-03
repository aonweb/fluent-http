using System;

namespace AonWeb.FluentHttp
{
    public static class FactoryExtensions
    {
        public static IBuilderFactory<TBuilder> WithBuilderConfiguration<TBuilder>(this IBuilderFactory<TBuilder> factory,
            IBuilderConfiguration<TBuilder> configuration)
        {
            if (configuration == null)
                return factory;

            factory.Configurations.Add(configuration);

            return factory;
        }

        public static IBuilderFactory<TBuilder> WithBuilderConfiguration<TBuilder>(this IBuilderFactory<TBuilder> factory,
            Action<TBuilder> configuration)
        {
            return factory.WithBuilderConfiguration(new BuilderConfiguration<TBuilder>(configuration));
        }
    }
}