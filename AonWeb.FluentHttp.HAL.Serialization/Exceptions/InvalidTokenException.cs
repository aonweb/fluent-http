using System;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Serialization.Exceptions
{
    public class InvalidTokenException : HyperMediaLinkException
    {
        public InvalidTokenException(string token, Type type)
            : this(token, type, null)
        { }

        public InvalidTokenException(string token, Type type, Exception innerException)
            : this($"Could not find link with rel '{token}' on type {type.FormattedTypeName()}", innerException)
        { }

        public InvalidTokenException(string message)
            : this(message, (Exception)null)
        { }

        public InvalidTokenException(string message, Exception exception)
            : base(message, exception)
        { }
    }
}