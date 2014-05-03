namespace AonWeb.FluentHttp.Handlers
{
    public class HttpResultContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        public HttpResultContext(HttpCallContext<TResult, TContent, TError> context, TResult result)
            : base(context)
        {
            Result = result;
        }

        public HttpResultContext(IHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings, TResult result)
            : base(builder, settings)
        {
            Result = result;
        }

        public TResult Result { get; set; }
    }
}