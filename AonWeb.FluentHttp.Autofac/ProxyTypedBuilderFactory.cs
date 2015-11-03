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
            var builder = CreateAsChild();

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        public IChildTypedBuilder CreateAsChild()
        {
            return _scope.Resolve<IChildTypedBuilder>();
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