using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockResponse: IMaybeTransient
    {
        string ContentString { set; }
        HttpContent Content { get; set; }
        Encoding ContentEncoding { get; set; }
        string ContentType { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string ReasonPhrase { get; set; }
        HttpMethod Method { get; }
        Uri RequestUri { get; }
        HttpResponseHeaders Headers { get; }
        long RequestCount { get; }
        long RequestCountForThisUrl { get; }
        HttpResponseMessage ToHttpResponseMessage();
    }
}