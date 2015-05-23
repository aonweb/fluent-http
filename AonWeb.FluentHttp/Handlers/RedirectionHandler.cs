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
        
        public RedirectHandler()
        {
            Enabled = HttpCallBuilderDefaults.AutoRedirectEnabled;
            MaxAutoRedirects = HttpCallBuilderDefaults.DefaultMaxAutoRedirects;
            RedirectStatusCodes = new HashSet<HttpStatusCode>(HttpCallBuilderDefaults.DefaultRedirectStatusCodes);

            RedirectValidtor = ShouldRedirect;
        }

        private int MaxAutoRedirects { get; set; }
        private static ISet<HttpStatusCode> RedirectStatusCodes { get; set; }
        private Func<HttpSentContext, bool> RedirectValidtor { get; set; }
        private Action<HttpRedirectContext> OnRedirect { get; set; }

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

        public RedirectHandler WithRedirectValidator(Func<HttpSentContext, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            RedirectValidtor = validator;

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

        public override async Task OnSent(HttpSentContext context)
        {
            if (!RedirectValidtor(context)) 
                return;
            
            var uri = context.Uri;

            var redirectCount = context.Items["RedirectCount"].As<int?>().GetValueOrDefault();

            if (redirectCount >= MaxAutoRedirects)
                throw new MaximumAutoRedirectsException(context.Result.StatusCode, string.Format(SR.MaxAutoRedirectsErrorFormat, redirectCount, context.Result.DetailsForException()));

            var newUri = GetRedirectUri(uri, context.Result);

            var ctx = new HttpRedirectContext
            {
                StatusCode = context.Result.StatusCode,
                RequestMessage = context.Result.RequestMessage,
                RedirectUri = newUri,
                CurrentUri = uri,
                CurrentRedirectionCount = redirectCount
            };

            if (ctx.RedirectUri == null)
                return;

            if (OnRedirect != null)
                OnRedirect(ctx);

            if (!ctx.ShouldRedirect) 
                return;

            context.Builder.WithUri(ctx.RedirectUri);
            context.Items["RedirectCount"] = redirectCount + 1;

            // dispose of previous response
            Helper.DisposeResponse(context.Result);

            context.Result = await context.Builder.RecursiveResultAsync();
            
        }

        private bool ShouldRedirect(HttpSentContext context)
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

            if (locationUri.IsAbsoluteUri)
                return new Uri(originalUri, locationUri);

            return new Uri(Helper.CombineVirtualPaths(originalUri.GetLeftPart(UriPartial.Path), locationUri.OriginalString));
        }   
    }
}