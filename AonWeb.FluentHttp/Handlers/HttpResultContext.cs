using System.Net.Http;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpResultContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        public HttpResultContext(HttpCallContext<TResult, TContent, TError> context, TResult result, HttpResponseMessage response)
            : base(context)
        {
            Result = result;
            Response = response;
        }

        public HttpResultContext(IRecursiveHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings, TResult result, HttpResponseMessage response)
            : base(builder, settings)
        {
            Result = result;
            Response = response;
        }

        public TResult Result { get; set; }

        public HttpResponseMessage Response { get; private set; }
    }
}