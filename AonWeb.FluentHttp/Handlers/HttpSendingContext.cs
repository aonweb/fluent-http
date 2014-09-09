using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpSendingContext : HttpCallContext, IHttpCallHandlerContextWithResult<HttpResponseMessage>
    {
        private readonly ModifyTracker<HttpResponseMessage> _result;

        public HttpSendingContext(HttpCallContext context, HttpRequestMessage request) 
            : base(context)
        {
            _result = new ModifyTracker<HttpResponseMessage>();

            Request = request;
        }

        public HttpRequestMessage Request { get; private set; }

        public HttpContent Content { get { return Request.Content; } }

        public HttpResponseMessage Result
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public ModifyTracker GetHandlerResult()
        {
            return _result;
        }
    }
}