using System;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.Fluent.Http.Exceptions;

namespace AonWeb.Fluent.Http.Handlers
{
    public class RedirectHandler : IRedirectHandler
    {

        private readonly RedirectSettings _settings;

        public RedirectHandler()
            : this(new RedirectSettings()) { }

        internal RedirectHandler(RedirectSettings settings)
        {
            _settings = settings;
        }

        public IRedirectHandler WithAutoRedirect()
        {
            return WithAutoRedirect(-1);
        }

        public IRedirectHandler WithAutoRedirect(int maxAutoRedirects)
        {
            _settings.AllowAutoRedirect = true;

            if (maxAutoRedirects >= 0)
                _settings.MaxAutoRedirects = maxAutoRedirects;

            return this;
        }

        public IRedirectHandler WithHandler(Action<HttpRedirectContext> handler)
        {
            _settings.RedirectHandler = Utils.MergeAction(_settings.RedirectHandler, handler);

            return this;
        }

        public async Task Sending(HttpCallContext<IHttpCallBuilder> context)
        {
            // do nothing
        }

        public async Task Sent(HttpCallContext<IHttpCallBuilder> context)
        {
            // TODO: Reconfigure for create and other methods that require get
            if (_settings.AllowAutoRedirect && IsRedirect(context.Response))
            {
                var uri = context.Uri;

                var redirectCount = context.Items["RedirectCount"].As<int?>().GetValueOrDefault();

                if (redirectCount > _settings.MaxAutoRedirects)
                    throw new MaximumAutoRedirectsException(string.Format(SR.MaxAutoRedirectsErrorFormat, redirectCount, uri));

                var newUri = GetRedirectUri(uri, context.Response);

                if (newUri == null)
                    return;

                // TODO: is there any additional data a consumer would need in the HttpRedirectContext to reason about
                var ctx = new HttpRedirectContext
                {
                    StatusCode = context.Response.StatusCode,
                    RequestMessage = context.Response.RequestMessage,
                    RedirectUri = newUri,
                    CurrentUri = uri
                };

                if (_settings.RedirectHandler != null)
                    _settings.RedirectHandler(ctx);

                context.Builder.WithUri(ctx.RedirectUri);
                context.Items["RedirectCount"] = redirectCount + 1;
                context.Response = await context.Builder.ResultAsync();

            }
        }

        private bool IsRedirect(HttpResponseMessage response)
        {
            return _settings.RedirectStatusCodes.Contains(response.StatusCode);
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