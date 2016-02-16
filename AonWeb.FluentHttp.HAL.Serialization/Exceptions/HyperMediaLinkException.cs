using System;

namespace AonWeb.FluentHttp.HAL.Serialization.Exceptions
{
    public abstract class HyperMediaLinkException : Exception
    {
        protected HyperMediaLinkException(string message)
            : this(message, null)
        { }

        protected HyperMediaLinkException(string message, Exception exception) 
            : base(message, exception)
        { }
    }
}