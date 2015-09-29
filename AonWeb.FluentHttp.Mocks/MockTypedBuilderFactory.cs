namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilderFactory : IMockTypedBuilderFactory
    {
        private readonly IMockHttpBuilderFactory _innerBuilderFactory;

        public MockTypedBuilderFactory()
            :this(new MockHttpBuilderFactory()) { }

        public MockTypedBuilderFactory(IMockHttpBuilderFactory innerBuilderFactory)
        {
            _innerBuilderFactory = innerBuilderFactory;
        }

        public IMockTypedBuilder Create()
        {
            var child = _innerBuilderFactory.CreateAsChild();
            var settings = new MockTypedBuilderSettings();

            var builder = new MockTypedBuilder(settings, child, new MockFormatter(), Defaults.TypedBuilder.HandlerFactory());

            settings.SetBuilder(builder);

            Defaults.Factory.DefaultTypedBuilderConfiguration?.Invoke(builder);

            return builder;
        }

        public IMockTypedBuilder CreateAsChild()
        {
            var child = _innerBuilderFactory.CreateAsChild();
            var settings = new MockTypedBuilderSettings();

            var builder = new MockTypedBuilder(settings, child, new MockFormatter(), Defaults.TypedBuilder.ChildHandlerFactory());

            settings.SetBuilder(builder);

            return builder;
        }

        IChildTypedBuilder ITypedBuilderFactory.CreateAsChild()
        {
            return CreateAsChild();
        }

        ITypedBuilder ITypedBuilderFactory.Create()
        {
            return Create();
        }
    }
}