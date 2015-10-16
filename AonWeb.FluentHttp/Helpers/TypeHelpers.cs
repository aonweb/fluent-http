using System;
using System.Linq;
using System.Reflection;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Helpers
{
    public static class TypeHelpers
    {
        public static object GetDefaultValueForType(Type type)
        {
            if (!IsNullable(type))
                return Activator.CreateInstance(type);

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

        internal static bool IsNullable(Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
                return true;

            return Nullable.GetUnderlyingType(type) != null;
        }

        public static T CheckType<T>(object value, bool suppressTypeMismatchException = false, Func<string> additionalErrorDetails = null)
        {
            var requestedType = typeof(T);

            if (ValidateType(value, requestedType, suppressTypeMismatchException, additionalErrorDetails))
                return (T)value;

            return default(T);
        }

        public static bool ValidateType(object value, Type requestedType, bool suppressTypeMismatchException = false, Func<string> additionalErrorDetails = null)
        {
            var canCast = (value == null && IsNullable(requestedType)) || requestedType.IsInstanceOfType(value);

            if (!canCast && !suppressTypeMismatchException)
                throw new TypeMismatchException(requestedType, value?.GetType(), additionalErrorDetails?.Invoke());

            return canCast;
        }
    }
}