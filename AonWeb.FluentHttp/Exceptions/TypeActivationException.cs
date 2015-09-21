using System;
using System.Reflection;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an a type could not be instantiated.
    /// </summary>
    public class TypeActivationException : Exception
    {
        public TypeActivationException(TypeInfo failedActivationType, int ctorArgsLength)
            : this(failedActivationType, ctorArgsLength, string.Empty)
        {
            FailedActivationType = failedActivationType;
            ConstructorArgsLength = ctorArgsLength;
        }

        public TypeActivationException(TypeInfo failedActivationType, int ctorArgsLength, string message)
            : base(string.Format(SR.ConstructorMissingErrorFormat, failedActivationType.FormattedTypeName(), ctorArgsLength, message))
        {
            FailedActivationType = failedActivationType;
            ConstructorArgsLength = ctorArgsLength;
        }

        public TypeActivationException(TypeInfo failedActivationType, int ctorArgsLength, string message, Exception exception) :
            base(string.Format(SR.ConstructorMissingErrorFormat, failedActivationType.FormattedTypeName(), ctorArgsLength, message), exception)
        {
            FailedActivationType = failedActivationType;
            ConstructorArgsLength = ctorArgsLength;
        }

        public TypeInfo FailedActivationType { get; private set; }
        public int ConstructorArgsLength { get; private set; }
    }
}