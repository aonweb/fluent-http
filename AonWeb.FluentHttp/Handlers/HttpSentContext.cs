using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpSentContext : HttpCallContext
    {
        public HttpSentContext(HttpCallContext context, HttpResponseMessage response)
            : base(context)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; set; }

        public bool IsSuccessfulResponse()
        {
            return IsSuccessfulResponse(Response);
        }
    }
}