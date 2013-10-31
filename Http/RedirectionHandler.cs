using System;
using System.Net;
using System.Net.Http;

using AonWeb.Fluent.Http.Exceptions;

namespace AonWeb.Fluent.Http
{
    public class RedirectionSettings
    {
        public RedirectionSettings()
        {
            AllowAutoRedirect = true;
            MaxAutomaticRedirections = 5;
        }

        public bool AllowAutoRedirect { get; internal set; }
        public int MaxAutomaticRedirections { get; internal set; }
        public Action<HttpRedirectionContext> RedirectHandler { get; set; }
    }

    public class HttpRedirectionContext
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpRequestMessage RequestMessage { get; set; }
        public Uri RedirectionUri { get; set; }
        public Uri CurrentUri { get; set; }
    }

    public interface IRedirectionHandler
    {
        IRedirectionHandler WithAutoRedirect();
        IRedirectionHandler WithAutoRedirect(int maxAutomaticRedirections);
        IRedirectionHandler WithRedirectionHandler(Action<HttpRedirectionContext> handler);
        HttpRedirectionContext HandleRedirection(HttpCallBuilder builder, HttpResponseMessage response, int redirectCount = 0);
    }

    public class RedirectionHandler : IRedirectionHandler
    {

        private readonly RedirectionSettings _settings;

        public RedirectionHandler()
            : this(new RedirectionSettings()) { }

        internal RedirectionHandler(RedirectionSettings settings)
        {
            _settings = settings;
        }

        public IRedirectionHandler WithAutoRedirect()
        {
            return WithAutoRedirect(-1);
        }

        public IRedirectionHandler WithAutoRedirect(int maxAutomaticRedirections)
        {
            _settings.AllowAutoRedirect = true;

            if (maxAutomaticRedirections > 0)
                _settings.MaxAutomaticRedirections = maxAutomaticRedirections;

            return this;
        }

        public IRedirectionHandler WithRedirectionHandler(Action<HttpRedirectionContext> handler)
        {
            _settings.RedirectHandler = Utils.MergeAction(_settings.RedirectHandler, handler);

            return this;
        }

        public HttpRedirectionContext HandleRedirection(HttpCallBuilder builder, HttpResponseMessage response, int redirectCount = 0)
        {
            if (_settings.AllowAutoRedirect && IsRedirect(response))
            {
                var uri = builder.Settings.Uri;

                if (redirectCount > _settings.MaxAutomaticRedirections)
                    throw new MaximumAutomaticRedirectsException(string.Format(SR.MaxAutoRedirectsErrorFormat, redirectCount, uri));

                var newUri = GetRedirectUri(uri, response);

                var ctx = new HttpRedirectionContext
                {
                    StatusCode = response.StatusCode,
                    RequestMessage = response.RequestMessage,
                    RedirectionUri = newUri,
                    CurrentUri = uri
                };

                if (_settings.RedirectHandler != null)
                    _settings.RedirectHandler(ctx);

                return ctx;
            }

            return null;
        }

        private static bool IsRedirect(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.MovedPermanently;
        }

        private static Uri GetRedirectUri(Uri originalUri, HttpResponseMessage response)
        {
            var locationUri = response.Headers.Location;

            if (locationUri.IsAbsoluteUri)
                return locationUri;

            return new Uri(originalUri.GetLeftPart(UriPartial.Authority) + locationUri.PathAndQuery);
        } 
    }
}