using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace AonWeb.FluentHttp.Autofac
{
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
}