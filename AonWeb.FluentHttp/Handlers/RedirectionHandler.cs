using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class RedirectHandler : Handler
    {
        
        public RedirectHandler()
        {
            Enabled = Defaults.Handlers.AutoRedirectEnabled;
            MaxAutoRedirects = Defaults.Handlers.MaxAutoRedirects;
            RedirectStatusCodes = new HashSet<HttpStatusCode>(Defaults.Handlers.RedirectStatusCodes);

            RedirectValidtor = ShouldRedirect;
        }

        private int MaxAutoRedirects { get; set; }
        private static ISet<HttpStatusCode> RedirectStatusCodes { get; set; }
        private Func<SentContext, bool> RedirectValidtor { get; set; }
        private Action<RedirectContext> OnRedirect { get; set; }

        public RedirectHandler WithAutoRedirect(bool enabled = true)
        {
            if (enabled)
                return WithAutoRedirect(-1);

            Enabled = false;

            return this;
        }

        public RedirectHandler WithAutoRedirect(int maxAutoRedirects)
        {
            Enabled = true;

            if (maxAutoRedirects >= 0)
                MaxAutoRedirects = maxAutoRedirects;

            return this;
        }

        public RedirectHandler WithRedirectStatusCode(HttpStatusCode statusCode)
        {
            if (!RedirectStatusCodes.Contains(statusCode))
                RedirectStatusCodes.Add(statusCode);

            return this;
        }

        public RedirectHandler WithRedirectValidator(Func<SentContext, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            RedirectValidtor = validator;

            return this;
        }

        public RedirectHandler WithCallback(Action<RedirectContext> callback)
        {
            OnRedirect = (Action<RedirectContext>)Delegate.Combine(OnRedirect, callback);

            return this;
        }

        public override HandlerPriority GetPriority(HandlerType type)
        {
            if (type == HandlerType.Sent)
                return HandlerPriority.High;

            return base.GetPriority(type);
        }

        public override async Task OnSent(SentContext context)
        {
            if (!RedirectValidtor(context)) 
                return;
            
            var uri = context.Uri;

            var redirectCount = context.Items["RedirectCount"].As<int?>().GetValueOrDefault();

            if (redirectCount >= MaxAutoRedirects)
                throw new MaximumAutoRedirectsException(context.Result.StatusCode, string.Format(SR.MaxAutoRedirectsErrorFormat, redirectCount, context.Result.DetailsForException()));

            var newUri = GetRedirectUri(uri, context.Result);

            var ctx = new RedirectContext
            {
                StatusCode = context.Result.StatusCode,
                RequestMessage = context.Result.RequestMessage,
                RedirectUri = newUri,
                CurrentUri = uri,
                CurrentRedirectionCount = redirectCount
            };

            if (ctx.RedirectUri == null)
                return;

            OnRedirect?.Invoke(ctx);

            if (!ctx.ShouldRedirect) 
                return;

            context.Builder.WithUri(ctx.RedirectUri);
            context.Items["RedirectCount"] = redirectCount + 1;

            // dispose of previous response
            ObjectHelpers.DisposeResponse(context.Result);

            context.Result = await context.Builder.RecursiveResultAsync(context.Token);
            
        }

        private bool ShouldRedirect(SentContext context)
        {
            return RedirectStatusCodes.Contains(context.Result.StatusCode);
        } 

        private static Uri GetRedirectUri(Uri originalUri, HttpResponseMessage response)
        {
            var locationUri = response.Headers.Location;

            if (locationUri == null)
                return null;

            if (locationUri.IsAbsoluteUri)
                return locationUri;

            if (UriHelpers.IsAbsolutePath(locationUri.OriginalString))
                return new Uri(originalUri, locationUri);

            return new Uri(UriHelpers.CombineVirtualPaths(originalUri.GetSchemeHostPath(), locationUri.OriginalString));
        }   
    }
}