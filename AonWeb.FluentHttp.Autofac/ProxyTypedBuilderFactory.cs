using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace AonWeb.FluentHttp.Autofac
{
    public class ProxyTypedBuilderFactory : ITypedBuilderFactory
    {
        private readonly ILifetimeScope _scope;

        public ProxyTypedBuilderFactory(
            ILifetimeScope scope,
            IEnumerable<IBuilderConfiguration<ITypedBuilder>> configurations)
        {
            _scope = scope;

            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<ITypedBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<ITypedBuilder>> Configurations { get; }

        public ITypedBuilder Create()
        {
            using (var builderScope = _scope.BeginLifetimeScope(Constants.BuilderScopeTag))
            {
                var builder = CreateBuilder(builderScope);

                ApplyConfigurations(Configurations, builder);

                return builder;
            }
        }

        public IChildTypedBuilder CreateAsChild()
        {
            return CreateBuilder(_scope);
        }

        private static IChildTypedBuilder CreateBuilder(IComponentContext scope)
        {
            return scope.Resolve<IChildTypedBuilder>();
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<ITypedBuilder>> configurations, ITypedBuilder builder)
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