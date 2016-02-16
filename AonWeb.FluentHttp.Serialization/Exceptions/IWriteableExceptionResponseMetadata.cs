using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    public interface IWriteableExceptionResponseMetadata : IExceptionResponseMetadata
    {
        new HttpStatusCode StatusCode { get; set; }
        new string ReasonPhrase { get; set; }
        new long? ResponseContentLength { get; set; }
        new string ResponseContentType { get; set; }
        new Uri RequestUri { get; set; }
        new HttpMethod RequestMethod { get; set; }
        new long? RequestContentLength { get; set; }
        new string RequestContentType { get; set; }
    }
}