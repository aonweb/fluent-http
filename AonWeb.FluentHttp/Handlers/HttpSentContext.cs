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

        public HttpSentContext(IHttpCallBuilder builder, HttpCallBuilderSettings settings, HttpResponseMessage response)
            : base(builder, settings)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; set; }
    }

    public class HttpSentContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        public HttpSentContext(HttpCallContext<TResult, TContent, TError> context, HttpResponseMessage response)
            : base(context)
        {
            Response = response;
        }

        public HttpSentContext(IHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings, HttpResponseMessage response)
            : base(builder, settings)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; set; }
    }
}