using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Client;
using Autofac;

namespace AonWeb.FluentHttp.Autofac
{
    public class ProxyHttpClientBuilderFactory : IHttpClientBuilderFactory
    {
        private readonly ILifetimeScope _scope;

        public ProxyHttpClientBuilderFactory(
            ILifetimeScope scope,

            IEnumerable<IBuilderConfiguration<IHttpClientBuilder>> configurations)
        {
            _scope = scope;

            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IHttpClientBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IHttpClientBuilder>> Configurations { get; }

        public IHttpClientBuilder Create()
        {
            using (var builderScope = _scope.BeginLifetimeScope(Constants.BuilderScopeTag))
            {
                var builder = builderScope.Resolve<IHttpClientBuilder>();

                ApplyConfigurations(Configurations, builder);

                return builder;
            }
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<IHttpClientBuilder>> configurations, IHttpClientBuilder builder)
        {
            if (configurations == null)
                return;

            foreach (var configuration in configurations)
            {
                configuration.Configure(builder);
            }
        }
    }
}