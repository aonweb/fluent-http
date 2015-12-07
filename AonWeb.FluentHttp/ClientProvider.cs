using System;
using AonWeb.FluentHttp.Client;

namespace AonWeb.FluentHttp
{
    public static class ClientProvider
    {
        private static Lazy<IHttpClientBuilderFactory> _factory;

        static ClientProvider()
        {
            SetFactory(() => new HttpClientBuilderFactory());
        }

        public static void SetFactory(Func<IHttpClientBuilderFactory> factory)
        {
            _factory = new Lazy<IHttpClientBuilderFactory>(factory, true);
        }

        internal static IHttpClientBuilderFactory Current => _factory.Value;
    }
}