using System.Net.Http;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Settings;
using ModernHttpClient;

namespace AonWeb.FluentHttp.Xamarin.HttpClient
{
    public class ModernHttpClientBuilder: HttpClientBuilder
    {
        public ModernHttpClientBuilder(IHttpClientSettings settings) : base(settings)
        {
        }

        protected override HttpClientHandler GetHttpClientHandler()
        {
            return new NativeMessageHandlerProper();
        }
        
        protected override IHttpClient GetClientInstance(HttpMessageHandler handler, IHttpClientSettings settings)
        {
            return base.GetClientInstance(handler, settings);
        }
    }
}