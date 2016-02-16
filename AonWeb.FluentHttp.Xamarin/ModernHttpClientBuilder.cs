using System.Net.Http;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Xamarin
{
    public class ModernHttpClientBuilder: HttpClientBuilder
    {
        public ModernHttpClientBuilder(IHttpClientSettings settings) : base(settings)
        {

        }

        protected override HttpMessageHandler CreateHandler(IHttpClientSettings settings)
        {
            return base.CreateHandler(settings);
        }

        protected override IHttpClient GetClientInstance(HttpMessageHandler handler, IHttpClientSettings settings)
        {
            return base.GetClientInstance(handler, settings);
        }
    }
}