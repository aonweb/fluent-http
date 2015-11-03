using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp.HAL
{
    public class HalBuilderFactory : IHalBuilderFactory
    {
        private readonly ITypedBuilderFactory _typedBuilderFactory;

        public HalBuilderFactory()
            : this(new TypedBuilderFactory(), new[] { new HalConfiguration() }) { }

        protected HalBuilderFactory(
            ITypedBuilderFactory typedBuilderFactory,
            IEnumerable<IBuilderConfiguration<IHalBuilder>> configurations)
        {
            _typedBuilderFactory = typedBuilderFactory;
            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IHalBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IHalBuilder>> Configurations { get; }

        public IHalBuilder Create()
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

        protected virtual IHalBuilder GetBuilder( IChildTypedBuilder innerBuilder)
        {
            return new HalBuilder(innerBuilder);
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