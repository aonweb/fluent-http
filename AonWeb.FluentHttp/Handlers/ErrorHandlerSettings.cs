using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Handlers
{
    public class DefaultSuccessfulResponseValidator
    {
        // TODO: allow this to be configurable
        static DefaultSuccessfulResponseValidator()
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

        public static bool IsSuccessfulResponse(HttpResponseMessage response)
        {
            return ValidStatusCodes.Contains(response.StatusCode);
        }

        public static HashSet<HttpStatusCode> ValidStatusCodes { get; private set; }
    }

    public class DefaultExceptionFactory<TError>
    {
        public static Exception CreateException(HttpErrorContext context)
        {
            return new HttpCallException(context.StatusCode);
        }

        public static Exception CreateException<TResult, TContent>(HttpErrorContext<TResult, TContent, TError> context )
        {
            return new HttpErrorException<TError>(context.Error, context.StatusCode);
        }
    }
}