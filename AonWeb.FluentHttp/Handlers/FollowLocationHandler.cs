using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AonWeb.FluentHttp.Handlers
{
    public class FollowLocationHandler : HttpCallHandler
    {

        public FollowLocationHandler()
        {
            Enabled = HttpCallBuilderDefaults.AutoFollowLocationEnabled;
            FollowedStatusCodes = new HashSet<HttpStatusCode>(HttpCallBuilderDefaults.DefaultFollowedStatusCodes);
            FollowValidtor = ShouldFollow;
        }

        private Func<HttpSentContext, bool> FollowValidtor { get; set; }
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
        

        public FollowLocationHandler WithFollowValidator(Func<HttpSentContext, bool> validator)
        {
            if (validator == null)
                throw new ArgumentNullException("validator");

            FollowValidtor = validator;

            return this;
        }

        public FollowLocationHandler WithCallback(Action<HttpFollowLocationContext> callback)
        {
            OnFollow = Helper.MergeAction(OnFollow, callback);

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

            if (OnFollow != null)
                OnFollow(ctx);

            if (!ctx.ShouldFollow) 
                return;

            context.Builder.WithUri(ctx.LocationUri).AsGet().WithContent(string.Empty);

            // dispose of previous response
            Helper.DisposeResponse(context.Result);

            context.Result = await context.Builder.RecursiveResultAsync();
            
        }

        private bool ShouldFollow(HttpSentContext context)
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

            if (locationUri.IsAbsoluteUri)
                return new Uri(originalUri, locationUri);

            return new Uri(Helper.CombineVirtualPaths(originalUri.GetLeftPart(UriPartial.Path), locationUri.OriginalString));
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