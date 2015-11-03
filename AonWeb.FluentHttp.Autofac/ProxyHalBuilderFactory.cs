using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.HAL;
using Autofac;

namespace AonWeb.FluentHttp.Autofac
{
    public class ProxyHalBuilderFactory : IHalBuilderFactory
    {
        private readonly ILifetimeScope _scope;

        public ProxyHalBuilderFactory(
            ILifetimeScope scope,
            IEnumerable<IBuilderConfiguration<IHalBuilder>> configurations) 
        {
            _scope = scope;

            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IHalBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IHalBuilder>> Configurations { get; }

        public IHalBuilder Create()
        {
            var builder = _scope.Resolve<IAdvancedHalBuilder>();

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<IHalBuilder>> configurations, IHalBuilder builder)
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