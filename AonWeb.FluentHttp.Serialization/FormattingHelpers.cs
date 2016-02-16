using System;
using System.Linq;
using System.Reflection;

namespace AonWeb.FluentHttp.Serialization
{
    public static class FormattingHelpers
    {
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

    }
}