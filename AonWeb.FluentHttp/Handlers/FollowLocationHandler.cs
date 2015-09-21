using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class FollowLocationHandler : Handler
    {
        public FollowLocationHandler()
        {
            Enabled = Defaults.Handlers.AutoFollowLocationEnabled;
            FollowedStatusCodes = new HashSet<HttpStatusCode>(Defaults.Handlers.FollowedStatusCodes);
            FollowValidtor = ShouldFollow;
        }

        private Func<SentContext, bool> FollowValidtor { get; set; }
        private Action<HttpFollowLocationContext> OnFollow { get; set; }
        private static ISet<HttpStatusCode> FollowedStatusCodes { get; set; }

        public FollowLocationHandler WithAutoFollow(bool enabled = true)
        {
            if (enabled)
                return WithAutoFollow(-1);

            Enabled = false;

            return this;
        }

        public FollowLocationHandler WithAutoFollow(int maxAutoFollows)
        {
            Enabled = true;

            return this;
        }
        

        public FollowLocationHandler WithFollowValidator(Func<SentContext, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            FollowValidtor = validator;

            return this;
        }

        public FollowLocationHandler WithCallback(Action<HttpFollowLocationContext> callback)
        {
            OnFollow = (Action<HttpFollowLocationContext>)Delegate.Combine(OnFollow, callback);

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
            if (!FollowValidtor(context)) 
                return;
            
            var uri = context.Uri;

            var locationUri = GetLocationUri(uri, context.Result);

            var ctx = new HttpFollowLocationContext
            {
                StatusCode = context.Result.StatusCode,
                RequestMessage = context.Result.RequestMessage,
                LocationUri = locationUri,
                CurrentUri = uri
            };

            if (ctx.LocationUri == null)
                return;

            OnFollow?.Invoke(ctx);

            if (!ctx.ShouldFollow) 
                return;

            context.Builder.WithUri(ctx.LocationUri).AsGet().WithContent(string.Empty);

            // dispose of previous response
            ObjectHelpers.DisposeResponse(context.Result);

            context.Result = await context.Builder.RecursiveResultAsync(context.Token);
        }

        private static bool ShouldFollow(SentContext context)
        {
            if (!context.IsSuccessfulResponse())
                return false;

            if (!FollowedStatusCodes.Contains(context.Result.StatusCode))
                return false;

            if (context.Result.Headers.Location == null) 
                return false;

            return true;

        } 

        private static Uri GetLocationUri(Uri originalUri, HttpResponseMessage response)
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

    public class HttpFollowLocationContext
    {
        public HttpFollowLocationContext()
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