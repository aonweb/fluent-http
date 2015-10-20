

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace AonWeb.FluentHttp.Autofac
{
    public class Registration: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblyNames = new[] { "Amc.Api.Core", "Amc.Api.Catalog", "Amc.Api.Account", "Amc.Api.Commerce" };

            IList<Assembly> assemblies = new List<Assembly>();

            foreach (var name in assemblyNames)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(name));

                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                }
            }


            base.Load(builder);
        }

        public static void Register(ContainerBuilder builder)
        {
            builder.RegisterModule(new Registration());
        }
    }
}