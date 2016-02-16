using System.Net.Http;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Helpers;

namespace AonWeb.FluentHttp.Handlers
{
    public class HttpSentContext : HttpHandlerContext, IHandlerContextWithResult<HttpResponseMessage>, IHandlerContextWithResult
    {
        private readonly Modifiable<HttpResponseMessage> _result;

        public HttpSentContext(IHttpBuilderContext context, HttpRequestMessage request, HttpResponseMessage result)
            : base(context, request)
        {
            _result = new Modifiable<HttpResponseMessage>(result);
        }

        public HttpResponseMessage Result 
        {
            get { return _result.Value; }
            set { _result.Value = value; }
        }

        public bool IsSuccessfulResponse()
        {
            return IsSuccessfulResponse(Result);
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