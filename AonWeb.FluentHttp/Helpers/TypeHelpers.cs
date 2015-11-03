using System;
using System.Reflection;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Helpers
{
    public static class TypeHelpers
    {
        public static TScope As<TScope>(this IBuilderScope scope)
            where TScope : IBuilderScope
        {
            return (TScope)scope;
        }

        public static object GetDefaultValueForType(Type type)
        {
            if (!IsNullable(type))
                return Activator.CreateInstance(type);

            return null;
        }

        
        public static bool IsAssignableFrom(this Type baseType, Type childType)
        {
            return baseType.IsAssignableFrom(childType.GetTypeInfo());
        }

        public static bool IsAssignableFrom(this Type baseType, TypeInfo childTypeInfo)
        {
            return baseType.GetTypeInfo().IsAssignableFrom(childTypeInfo);
        }

        public static bool IsInstanceOfType(this Type baseType, object obj)
        {
            return obj != null && baseType.IsAssignableFrom(obj.GetType());
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