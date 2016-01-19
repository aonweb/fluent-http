using AonWeb.FluentHttp.Autofac;
using AonWeb.FluentHttp.Caching;
using AonWeb.FluentHttp.Tests.AutofacTests;
using Autofac;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public static class RegistrationHelpers
    {
        public static IContainer CreateContainer(bool singleInstanceCache = true)
        {
            var builder = new ContainerBuilder();

            if (singleInstanceCache)
                builder.RegisterType<CustomCacheProvider>()
                    .As<ICacheProvider>()
                    .InstancePerDependency();

            Registration.Register(builder, new[] { typeof(RegistrationHelpers).Assembly }, new [] {typeof(CustomScopeTypedCacheHandler)});
            return builder.Build();
        }
    }
}