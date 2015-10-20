using System;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.HAL.Serialization.Exceptions
{
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
}