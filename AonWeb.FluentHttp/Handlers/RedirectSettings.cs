using System;
using System.Collections.Generic;
using System.Net;

namespace AonWeb.Fluent.Http.Handlers
{
    public class RedirectSettings
    {
        internal const int DefaultMaxAutoRedirects = 5;

        public RedirectSettings()
        {
            AllowAutoRedirect = true;
            MaxAutoRedirects = DefaultMaxAutoRedirects;
            RedirectStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.Redirect, HttpStatusCode.MovedPermanently, HttpStatusCode.Created };
        }

        public bool AllowAutoRedirect { get; internal set; }
        public int MaxAutoRedirects { get; internal set; }
        public Action<HttpRedirectContext> RedirectHandler { get; set; }

        // TODO: allow this to be configurable
        public HashSet<HttpStatusCode> RedirectStatusCodes { get; internal set; }
    }
}