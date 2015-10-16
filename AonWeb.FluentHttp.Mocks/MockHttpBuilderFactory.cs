namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilderFactory : IMockHttpBuilderFactory
    {
        public IMockHttpBuilder Create()
        {
            var settings = new MockHttpBuilderSettings();

            var builder = new MockHttpBuilder(settings, new MockHttpClientBuilder(), Defaults.Current.GetHttpBuilderDefaults().Handlers.GetHandlers(settings));

            settings.SetBuilder(builder);

            Defaults.Current.GetHttpBuilderDefaults().DefaultBuilderConfiguration?.Invoke(builder);

            return builder;

        }

        public IMockHttpBuilder CreateAsChild()
        {
            var settings = new MockHttpBuilderSettings();
            var builder = new MockHttpBuilder(
                settings,
                new MockHttpClientBuilder(),
                Defaults.Current.GetHttpBuilderDefaults().ChildHandlers.GetHandlers(settings));

            settings.SetBuilder(builder);

            // allow parent to cache
            builder.WithCaching(false);

            return builder;
        }

        IChildHttpBuilder IHttpBuilderFactory.CreateAsChild()
        {
            return CreateAsChild();
        }

        IHttpBuilder IHttpBuilderFactory.Create()
        {
            return Create();
        }
    }
}