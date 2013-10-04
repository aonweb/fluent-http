using System;
using System.Net;
using System.Runtime.Serialization;

namespace AonWeb.Fluent.Http.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the maximum number of automatically handled redirect responses from a request is reached.
    /// </summary>
    public class MaximumAutomaticRedirectsException : Exception
    {
        public MaximumAutomaticRedirectsException() { }

        public MaximumAutomaticRedirectsException(string message)
            : base(message) {  }

        public MaximumAutomaticRedirectsException(string message, Exception exception) :
            base(message, exception) { }

        protected MaximumAutomaticRedirectsException(SerializationInfo info, StreamingContext context) :
            base(info, context) { }      
    }
}