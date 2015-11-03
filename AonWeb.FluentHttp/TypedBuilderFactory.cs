using System.Collections.Generic;
using System.Linq;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.Settings;

namespace AonWeb.FluentHttp
{
    public class TypedBuilderFactory : ITypedBuilderFactory
    {
        private readonly IHttpBuilderFactory _httpBuilderFactory;

        public TypedBuilderFactory()
            : this(new HttpBuilderFactory(), null)
        { }

        protected TypedBuilderFactory(
            IHttpBuilderFactory httpBuilderFactory,
            IEnumerable<IBuilderConfiguration<ITypedBuilder>> configurations)
        {
            _httpBuilderFactory = httpBuilderFactory;
            Configurations = (configurations ?? Enumerable.Empty<IBuilderConfiguration<ITypedBuilder>>()).ToList();
        }

        public IList<IBuilderConfiguration<ITypedBuilder>> Configurations { get; }

        public ITypedBuilder Create()
        {
            return CreateAsChild();
        }

        public IChildTypedBuilder CreateAsChild()
        {

            var child = GetChildBuilder();

            // allow parent to cache
            child.WithCaching(false);

            var cacheHandlers = GetCacheHandlers();
            var cacheSettings = GetCacheSettings();
            var handlers = GetHandlers(cacheSettings, cacheHandlers);
            var formatter = GetFormatter();
            var validators = GetResponseValidators();
            var settings = GetSettings(formatter, handlers, cacheSettings, validators);
            var builder = GetBuilder(settings, child);

            ApplyConfigurations(Configurations, builder);

            return builder;
        }

        protected virtual IChildHttpBuilder GetChildBuilder()
        {
            return _httpBuilderFactory.CreateAsChild();
        }

        protected virtual IChildTypedBuilder GetBuilder(ITypedBuilderSettings settings, IChildHttpBuilder innerBuilder)
        {
            return new TypedBuilder(settings, innerBuilder);
        }

        protected virtual IList<ITypedHandler> GetHandlers(ICacheSettings cacheSettings, IEnumerable<ITypedCacheHandler> handlers)
        {
            return new ITypedHandler[] { new TypedCacheConfigurationHandler(cacheSettings, Cache.Current, handlers) };
        }

        protected virtual ITypedBuilderSettings GetSettings(IFormatter formatter, IList<ITypedHandler> handlers, ICacheSettings cacheSettings, IEnumerable<ITypedResponseValidator> validators)
        {
            var settings = new TypedBuilderSettings(formatter, cacheSettings, handlers, validators);

            return settings;
        }

        protected virtual IFormatter GetFormatter()
        {
            return new Formatter();
        }

        protected virtual IEnumerable<ITypedResponseValidator> GetResponseValidators()
        {
            return new[] { new DefaultResponseValidator() };
        }

        protected virtual IList<ITypedCacheHandler> GetCacheHandlers()
        {
            return new ITypedCacheHandler[0];
        }

        protected virtual ICacheSettings GetCacheSettings()
        {
            var settings = new CacheSettings();

            return settings;
        }

        private static void ApplyConfigurations(IEnumerable<IBuilderConfiguration<ITypedBuilder>> configurations, ITypedBuilder builder)
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