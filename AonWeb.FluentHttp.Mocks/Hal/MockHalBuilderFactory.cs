using System.Collections.Generic;
using AonWeb.FluentHttp.HAL;

namespace AonWeb.FluentHttp.Mocks.Hal
{
    public class MockHalBuilderFactory : HalBuilderFactory, IMockHalBuilderFactory
    {
        public MockHalBuilderFactory()
            : this(new MockTypedBuilderFactory(),
                new [] { new HalConfiguration() })
        { }

        public MockHalBuilderFactory(
            ITypedBuilderFactory typedBuilderFactory,
            IEnumerable<IBuilderConfiguration<IHalBuilder>> configurations) 
                : base(typedBuilderFactory, configurations)
        { }

        protected override IHalBuilder GetBuilder(IChildTypedBuilder innerBuilder)
        {
            return new MockHalBuilder((IMockTypedBuilder)innerBuilder);
        }

        public new IMockHalBuilder Create()
        {
            return (IMockHalBuilder)base.Create();
        }
    }
}