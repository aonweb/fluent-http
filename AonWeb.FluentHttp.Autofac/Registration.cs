using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Helpers;
using AonWeb.FluentHttp.Settings;
using Autofac;
using Module = Autofac.Module;

namespace AonWeb.FluentHttp.Autofac
{
    public class Registration : Module
    {
        private readonly Assembly[] _additionalAssemblies;
        private readonly IList<Type> _excludedTypes;

        private readonly IList<Type> _myTypes = new[]
        {
            typeof (IHttpHandler),
            typeof (ITypedHandler),
            typeof (IHttpCacheHandler),
            typeof (ITypedCacheHandler),
            typeof (IHttpResponseValidator),
            typeof (ITypedResponseValidator)
        };

        public Registration(IEnumerable<Assembly> additionalAssemblies, IEnumerable<Type> excludedTypes)
        {
            _additionalAssemblies = (additionalAssemblies ?? Enumerable.Empty<Assembly>()).ToArray();
            _excludedTypes = (excludedTypes ?? Enumerable.Empty<Type>()).ToList();
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblyNames = new[] { "AonWeb.FluentHttp.Xamarin", "AonWeb.FluentHttp.Full", "AonWeb.FluentHttp", "AonWeb.FluentHttp.HAL", "AonWeb.FluentHttp.Xamarin.HttpClient" };
            var myAssemblies = assemblyNames
                .Select(name =>
                {
                    try
                    {
                        return Assembly.Load(new AssemblyName(name));
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                })
                .Where(a => a != null)
                .ToArray();

            var allAssemblies = _additionalAssemblies.Concat(myAssemblies).ToArray();

            //factories
            builder.RegisterType<HttpClientBuilderFactory>().As<IHttpClientBuilderFactory>();
            builder.RegisterType<ProxyHttpBuilderFactory>().As<IHttpBuilderFactory>();
            builder.RegisterType<ProxyTypedBuilderFactory>().As<ITypedBuilderFactory>();
            builder.RegisterType<ProxyHalBuilderFactory>().As<IHalBuilderFactory>();
            builder.RegisterType<Formatter>().As<IFormatter>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);

            //builders
            builder.Register(c => ClientProvider.Current.Create()).As<IHttpClientBuilder>();
            builder.Register(c => c.Resolve<IHttpBuilderFactory>().Create()).As<IHttpBuilder>();
            builder.Register(c => c.Resolve<ITypedBuilderFactory>().Create()).As<ITypedBuilder>();
            builder.Register(c => c.Resolve<IHalBuilderFactory>().Create()).As<IHalBuilder>();

            builder.RegisterType<HttpBuilder>().As<IChildHttpBuilder>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            builder.RegisterType<TypedBuilder>().As<IChildTypedBuilder>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            builder.RegisterType<HalBuilder>().As<IAdvancedHalBuilder>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            //settings
            builder.RegisterType<HttpBuilderSettings>().As<IHttpBuilderSettings>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<TypedBuilderSettings>().As<ITypedBuilderSettings>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<HttpClientSettings>().As<IHttpClientSettings>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<CacheSettings>().As<ICacheSettings>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);

            foreach (var type in _myTypes)
            {
                builder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => !_excludedTypes.Any(x => x.IsAssignableFrom(t)) && type.IsAssignableFrom(t))
                .As(type)
                .PreserveExistingDefaults();
            }

            builder.RegisterAssemblyTypes(allAssemblies)
               .Where(t => !_excludedTypes.Any(x => x.IsAssignableFrom(t)) &&  t.IsClosedTypeOf(typeof(IBuilderConfiguration<>)))
               .AsImplementedInterfaces()
               .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
              .Where(t => !_excludedTypes.Any(x => x.IsAssignableFrom(t)) && t.IsAssignableTo<ICacheProvider>())
              .As<ICacheProvider>()
              .InstancePerDependency()
              .PreserveExistingDefaults();

            builder.RegisterType<ResponseSerializer>().AsSelf()
                .SingleInstance();

            builder.RegisterType<VaryByProvider>().As<IVaryByProvider>()
                .SingleInstance().PreserveExistingDefaults();

            builder.RegisterType<UriInfoProvider>().As<IUriInfoProvider>()
                .SingleInstance().PreserveExistingDefaults();

            builder.RegisterType<CacheManager>().As<ICacheManager>()
                .SingleInstance().PreserveExistingDefaults()
                .AutoActivate()
                .OnActivated(args => Cache.SetManager(() => args.Instance));

            base.Load(builder);
        }

        public static void Register(ContainerBuilder builder, IEnumerable<Assembly> additionalAssemblies = null, IEnumerable<Type> excludedTypes = null)
        {
            builder.RegisterModule(new Registration(additionalAssemblies, excludedTypes));
        }
    }
}