using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderFactory : IHttpBuilderFactory
    {
        public IHttpBuilder Create()
        {
            var settings = new HttpBuilderSettings();

            var builder = new HttpBuilder(settings, new HttpClientBuilder(), Defaults.Current.GetHttpBuilderDefaults().Handlers.GetHandlers(settings));

            settings.Builder = builder;

            Defaults.Current.GetHttpBuilderDefaults().DefaultBuilderConfiguration?.Invoke(builder);

            return builder;

        }

        public IChildHttpBuilder CreateAsChild()
        {
            var settings = new HttpBuilderSettings();
            var builder = new HttpBuilder(
                settings,
                new HttpClientBuilder(),
                Defaults.Current.GetHttpBuilderDefaults().ChildHandlers.GetHandlers(settings));

            settings.Builder = builder;

            // allow parent to cache
            builder.WithCaching(false);

            return builder;
        }
    }
}