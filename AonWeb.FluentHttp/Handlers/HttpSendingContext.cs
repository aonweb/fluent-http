using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpSendingContext : HttpCallContext
    {
        public HttpSendingContext(HttpCallContext context, HttpRequestMessage request) 
            : base(context)
        {
            Request = request;
        }

        public HttpRequestMessage Request { get; private set; }
        public HttpContent Content { get { return Request.Content; } }

        public HttpResponseMessage Response { get; set; }
    }
}