using System;
using System.Linq;
using System.Reflection;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Client;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Handlers.Caching;
using AonWeb.FluentHttp.HAL;
using AonWeb.FluentHttp.Settings;
using Autofac;
using Module = Autofac.Module;

namespace AonWeb.FluentHttp.Autofac
{
    public class Registration : Module
    {
        private readonly Assembly[] _additionalAssemblies;

        public Registration(params Assembly[] additionalAssemblies)
        {
            _additionalAssemblies = additionalAssemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblyNames = new[] { "AonWeb.FluentHttp", "AonWeb.FluentHttp.HAL" };
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
            builder.RegisterType<ProxyHttpBuilderFactory>().As<IHttpBuilderFactory>();
            builder.RegisterType<ProxyTypedBuilderFactory>().As<ITypedBuilderFactory>();
            builder.RegisterType<ProxyHalBuilderFactory>().As<IHalBuilderFactory>();
            builder.RegisterType<HttpClientBuilder>().As<IHttpClientBuilder>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<Formatter>().As<IFormatter>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);

            //builders
            builder.Register(c => c.Resolve<IHttpBuilderFactory>().Create()).As<IHttpBuilder>();
            builder.Register(c => c.Resolve<ITypedBuilderFactory>().Create()).As<ITypedBuilder>();
            builder.Register(c => c.Resolve<IHalBuilderFactory>().Create()).As<IHalBuilder>();

            builder.RegisterType<HttpBuilder>().As<IChildHttpBuilder>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag).PreserveExistingDefaults();
            builder.RegisterType<TypedBuilder>().As<IChildTypedBuilder>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag).PreserveExistingDefaults();
            builder.RegisterType<HalBuilder>().As<IAdvancedHalBuilder>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag).PreserveExistingDefaults();

            //settings
            builder.RegisterType<HttpBuilderSettings>().As<IHttpBuilderSettings>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<TypedBuilderSettings>().As<ITypedBuilderSettings>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<HttpClientSettings>().As<IHttpClientSettings>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);
            builder.RegisterType<CacheSettings>().As<ICacheSettings>().InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag);

            //handlers
            builder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => t.IsAssignableTo<IHttpHandler>())
                .As<IHttpHandler>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => t.IsAssignableTo<ITypedHandler>())
                .As<ITypedHandler>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => t.IsAssignableTo<IHttpCacheHandler>())
                .As<IHttpCacheHandler>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
                .Where(t => t.IsAssignableTo<ITypedCacheHandler>())
                .As<ITypedCacheHandler>()
                .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
                .PreserveExistingDefaults();

            //configurations
            builder.RegisterAssemblyTypes(allAssemblies)
               .Where(t => t.IsAssignableTo<IHttpResponseValidator>())
               .As<IHttpResponseValidator>()
               .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
               .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
               .Where(t => t.IsAssignableTo<ITypedResponseValidator>())
               .As<ITypedResponseValidator>()
               .InstancePerMatchingLifetimeScope(Constants.BuilderScopeTag)
               .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
               .Where(t =>
               {
                   var ret = t.IsAssignableTo<IBuilderConfiguration<IHttpBuilder>>() ||
                             t.IsAssignableTo<IBuilderConfiguration<ITypedBuilder>>() ||
                             t.IsAssignableTo<IBuilderConfiguration<IHalBuilder>>();

                   return ret;
               })
               .AsImplementedInterfaces()
               .PreserveExistingDefaults();


            //providers
            builder.RegisterAssemblyTypes(allAssemblies)
               .Where(t => t.IsAssignableTo<IVaryByProvider>())
               .As<IVaryByProvider>()
               .SingleInstance()
               .PreserveExistingDefaults();

            builder.RegisterAssemblyTypes(allAssemblies)
               .Where(t => t.IsAssignableTo<ICacheProvider>())
               .As<ICacheProvider>()
               .SingleInstance()
               .AutoActivate()
               .OnActivated(args =>
               {
                   Cache.SetProvider(() => (ICacheProvider)args.Instance);
               })
               .PreserveExistingDefaults();

            base.Load(builder);
        }

        public static void Register(ContainerBuilder builder, params Assembly[] additionalAssemblies)
        {
            builder.RegisterModule(new Registration(additionalAssemblies));
        }
    }
}