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
            using (var builderScope = _scope.BeginLifetimeScope(Constants.BuilderScopeTag))
            {
                var builder = CreateBuilder(builderScope);

                ApplyConfigurations(Configurations, builder);

                return builder;
            }
        }

        private static IHalBuilder CreateBuilder(IComponentContext scope)
        {
            return scope.Resolve<IAdvancedHalBuilder>();
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