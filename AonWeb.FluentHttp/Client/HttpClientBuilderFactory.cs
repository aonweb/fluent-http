using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp.Client
{
    public class HttpClientBuilderFactory : IHttpClientBuilderFactory
    {
        public HttpClientBuilderFactory()
            :this(null) { }

        protected HttpClientBuilderFactory(IEnumerable<IBuilderConfiguration<IHttpClientBuilder>> configurations)
        {
            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IHttpClientBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IHttpClientBuilder>> Configurations { get; }

        public virtual IHttpClientBuilder Create()
        {
            var builder = new HttpClientBuilder(new HttpClientSettings());

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        protected static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<IHttpClientBuilder>> configurations, IHttpClientBuilder builder)
        {
            if (configurations == null)
                return;

            foreach (var configuration in configurations)
            {
                configuration.Configure(builder);
            }
        }
    }
}