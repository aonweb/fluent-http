namespace AonWeb.FluentHttp.Xamarin.HttpClient
{
    public class ModernHttpClientInitializer: Initializer
    {
        public override void Initialize()
        {
            ClientProvider.SetFactory(() => new ModernHttpClientBuilderFactory());
        }
    }
}