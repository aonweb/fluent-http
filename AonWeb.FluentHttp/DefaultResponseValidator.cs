using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public class DefaultResponseValidator: ITypedResponseValidator
    {
        public DefaultResponseValidator()
            : this(new HashSet<HttpStatusCode>(Defaults.ValidStatusCodes)) { }

        public DefaultResponseValidator(ISet<HttpStatusCode> validStatusCodes)
        {
            ValidStatusCodes = validStatusCodes;
        }

        public ISet<HttpStatusCode> ValidStatusCodes { get; }
        public bool IsValid(HttpResponseMessage response)
        {
            return ValidStatusCodes.Contains(response.StatusCode);
        }
    }

    public class Defaults
    {
        public static IEnumerable<HttpStatusCode> ValidStatusCodes =>
            new[]
            {
                HttpStatusCode.OK,
                HttpStatusCode.Created,
                HttpStatusCode.Accepted,
                HttpStatusCode.NonAuthoritativeInformation,
                HttpStatusCode.NoContent,
                HttpStatusCode.ResetContent,
                HttpStatusCode.PartialContent,
                HttpStatusCode.NotModified
            };
    }
}