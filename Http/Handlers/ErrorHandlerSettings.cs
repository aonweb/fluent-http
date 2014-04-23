using System;
using System.Collections.Generic;
using System.Net;

namespace AonWeb.Fluent.Http.Handlers
{
    public class ErrorHandlerSettings<TError>
    {
        public ErrorHandlerSettings()
        {
            ValidStatusCodes = new HashSet<HttpStatusCode>
            { 
                HttpStatusCode.OK, 
                HttpStatusCode.Created,
                HttpStatusCode.Accepted,
                HttpStatusCode.NonAuthoritativeInformation,
                HttpStatusCode.NoContent,
                HttpStatusCode.ResetContent, 
                HttpStatusCode.PartialContent
            };
        }

        public Action<HttpErrorContext<TError>> ErrorHandler { get; set; }
        public Action<HttpExceptionContext> ExceptionHandler { get; set; }

        // TODO: allow this to be configurable
        public HashSet<HttpStatusCode> ValidStatusCodes { get; private set; }
    }
}