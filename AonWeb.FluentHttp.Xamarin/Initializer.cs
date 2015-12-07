namespace AonWeb.FluentHttp.Xamarin
{
    public class Initializer: IInitializer
    {
        public void Initialize()
        {
            Cache.SetProvider(() => new SqlLiteCacheProvider());
            ClientProvider.SetFactory(() => new ModernHttpClientBuilderFactory());
        }
    }
}