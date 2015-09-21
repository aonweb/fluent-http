using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderFactory : IHttpBuilderFactory
    {
        public IHttpBuilder Create()
        {
            var settings = new HttpBuilderSettings();

            var builder = new HttpBuilder(settings, new HttpClientBuilder(), Defaults.Builder.HandlerFactory());

            settings.Builder = builder;

            Defaults.Factory.DefaultHttpBuilderConfiguration?.Invoke(builder);

            return builder;

        }

        public IChildHttpBuilder CreateAsChild()
        {
            var settings = new HttpBuilderSettings();
            var builder = new HttpBuilder(
                settings,
                new HttpClientBuilder(),
                Defaults.Builder.ChildHandlerFactory());

            settings.Builder = builder;

            // allow parent to cache
            builder.WithCaching(false);

            return builder;
        }
    }
}