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

        public HttpSentContext(IRecursiveHttpCallBuilder builder, HttpCallBuilderSettings settings, HttpResponseMessage response)
            : base(builder, settings)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; set; }
    }

    public class HttpSentContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        private TResult _result;

        public HttpSentContext(HttpCallContext<TResult, TContent, TError> context, HttpResponseMessage response)
            : base(context)
        {
            Response = response;
        }

        public HttpSentContext(IRecursiveHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings, HttpResponseMessage response)
            : base(builder, settings)
        {
            Response = response;
        }

        public HttpResponseMessage Response { get; set; }

        public TResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                IsResultSet = true;
            }
        }

        //this is necessary because we provide no constraints on types, so we wouldn't be able to tell if value types had been set
        public bool IsResultSet { get; set; }
    }
}