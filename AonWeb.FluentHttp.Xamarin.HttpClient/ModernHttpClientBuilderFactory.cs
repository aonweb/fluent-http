using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Xamarin.HttpClient
{
    public class ModernHttpClientBuilderFactory: HttpClientBuilderFactory
    {
        public override IHttpClientBuilder Create()
        {
            var builder = new ModernHttpClientBuilder(new HttpClientSettings());

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

    }
}