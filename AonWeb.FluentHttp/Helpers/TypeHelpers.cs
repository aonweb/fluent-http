using System;
using System.Linq;
using System.Reflection;

namespace AonWeb.FluentHttp.Helpers
{
    public static class TypeHelpers
    {
        public static object GetDefaultValueForType(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        public static object GetDefaultValueForType(TypeInfo type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type.GetType());

            return null;
        }

        public static string FormattedTypeName(this Type type)
        {
            return type?.GetTypeInfo().FormattedTypeName() ?? "<Unknown Type>";
        }

        public static string FormattedTypeName(this TypeInfo type)
        {
            if (type == null)
               return "<Unknown Type>";

            var name = type.Name;

            if (type.GenericTypeArguments.Length == 0)
                return name;

            var prettyName = name.Substring(0, name.IndexOf("`", StringComparison.Ordinal));

            return prettyName + "<" + string.Join(",", type.GenericTypeArguments.Select(t => FormattedTypeName(t.GetTypeInfo()))) + ">";
        }

        public static bool IsAssignableFrom(this Type baseType, Type childType)
        {
            return baseType.IsAssignableFrom(childType.GetTypeInfo());
        }

        public static bool IsAssignableFrom(this Type baseType, TypeInfo childTypeInfo)
        {
            return baseType.GetTypeInfo().IsAssignableFrom(childTypeInfo);
        }

        public static bool IsInstanceOfType(this Type type, object obj)
        {
            return obj != null && type.IsAssignableFrom(obj.GetType());
        }

        internal static T As<T>(this object @this)
        {
            return (T)@this;
        }
    }
}