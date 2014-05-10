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

    public class HttpSendingContext<TResult, TContent, TError> : HttpCallContext<TResult, TContent, TError>
    {
        private TContent _content;
        private TResult _result;

        public HttpSendingContext(HttpCallContext<TResult, TContent, TError> context, HttpRequestMessage request)
            : base(context)
        {
            Request = request;
        }

        public bool HasContent { get; private set; }
        public bool IsResultSet { get; private set; }

        public TContent Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                HasContent = true;
            }
        }

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

        public HttpRequestMessage Request { get; private set; }
    }

}