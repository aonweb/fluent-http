using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public class HttpBuilderFactory : IHttpBuilderFactory
    {
        private readonly IHttpClientBuilderFactory _clientBuilderFactory;

        static HttpBuilderFactory ()
        {
            InitializationManager.Initialize();
        }

        public HttpBuilderFactory()
            :this(ClientProvider.Current, null) { }

        protected HttpBuilderFactory(IHttpClientBuilderFactory clientBuilderFactory, IEnumerable<IBuilderConfiguration<IHttpBuilder>> configurations)
        {
            _clientBuilderFactory = clientBuilderFactory;
            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<IHttpBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<IHttpBuilder>> Configurations { get; }

        public virtual IHttpBuilder Create()
        {
            var builder = CreateAsChild();

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        public virtual IChildHttpBuilder CreateAsChild()
        {
            var client = _clientBuilderFactory.Create();
            var cacheHandlers = GetCacheHandlers();
            var cacheSettings = GetCacheSettings();
            var handlers = GetHandlers(cacheSettings, cacheHandlers);
            var settings = GetSettings(handlers, cacheSettings);
            return GetBuilder(settings, client);
        }

        protected virtual IChildHttpBuilder GetBuilder(IHttpBuilderSettings settings, IHttpClientBuilder clientBuilder)
        {
            return new HttpBuilder(settings, clientBuilder);
        }

        protected virtual IList<IHttpHandler> GetHandlers(ICacheSettings cacheSettings, IEnumerable<IHttpCacheHandler> handlers)
        {
            return new IHttpHandler[] { new RedirectHandler(), new RetryHandler(), new FollowLocationHandler(), new HttpCacheConfigurationHandler(cacheSettings, Cache.Current, handlers) };
        }

        protected virtual IHttpBuilderSettings GetSettings(IList<IHttpHandler> handlers, ICacheSettings cacheSettings)
        {
            var settings = new HttpBuilderSettings(cacheSettings, handlers, null);

            return settings;
        }

        protected virtual IList<IHttpCacheHandler> GetCacheHandlers()
        {
            return new IHttpCacheHandler[0];
        }

        protected virtual ICacheSettings GetCacheSettings()
        {
            var settings = new CacheSettings();

            return settings;
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<IHttpBuilder>> configurations, IHttpBuilder builder)
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