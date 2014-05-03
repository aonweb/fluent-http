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

        public HttpSendingContext(IHttpCallBuilder builder, HttpCallBuilderSettings settings, HttpRequestMessage request)
            : base(builder, settings)
        {
            Request = request;
        }

        public HttpRequestMessage Request { get; private set; }
        public HttpContent Content { get { return Request.Content; } }
    }

    public class HttpSendingContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        private TResult _result;

        public HttpSendingContext(HttpCallContext<TResult, TContent, TError> context, TContent content)
            : base(context)
        {
            Content = content;
        }

        public HttpSendingContext(IHttpCallBuilder<TResult, TContent, TError> builder, HttpCallBuilderSettings<TResult, TContent, TError> settings, TContent content)
            : base(builder, settings)
        {
            Content = content;
        }

        public TContent Content { get; set; }

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

        public bool IsResultSet { get; set; }
    }

}