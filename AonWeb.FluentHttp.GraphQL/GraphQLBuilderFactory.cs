using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp.GraphQL
{
    public class GraphQLBuilderFactory : IGraphQLBuilderFactory
    {
        private readonly ITypedBuilderFactory _typedBuilderFactory;

        public GraphQLBuilderFactory()
            : this(new TypedBuilderFactory(), new[] { new GraphQLConfiguration() }) { }

        protected GraphQLBuilderFactory(
            ITypedBuilderFactory typedBuilderFactory,
            IEnumerable<IBuilderConfiguration<IGraphQLBuilder>> configurations)
        {
            _typedBuilderFactory = typedBuilderFactory;
            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IGraphQLBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IGraphQLBuilder>> Configurations { get; }

        public IGraphQLBuilder Create()
        {
            var child = GetChildBuilder();

            var builder = GetBuilder( child);

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        protected virtual IChildTypedBuilder GetChildBuilder()
        {
            return _typedBuilderFactory.CreateAsChild();
        }

        protected virtual IGraphQLBuilder GetBuilder( IChildTypedBuilder innerBuilder)
        {
            return new GraphQLBuilder(innerBuilder);
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<IGraphQLBuilder>> configurations, IGraphQLBuilder builder)
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