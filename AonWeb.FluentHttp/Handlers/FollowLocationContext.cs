using System;
using System.Net;
using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class FollowLocationContext
    {
        public FollowLocationContext()
        {
            ShouldFollow = true;
        }

        public HttpStatusCode StatusCode { get; internal set; }
        public HttpRequestMessage RequestMessage { get; internal set; }
        public Uri LocationUri { get; set; }
        public Uri CurrentUri { get; internal set; }
        public bool ShouldFollow { get; set; }
    }
}