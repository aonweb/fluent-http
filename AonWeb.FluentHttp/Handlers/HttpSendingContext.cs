using System.Net.Http;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpSendingContext : HttpHandlerContext, IHandlerContextWithResult<HttpResponseMessage>, IHandlerContextWithResult
    {
        private readonly Modifiable<HttpResponseMessage> _result;

        public HttpSendingContext(IHttpBuilderContext context, HttpRequestMessage request) 
            : base(context, request)
        {
            _result = new Modifiable<HttpResponseMessage>();
        }

        public HttpContent Content => Request.Content;

        public HttpResponseMessage Result
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public override Modifiable GetHandlerResult()
        {
            return _result;
        }

        object IHandlerContextWithResult.Result
        {
            set
            {
                if (!(value is HttpResponseMessage))
                    throw new TypeMismatchException(typeof(HttpResponseMessage), value?.GetType());

                Result = (HttpResponseMessage)value;
            }
        }
    }
}