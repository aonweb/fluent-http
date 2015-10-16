namespace AonWeb.FluentHttp.Mocks
{
    public class MockTypedBuilderFactory : IMockTypedBuilderFactory
    {
        private readonly IMockFormatterFactory _formatterFactory;
        private readonly IMockHttpBuilderFactory _innerBuilderFactory;

        public MockTypedBuilderFactory()
            :this(new MockHttpBuilderFactory(), new MockFormatterFactory()) { }

        public MockTypedBuilderFactory(IMockHttpBuilderFactory innerBuilderFactory, IMockFormatterFactory formatterFactory)
        {
            _innerBuilderFactory = innerBuilderFactory;
            _formatterFactory = formatterFactory;
        }

        public IMockTypedBuilder Create()
        {
            var child = _innerBuilderFactory.CreateAsChild();
            var formatter = _formatterFactory.Create();
            var settings = new MockTypedBuilderSettings(formatter);

            var builder = new MockTypedBuilder(settings, child, Defaults.Current.GetTypedBuilderDefaults().Handlers.GetHandlers(settings));

            settings.SetBuilder(builder);

            Defaults.Current.GetTypedBuilderDefaults().DefaultBuilderConfiguration?.Invoke(builder);

            return builder;
        }

        public IMockTypedBuilder CreateAsChild()
        {
            var child = _innerBuilderFactory.CreateAsChild();
            var formatter = _formatterFactory.Create();
            var settings = new MockTypedBuilderSettings(formatter);

            var builder = new MockTypedBuilder(settings, child, Defaults.Current.GetTypedBuilderDefaults().ChildHandlers.GetHandlers(settings));

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