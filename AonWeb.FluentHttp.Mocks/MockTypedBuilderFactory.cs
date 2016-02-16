using System.Collections.Generic;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilderFactory : TypedBuilderFactory, IMockTypedBuilderFactory
    {
        public MockTypedBuilderFactory()
            : this(new MockHttpBuilderFactory(), null) { }

        public MockTypedBuilderFactory(
            IHttpBuilderFactory httpBuilderFactory,
            IEnumerable<IBuilderConfiguration<ITypedBuilder>> configurations)
            : base(httpBuilderFactory, configurations)
        {
        }

        protected override IFormatter GetFormatter()
        {
            return new MockFormatter();
        }

        protected override IChildTypedBuilder GetBuilder( ITypedBuilderSettings settings, IChildHttpBuilder innerBuilder)
        {
            return new MockTypedBuilder((IMockTypedBuilderSettings)settings, (IMockHttpBuilder)innerBuilder);
        }

        protected override ITypedBuilderSettings GetSettings( IFormatter formatter, IList<ITypedHandler> handlers,
            ICacheSettings cacheSettings, IEnumerable<ITypedResponseValidator> validators)
        {
            return new MockTypedBuilderSettings((IMockFormatter)formatter, cacheSettings, handlers,  validators);
        }

        public new IMockTypedBuilder Create()
        {
            return (IMockTypedBuilder)base.Create();
        }

        public new IMockTypedBuilder CreateAsChild()
        {
            return (IMockTypedBuilder)base.CreateAsChild();
        }
    }
}