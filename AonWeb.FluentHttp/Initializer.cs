using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp
{
    public class Initializer
    {
        private static bool _isInitialized; 
        private static readonly object Lock = new object();

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            lock (Lock)
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;
                InitInternal();
            }
        }

        private static void InitInternal()
        {
            var initType = typeof (IInitializer);
            IList<TypeInfo> types = null;

            try
            {
                var currentdomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
                var getassemblies = currentdomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
                var assemblies = getassemblies.Invoke(currentdomain, new object[] { }) as Assembly[];
                types = assemblies.SelectMany(a =>
                {
                    try
                    {
                        return a.ExportedTypes;
                    }
                    catch (Exception)
                    {
                        return Enumerable.Empty<Type>();
                    }
                    
                }).Where(t => initType.IsAssignableFrom(t))
                .Select(t => t.GetTypeInfo())
                .ToList();
            }
            catch (Exception)
            {
                // if the reflection code fails, let if fail silently. Don't know what platforms support it.
            }

            if (types == null || !types.Any())
                return;

            var initializers = types.Select(typeInfo =>
            {
                var ctor = typeInfo.DeclaredConstructors.FirstOrDefault(c => c.IsPublic && c.GetParameters().Length == 0);

                return (IInitializer)ctor?.Invoke(null);

            }).Where(i => i != null);

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }
        }
    }

    public interface IInitializer
    {
        void Initialize();
    }
}