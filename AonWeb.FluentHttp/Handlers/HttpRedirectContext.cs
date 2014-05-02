using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpRedirectContext
    {
        public HttpStatusCode StatusCode { get; internal set; }
        public HttpRequestMessage RequestMessage { get; internal set; }
        public Uri RedirectUri { get; set; }
        public Uri CurrentUri { get; internal set; }
        public int CurrentRedirectionCount { get; internal set; }
    }
}