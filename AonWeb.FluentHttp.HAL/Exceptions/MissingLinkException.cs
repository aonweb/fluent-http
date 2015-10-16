using System;
using System.Net;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.HAL
{
    public class HyperMediaLinkException : Exception
    {
        public HyperMediaLinkException(string message)
            : this(message, null)
        { }

        public HyperMediaLinkException(string message, Exception exception) 
            : base(message, exception)
        { }
    }

    public class MissingLinkException : HyperMediaLinkException
    {
        public MissingLinkException(string key, Type type)
            : this(key, type, null)
        { }

        public MissingLinkException(string key, Type type, Exception innerException)
            : this($"Could not find link with rel '{key}' on type {type.FormattedTypeName()}", innerException)
        { }

        public MissingLinkException(string message)
            : this(message, (Exception)null)
        { }

        public MissingLinkException(string message, Exception exception) 
            : base(message, exception)
        { }
    }

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