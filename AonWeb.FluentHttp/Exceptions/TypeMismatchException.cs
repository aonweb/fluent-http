using System;
using System.Reflection;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an unhandled error in the http call.
    /// </summary>
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(Type expectedType, Type actualType)
            : this(expectedType, actualType, string.Empty) { }

        public TypeMismatchException(Type expectedType, Type actualType, string message)
           : this(expectedType, actualType, message, null)
        { }

        public TypeMismatchException(Type expectedType, Type actualType, string message, Exception exception) :
            base(string.Format(SR.TypeMismatchErrorFormat, expectedType.FormattedTypeName(), actualType.FormattedTypeName(), message), exception)
        {
            ExpectedType = expectedType?.GetTypeInfo();
            ActualType = actualType?.GetTypeInfo();
        }

        public TypeInfo ExpectedType { get; private set; }
        public TypeInfo ActualType { get; private set; }
    }
}