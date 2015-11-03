using System;

namespace AonWeb.FluentHttp
{
    public class BuilderConfiguration<TBuilder> : IBuilderConfiguration<TBuilder>
    {
        private readonly Action<TBuilder> _configuration;

        public BuilderConfiguration(Action<TBuilder> configuration)
        {
            _configuration = configuration;
        }


        public void Configure(TBuilder builder)
        {
            _configuration?.Invoke(builder);
        }
    }
}