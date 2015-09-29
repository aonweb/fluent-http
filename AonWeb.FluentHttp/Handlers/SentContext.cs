using System.Net.Http;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Handlers
{
    public class SentContext : HandlerContext, IHandlerContextWithResult<HttpResponseMessage>, IHandlerContextWithResult
    {
        private readonly Modifiable<HttpResponseMessage> _result;

        public SentContext(IHttpBuilderContext context, HttpRequestMessage request, HttpResponseMessage result)
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
                if (!(value is HttpResponseMessage))
                    throw new TypeMismatchException(typeof(HttpResponseMessage), value?.GetType());

                Result = (HttpResponseMessage)value;
            }
        }
    }
}