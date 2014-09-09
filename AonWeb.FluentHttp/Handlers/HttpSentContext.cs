using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpSentContext : HttpCallContext, IHttpCallHandlerContextWithResult<HttpResponseMessage>
    {
        private readonly ModifyTracker<HttpResponseMessage> _result;

        public HttpSentContext(HttpCallContext context, HttpResponseMessage result)
            : base(context)
        {
            _result = new ModifyTracker<HttpResponseMessage>(result);
            Request = result.RequestMessage;
        }

        public HttpRequestMessage Request { get; private set; }

        public HttpResponseMessage Result 
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public bool IsSuccessfulResponse()
        {
            return IsSuccessfulResponse(Result);
        }

        public ModifyTracker GetHandlerResult()
        {
            return _result;
        }
    }
}