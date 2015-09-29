namespace AonWeb.FluentHttp.Mocks
{
    public class MockHttpBuilderFactory : IMockHttpBuilderFactory
    {
        public IMockHttpBuilder Create()
        {
            var settings = new MockHttpBuilderSettings();

            var builder = new MockHttpBuilder(settings, new MockHttpClientBuilder(), Defaults.Builder.HandlerFactory());

            settings.SetBuilder(builder);

            Defaults.Factory.DefaultHttpBuilderConfiguration?.Invoke(builder);

            return builder;

        }

        public IMockHttpBuilder CreateAsChild()
        {
            var settings = new MockHttpBuilderSettings();
            var builder = new MockHttpBuilder(
                settings,
                new MockHttpClientBuilder(),
                Defaults.Builder.ChildHandlerFactory());

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