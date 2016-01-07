using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    public interface IExceptionResponseMetadata
    {
        HttpStatusCode StatusCode { get; }
        string ReasonPhrase { get; }
        long? ResponseContentLength { get; }
        string ResponseContentType { get; }
        Uri RequestUri { get; }
        HttpMethod RequestMethod { get; }
        long? RequestContentLength { get; }
        string RequestContentType { get; }
    }
}