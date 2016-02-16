using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Exceptions
{
    internal class ExceptionResponseMetadata: IWriteableExceptionResponseMetadata
    {
        public string ReasonPhrase { get; set; }
        public long? ResponseContentLength { get; set; }
        public string ResponseContentType { get; set; }
        public Uri RequestUri { get; set; }
        public HttpMethod RequestMethod { get; set; }
        public long? RequestContentLength { get; set; }
        public string RequestContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}