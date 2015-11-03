using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.HAL;
using Autofac;

namespace AonWeb.FluentHttp.Autofac
{
    internal class ProxyBuilderScope: IBuilderScope
    {
        public ProxyBuilderScope(ILifetimeScope builderScope)
        {
            BuilderScope = builderScope;
        }

        public ILifetimeScope BuilderScope { get; }
    }

    public class ProxyHttpBuilderFactory: IHttpBuilderFactory
    {
        private readonly ILifetimeScope _scope;

        public ProxyHttpBuilderFactory(
            ILifetimeScope scope,
            IEnumerable<IBuilderConfiguration<IHttpBuilder>> configurations) 
        {
            _scope = scope;

            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IHttpBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IHttpBuilder>> Configurations { get; }

        public IHttpBuilder Create()
        {
            var builder = CreateAsChild();

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        public IChildHttpBuilder CreateAsChild()
        {
            return _scope.Resolve<IChildHttpBuilder>();
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<IHttpBuilder>> configurations, IHttpBuilder builder)
        {
            if (configurations == null)
                return;

            foreach (var configuration in configurations)
            {
                configuration.Configure(builder);
            }
        }
    }

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