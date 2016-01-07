using System.Net.Http;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Helpers;

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
                TypeHelpers.CheckType<HttpResponseMessage>(value);

                Result = (HttpResponseMessage)value;
            }
        }
    }
}