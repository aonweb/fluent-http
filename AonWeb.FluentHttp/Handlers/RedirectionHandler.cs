using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Handlers
{
    public class RedirectHandler : HttpCallHandler
    {
        private const int DefaultMaxAutoRedirects = 5;

        // TODO: allow this to be configurable?
        private static readonly HashSet<HttpStatusCode> RedirectStatusCodes = new HashSet<HttpStatusCode> { HttpStatusCode.Redirect, HttpStatusCode.MovedPermanently, HttpStatusCode.Created };
        
        public RedirectHandler()
        {
            AllowAutoRedirect = true;
            MaxAutoRedirects = DefaultMaxAutoRedirects;
        }


        private bool AllowAutoRedirect { get; set; }
        private int MaxAutoRedirects { get; set; }
        private Action<HttpRedirectContext> OnRedirect { get; set; }

        public RedirectHandler WithAutoRedirect()
        {
            return WithAutoRedirect(-1);
        }

        public RedirectHandler WithAutoRedirect(int maxAutoRedirects)
        {
            AllowAutoRedirect = true;

            if (maxAutoRedirects >= 0)
                MaxAutoRedirects = maxAutoRedirects;

            return this;
        }

        public RedirectHandler WithCallback(Action<HttpRedirectContext> callback)
        {
            OnRedirect = Helper.MergeAction(OnRedirect, callback);

            return this;
        }

        public override HttpCallHandlerPriority GetPriority(HttpCallHandlerType type)
        {
            if (type == HttpCallHandlerType.Sent)
                return HttpCallHandlerPriority.High;

            return base.GetPriority(type);
        }

        public override async Task OnSent(HttpCallContext context)
        {
            if (AllowAutoRedirect && IsRedirect(context.Response))
            {
                var uri = context.Uri;

                var redirectCount = context.Items["RedirectCount"].As<int?>().GetValueOrDefault();

                if (redirectCount > MaxAutoRedirects)
                    throw new MaximumAutoRedirectsException(context.Response.StatusCode, string.Format(SR.MaxAutoRedirectsErrorFormat, redirectCount, uri));

                var newUri = GetRedirectUri(uri, context.Response);

                if (newUri == null)
                    return;

                var ctx = new HttpRedirectContext
                {
                    StatusCode = context.Response.StatusCode,
                    RequestMessage = context.Response.RequestMessage,
                    RedirectUri = newUri,
                    CurrentUri = uri,
                    CurrentRedirectionCount = redirectCount
                };

                if (OnRedirect != null)
                    OnRedirect(ctx);

                context.Builder.WithUri(ctx.RedirectUri);
                context.Items["RedirectCount"] = redirectCount + 1;
                context.Response = await context.Builder.ResultAsync();
            }
        }

        private static bool IsRedirect(HttpResponseMessage response)
        {
            return RedirectStatusCodes.Contains(response.StatusCode);
        } 

        private static Uri GetRedirectUri(Uri originalUri, HttpResponseMessage response)
        {
            var locationUri = response.Headers.Location;

            if (locationUri == null)
                return null;

            if (locationUri.IsAbsoluteUri)
                return locationUri;

            return new Uri(originalUri.GetLeftPart(UriPartial.Authority) + locationUri.PathAndQuery);
        }   
    }
}