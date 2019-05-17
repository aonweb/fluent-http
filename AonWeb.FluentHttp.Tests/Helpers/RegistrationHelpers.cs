using System;
using AonWeb.FluentHttp.Autofac;
using AonWeb.FluentHttp.Tests.AutofacTests;
using Autofac;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public static class RegistrationHelpers
    {
        public static IContainer CreateContainer(Action<ContainerBuilder> configure = null)
        {
            var builder = new ContainerBuilder();

            configure?.Invoke(builder);

            Registration.Register(builder, new [] { typeof(RegistrationHelpers).Assembly }, new [] {typeof(CustomScopeTypedCacheHandler)});
            return builder.Build();
        }
    }
}